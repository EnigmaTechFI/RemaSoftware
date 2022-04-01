using System;
using System.Net.WebSockets;
using System.Threading;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using RemaSoftware.DALServices;
using RemaSoftware.DALServices.Impl;
using RemaSoftware.Helper;

namespace RemaSoftware
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
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
            
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("RequireAdministratorRole",
            //        policy => policy.RequireRole("SuperUser", "Admin", "User"));
            //});


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

            services.AddSignalR();
            services.AddTransient<MyHub>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IEmailService, EmailService>();
            //services.AddTransient<IPdfService, PdfService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IOperationService, OperationService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IWarehouseStockService, WarehouseStockService>();
            services.AddTransient<PdfHelper>();
            services.AddTransient<DashboardHelper>();
            services.AddTransient<AccountingHelper>();
            services.AddTransient<IAPIFatturaInCloudService, APIFatturaInCloudService>();
            services.AddTransient<OrderHelper>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context, UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager)
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
            
            // app.UseSignalR(routes =>
            // {
            //     routes.MapHub<MyHub>("/myhub");
            // });

            //if (!env.IsEnvironment("Test"))
                //app.UseHttpsRedirection();

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
                endpoints.MapHub<MyHub>("/myhub");
            });
            DbInitializer.SeedUsersAndRoles(userManager, roleManager);
        }
        
        private async Task Echo(HttpContext context, WebSocket webSocket)  
        {  
            var buffer = new byte[1024 * 4];  
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);  
            while (!result.CloseStatus.HasValue)  
            {  
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);  
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);  
            }
        }  
    }
}
