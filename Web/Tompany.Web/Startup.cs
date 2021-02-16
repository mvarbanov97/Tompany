namespace Tompany.Web
{
    using System.Reflection;

    using Tompany.Data;
    using Tompany.Data.Common;
    using Tompany.Data.Common.Repositories;
    using Tompany.Data.Models;
    using Tompany.Data.Repositories;
    using Tompany.Data.Seeding;
    using Tompany.Services.Data;
    using Tompany.Services.Mapping;
    using Tompany.Services.Messaging;
    using Tompany.Web.ViewModels;

    using AutoMapper;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Tompany.Services.Data.Contracts;
    using System.Linq;
    using Tompany.Web.Infrastructure.Hubs;
    using Tompany.Web.Infrastructure;
    using Tompany.Web.Infrastructure.Contracts;
    using CloudinaryDotNet;
    using Tompany.Services;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
               options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer();

            services.Configure<CookiePolicyOptions>(
                options =>
                    {
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });

            // Social Media Authentication
            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = this.configuration["Facebook:AppId"];
                    facebookOptions.AppSecret = this.configuration["Facebook:AppSecret"];
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = this.configuration["Google:ClientId"];
                    googleOptions.ClientSecret = this.configuration["Google:ClientSecret"];
                });

            // Cloudinary Account Initialization
            var cloudinaryAccount = new CloudinaryDotNet.Account(
                this.configuration["Cloudinary:Account"],
                this.configuration["Cloudinary:ApiKey"],
                this.configuration["Cloudinary:ApiSecret"]);
            var cloudinary = new Cloudinary(cloudinaryAccount);
            services.AddSingleton(cloudinary);

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddTransient<IEmailSender>(provider => new SendGridEmailSender(this.configuration["SendGridApiKey"]));
            services.AddTransient<IDestinationService, DestinationService>();
            services.AddTransient<IViewService, ViewsService>();
            services.AddTransient<ITripsService, TripsService>();
            services.AddTransient<ICarsService, CarsService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<ITripRequestsService, TripRequestsService>();
            services.AddTransient<IWatchListsService, WatchListTripsService>();
            services.AddTransient<ICloudinaryService, CloudinaryService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IChatService, ChatService>();
            services.AddTransient<INotificationService, NotificationService>();

            services.AddRazorPages();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddSingleton(this.configuration);

            services.AddAutoMapper(typeof(Startup));
            services.AddControllersWithViews();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(
                typeof(ErrorViewModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (env.IsDevelopment())
                {
                    dbContext.Database.Migrate();
                }
                else if (env.IsProduction())
                {
                    dbContext.Database.Migrate();
                }

                if (!dbContext.Users.Any())
                {
                    new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
                }
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithRedirects("/Error/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapControllerRoute(
                            name: "areas",
                            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                        endpoints.MapControllerRoute(
                            name: "default",
                            pattern: "{controller=Home}/{action=Index}/{id?}");

                        endpoints.MapHub<ChatHub>("/chatHub");
                        endpoints.MapHub<NotificationHub>("/notificationHub");
                        endpoints.MapHub<UserStatusHub>("/userStatusHub");

                        endpoints.MapRazorPages();
                    });
        }
    }
}
