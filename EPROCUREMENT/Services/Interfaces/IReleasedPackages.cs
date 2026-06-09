using EPROCUREMENT.DTO;
using EPROCUREMENT.Models;
using EPROCUREMENT.ViewModel;
using Org.BouncyCastle.Tsp;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
	public interface IReleasedPackages
	{
		 Task<List<packages_details>> GetReleased_Packages(int pkg_id);

		 Task<List<packages_header>> GetPackages_Headers
			(int project_id, int industry_id , CancellationToken cancellationToken = default);

         Task<List<packages_header>> GetAppliedPackages(
			 int project_id, int industry_id, CancellationToken cancellationToken = default);
        Task<List<packages_header>> GetPriceComparison(
			int project_id, int industry_id , CancellationToken cancellationToken = default);

	    Task<List<int>> GetVendorIndustries(int vendor_id);

         Task<int> CancelReleasedPackage(int pkg_id , CancellationToken cancellationToken);

		 Task<IQueryable<released_packages_vm>> GetPackages_Details(int pkg_id);

		 Task<List<packages_header>> GetAllReleased(int vendor_id);

		 Task<List<released_packages>> GetSelectedPackages(int project_id, int industry_id);

		 Task<IQueryable<released_headers_vm>> GetReleased_Headers(int project_id, int industry_id);

		 Task<IQueryable<vendors_vms>> GetVendors(int pkg_id);

		Task<List<UserAssignedDTO>> GetUserAssigned(
			int pkg_id , 
			CancellationToken cancellationToken = default);
		Task<int> DeleteAssignedUser(
			int pkg_id, 
			int user_id, 
			CancellationToken cancellationToken = default);
		Task<int> AssignUserToPackage(
			int pkg_id, 
			int user_id, 
			CancellationToken cancellationToken = default);
    }
}
