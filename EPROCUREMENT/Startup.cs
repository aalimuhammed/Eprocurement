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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));


            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromHours(2);
            });

            //services.AddAutoMapper(typeof(MappingProfile));

            var defaultConnectionString = Configuration.GetConnectionString("DefaultConnection");


            services.AddDbContext<ProcurementDBContext>(options => options.UseMySql(defaultConnectionString, ServerVersion.AutoDetect(defaultConnectionString)));

            //var mappingconfig = new MapperConfiguration(o =>
            //{
            //    o.AddProfile(new MappingProfile());
            //});

            //IMapper mapper = mappingconfig.CreateMapper();


            //services.AddSingleton(mapper);

            services.AddSingleton<IFileProvider>(
           new PhysicalFileProvider(
               Path.Combine(Directory.GetCurrentDirectory(), "Resources/PackagesFiles")));

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    //options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

          //  turn off options.CheckConsentNeeded = context => true;

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddTransient<IMailService, MailService>();

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            //cookiePolicyOptions = new CookiePolicyOptions
            //{
            //    MinimumSameSitePolicy = SameSiteMode.Lax
            //};

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=VendorLogin}/{action=Index}/{id?}");
            });
        }
    }
}
