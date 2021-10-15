using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using UtilityServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using RemaSoftware.DALServices;
using RemaSoftware.DALServices.Impl;
using RemaSoftware.Helper;
using Rotativa.AspNetCore;

namespace RemaSoftware
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
            });
            services.AddControllersWithViews();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("RemaConnection"))); // per entity framework
            services.AddIdentity<MyUser, IdentityRole>(opts =>
            {
                opts.SignIn.RequireConfirmedEmail = false;
                opts.SignIn.RequireConfirmedAccount = false;
                opts.Password.RequireDigit = true;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequiredLength = 8;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                
                // se si toglie l'events fa redirect a login portandosi dietro l'url al quale si è tentato di accedere senza autenticazione, decidere che fare
                // per adesso senza return url
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx => {
                        
                        ctx.Response.Redirect(options.LoginPath);
                        return Task.FromResult(0);
                    }
                };
            });
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("RequireAdministratorRole",
            //        policy => policy.RequireRole("SuperUser", "Admin", "User"));
            //});


            var mvcBuilder = services.AddControllersWithViews();
            
            #if DEBUG
            mvcBuilder.AddRazorRuntimeCompilation();
            #endif

            services.AddNotyf(config =>
            {
                config.DurationInSeconds = 10;
                config.IsDismissable = true;
                config.Position = NotyfPosition.TopRight;
            });

            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IEmailService, EmailService>();
            //services.AddTransient<IPdfService, PdfService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IOperationService, OperationService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IWarehouseStockService, WarehouseStockService>();
            services.AddTransient<PdfHelper>();
            services.AddTransient<DashboardHelper>();
            services.AddTransient<IAPIFatturaInCloudService, APIFatturaInCloudService>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
        {
            context.Database.Migrate();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseDeveloperExceptionPage();

            }
            var hostingEnvironment = app.ApplicationServices.GetService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
            RotativaConfiguration.Setup(hostingEnvironment);
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseNotyf();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });
        }
    }
}
