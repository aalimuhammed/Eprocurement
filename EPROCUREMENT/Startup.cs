using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Implementation;
using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.Settings;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using System;
using System.IO;
using System.Reflection;

namespace EPROCUREMENT
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime.
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // MVC + API Controllers
            services.AddControllersWithViews();

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "E-Procurement API",
                    Version = "v1"
                });
            });

            // Authentication
            services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            // Old Email Settings
            services.Configure<EmailSettings>(
                Configuration.GetSection("EmailSettings"));

            // Graph Settings
            services.Configure<GraphSettings>(
                Configuration.GetSection("GraphSettings"));

            // Session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
            });

            // Database
            var defaultConnectionString =
                Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ProcurementDBContext>(options =>
                options.UseMySql(
                    defaultConnectionString,
                    ServerVersion.AutoDetect(defaultConnectionString)));

            // File Provider
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "Resources/PackagesFiles")));

            // AutoMapper
            services.AddAutoMapper(
                Assembly.GetExecutingAssembly());

            // Old Email Service
            services.AddTransient<IEmailService, EmailService>();

            // Graph Email Service
            services.AddScoped<
                IGraphEmailService,
                GraphEmailService>();

            // Existing Services
            services.AddScoped<IProjectsList, ProjectServices>();
            services.AddScoped<IRetrievePackage, RetrieveProjectPackages>();
            services.AddScoped<IAddPackages, InsertPackages>();
            services.AddScoped<IPackageDetails, PackageDetail>();
            services.AddScoped<IGetBOQ, GetBOQ>();
            services.AddScoped<IGetBOQChapters, GetBOQChapters>();
            services.AddScoped<IDeletePackage, DeletePackage>();
            services.AddScoped<IVendorPackageDetails, VendorPackageDetails>();
            services.AddScoped<IUploadOffer, VendorUploadOffer>();
            services.AddScoped<IProjectsOperations, ProjectsOperations>();
            services.AddScoped<IUsersActions, GetAppliedUsers>();
            services.AddScoped<IRetrieveIndustry, RetrieveIndustry>();
            services.AddScoped<ICreatePackage, CreatePackage>();
            services.AddScoped<IRevokePackages, RevokePackages>();
            services.AddScoped<IReleasedPackages, ReleasedPackages>();
            services.AddScoped<IBidding, Bidding>();
            services.AddScoped<IServiceMaterials, ServiceMaterials>();
            services.AddScoped<IReports, Reports>();
            services.AddScoped<IAlreadyApplied, AlreadyApplied>();
            services.AddScoped<IExcelDataImporter, ExcelDataService>();
            services.AddScoped<IPackageHeader, PackageHeader>();
            services.AddScoped<IGetMaterialGrp, GetMaterialGrp>();
        }


        // This method gets called by the runtime.
        // Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            // Swagger
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "E-Procurement API V1");

                c.RoutePrefix = "swagger";
            });

            // Authentication
            app.UseAuthentication();

            // Authorization
            app.UseAuthorization();

            // Endpoints
            app.UseEndpoints(endpoints =>
            {
                // Existing MVC routes
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=VendorLogin}/{action=Index}/{id?}");

                // API Controllers
                endpoints.MapControllers();
            });
        }
    }
}