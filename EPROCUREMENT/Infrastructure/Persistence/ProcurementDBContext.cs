using EPROCUREMENT.Models;
using EPROCUREMENT.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace EPROCUREMENT.Infrastructure.Persistence
{
    public class ProcurementDBContext : DbContext
    {
        public DbSet<projects> projects { get; set; }

        public DbSet<projects_area> projects_area { get; set; }

        public DbSet<proj_packages> proj_packages { get; set; }

        public DbSet<boq_chapters> boq_chapters { get; set; }

        public DbSet<boq_standards> boq_standard { get; set; }

        public DbSet<packages_offers> packages_offers { get; set; }

        public DbSet<user_header> user_header { get; set; }

        public DbSet<sap_users> sap_users { get; set; }

        public DbSet<users_offers> users_offers { get; set; }

        public DbSet<siacadmin> siac_admin { get; set; }

        public DbSet<packages_header> packages_header { get; set; }
        public DbSet<packages_details> packages_details { get; set; }

        public DbSet<revoked_packages> revoked_packages { get; set; }

        public DbSet<released_packages> released_packages { get; set; }

        public DbSet<vendor_biddings> vendor_biddings { get; set; }

        public DbSet<service_header> service_header { get; set; }

        public DbSet<services_industries> services_industries { get; set; }

        public DbSet<service_mtr_grp> service_mtr_grp { get; set; }

        public DbSet<user_detail> user_detail { get; set; }

        public DbSet<accepted_offers> accepted_offers { get; set; }

        public DbSet<reset_password> reset_password { get; set; }

        public DbSet<package_manual> package_manual { get; set; }
         
        public DbSet<revoked_pkg_manual> revoked_pkg_manual { get; set; }
        public DbSet<sap_projects> sap_projects { get; set; }



        public virtual DbSet<ProjectPackageViewModel> ProjectPackageViewModel { get; set; }
        public virtual DbSet<ProjectViewModel> ProjectViewModels { get; set; }
        public virtual DbSet<user_header_vm> User_Header_Vms { get; set; }

        public virtual DbSet<DownloadFileViewModel> DownloadFileViewModel { get; set; }

        public virtual DbSet<bidding_users_ViewModel> Bidding_Users_ViewModels { get; set; }

        public virtual DbSet<sap_users_vm> Sap_Users_Vms { get; set; }

        public virtual DbSet<user_types_vm> User_Types_Vms { get; set; }

        public virtual DbSet<user_industry_vm> User_Industry_Vms { get; set; }

        public virtual DbSet<sap_project_ViewModel> Sap_Project_ViewModels { get; set; }

        public virtual DbSet<sap_industry_ViewModel> Sap_Industry_ViewModels { get; set; }

        public virtual DbSet<released_packages_vm> Released_Packages_Vms { get; set; }

        public virtual DbSet<ShortListedViewModel> ShortListedViewModel { get; set; }

        public virtual DbSet<released_headers_vm> Released_Headers_Vms { get; set; }

        public virtual DbSet<vendors_vms> Vendors_Vms { get; set; }

        public virtual DbSet<AlreadyApplied_Header_ViewModel> AlreadyApplied_Header_ViewModels { get; set; }

        public virtual DbSet<price_comparison_vm> Price_Comparison_Vms { get; set; }

        public virtual DbSet<packages_rebidded> Packages_Rebiddeds { get; set; }

        public virtual DbSet<Rebidding_Packges_Details_ViewModel> Rebidding_Packges_Details_ViewModels { get; set; }

		public virtual DbSet<Accepted_Offers_ViewModel> Accepted_Offers_ViewModels { get; set; }

        public virtual DbSet<ManualPackage_ViewModel> ManualPackage_ViewModels { get; set; }

		public virtual DbSet<ImportedPkgs_ViewModel> ImportedPkgs_ViewModels { get; set; }

        public virtual DbSet<Awarded_Packages_ViewModel> Awarded_Packages_ViewModels { get; set; }

        public virtual DbSet<BiddingCount_VM> BiddingCount_VM { get; set; }


        //   public virtual DbSet<AllApplied_ViewModel> AllApplied_ViewModels { get; set; }

        //  public virtual DbSet<user_industries_vm> User_Industries_Vms { get; set; }

        public ProcurementDBContext(DbContextOptions<ProcurementDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<user_types_vm>().HasNoKey();
            modelBuilder.Entity<user_industry_vm>().HasNoKey();
            modelBuilder.Entity<released_packages_vm>().HasNoKey();
            modelBuilder.Entity<ShortListedViewModel>().HasNoKey();
            modelBuilder.Entity<vendors_vms>().HasNoKey();
            modelBuilder.Entity<AlreadyApplied_Header_ViewModel>().HasNoKey();
            modelBuilder.Entity<price_comparison_vm>().HasNoKey();
			modelBuilder.Entity<Accepted_Offers_ViewModel>().HasNoKey();
			modelBuilder.Entity<ManualPackage_ViewModel>().HasNoKey();
			modelBuilder.Entity<BiddingCount_VM>().HasNoKey();
            modelBuilder.Entity<sap_users_vm>()
            .HasNoKey()  // Specify that this entity has no primary key
            .ToView("combined_view");
        }
    }
}
