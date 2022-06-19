using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using RemaSoftware.UtilityServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.Domain.Services.Impl;
using RemaSoftware.Domain.Data;
using RemaSoftware.WebApp.Helper;

namespace RemaSoftware.WebApp
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
            .SetBasePath(environment.ContentRootPath)
            .AddConfiguration(configuration)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)

            .AddEnvironmentVariables();
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
            
            

            var mvcBuilder = services.AddControllersWithViews().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            
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
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IOperationService, OperationService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IWarehouseStockService, WarehouseStockService>();
            services.AddTransient<PdfHelper>();
            services.AddTransient<DashboardHelper>();
            services.AddTransient<AccountingHelper>();
            services.AddTransient<IAPIFatturaInCloudService, APIFatturaInCloudService>(
                x=> new APIFatturaInCloudService(x.GetRequiredService<IConfiguration>(), _environment.EnvironmentName));
            services.AddTransient<OrderHelper>();
            services.AddTransient<StockHelper>();
            services.AddTransient<ClientHelper>();
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context, UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.Migrate();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseDeveloperExceptionPage();

            }
            
            if (env.EnvironmentName != "Test")
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
            DbInitializer.SeedUsersAndRoles(userManager, roleManager);
        }
    }
}
