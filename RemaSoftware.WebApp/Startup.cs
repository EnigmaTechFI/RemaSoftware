using System;
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
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using RemaSoftware.Domain;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.Domain.Services.Impl;
using RemaSoftware.Domain.Data;
using RemaSoftware.UtilityServices.Implementation;
using RemaSoftware.UtilityServices.Interface;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Hub;
using RemaSoftware.WebApp.Validation;

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
            
            // register azure blob storage
            var azureBlobCs = Configuration.GetValue<string>("AzureBlobStorage:ConnectionString");

            services.AddSingleton(x => new BlobServiceClient(azureBlobCs));
            services.AddSingleton(x=> 
                new OrderImageBlobService(new BlobServiceClient(azureBlobCs), 
                    Configuration.GetValue<string>("AzureBlobStorage:OrderContainerName")));
            
            services.AddSignalR().AddHubOptions<ProductionHub>(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(1800);
                options.KeepAliveInterval = TimeSpan.FromSeconds(900);
            });
            services.AddTransient<ProductionHub>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ISubBatchService, SubBatchService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IOperationService, OperationService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IWarehouseStockService, WarehouseStockService>();
            services.AddTransient<ISupplierService, SupplierService>();
            services.AddTransient<IUtilityService, UtilityService>();
            services.AddTransient<IEmployeeService, EmployeeService>();

            services.AddTransient<PdfHelper>();
            services.AddTransient<SupplierHelper>();
            services.AddTransient<DashboardHelper>();
            services.AddTransient<AccountingHelper>();
            services.AddTransient<ProductHelper>();
            services.AddTransient<IAPIFatturaInCloudService, APIFatturaInCloudService>(
                x=> new APIFatturaInCloudService(x.GetRequiredService<IConfiguration>(), _environment.EnvironmentName));
            services.AddTransient<OrderHelper>();
            services.AddTransient<SubBatchHelper>();
            services.AddTransient<StockHelper>();
            services.AddTransient<ClientHelper>();
            services.AddTransient<AccountHelper>();
            services.AddTransient<GuestHelper>();
            services.AddTransient<EmployeeHelper>();
            services.AddTransient<OrderValidation>();
            services.AddTransient<ProductValidation>();
            services.AddTransient<OperationValidation>();
            services.AddTransient<StockValidation>();
            services.AddTransient<EmployeeValidation>();

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
            app.UseCors(x=>x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseNotyf();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
                endpoints.MapHub<ProductionHub>("/productionhub");
            });
            DbInitializer.SeedUsersAndRoles(userManager, roleManager, context);
        }
    }
}
