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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();


            services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.Configure<EmailSettings>(
                Configuration.GetSection("EmailSettings"));

            services.Configure<GraphSettings>(
                Configuration.GetSection("GraphSettings"));

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
            });

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

            services.AddTransient<IEmailService, EmailService>();           

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
            services.AddScoped<IGraphEmailService,GraphEmailService>();
        }

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

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=VendorLogin}/{action=Index}/{id?}");
            });
        }
    }
}