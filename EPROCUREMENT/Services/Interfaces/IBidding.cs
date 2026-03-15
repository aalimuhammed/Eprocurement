using EPROCUREMENT.DTO;
using EPROCUREMENT.Models;
using EPROCUREMENT.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
	public interface IBidding
	{
		public Task<int> VendorBidding(VendorBiddingDTO vendorBiddingDTO);

		public Task<int> VendorRebidding(VendorRebiddingDTO vendorRebiddingDTO);

		public Task<IQueryable<price_comparison_vm>> PriceComparison(int pkg_id);

		public Task<IQueryable<packages_rebidded>> GetPackgesForRebedding(int user_id);

        public Task<IQueryable<Rebidding_Packges_Details_ViewModel>> GetPackgesDetailsRebidded(int user_id , int pkg_id);

		public Task<IQueryable<Accepted_Offers_ViewModel>> AcceptedoffersDetails(int pkg_id);

		public Task<int> AcceptBidding(int pkg_id, int user_id);

		public Task<List<packages_header>> GetAcceptedOffers(
			int project_id, int industry_id, CancellationToken cancellationToken = default);

		public Task<IQueryable<Awarded_Packages_ViewModel>> GetAwarded_Packages(int user_id);

        public Task<IQueryable<Awarded_Packages_ViewModel>> GetNotAwarded_Packages(int user_id);
    }
}
