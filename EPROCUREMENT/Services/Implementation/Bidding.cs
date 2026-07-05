using EPROCUREMENT.DTO;
using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using EPROCUREMENT.sap;
using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
	public class Bidding : IBidding
	{
		private readonly ProcurementDBContext _procurementDBContext;
        public Bidding(ProcurementDBContext procurementDBContext)
        {
			_procurementDBContext = procurementDBContext;	
        }

        public async Task<int> AcceptBidding(int pkg_id, int user_id)
        {
			var accept_offers = new accepted_offers();
			accept_offers.pkg_header_id = pkg_id;
			accept_offers.user_id = user_id;

			await _procurementDBContext.accepted_offers.AddAsync(accept_offers);

			var result = await _procurementDBContext.SaveChangesAsync();

			return result;
        }
		public async Task<IQueryable<Accepted_Offers_ViewModel>> AcceptedoffersDetails(int pkg_id)
		{
			return await Task.Run(() => _procurementDBContext.Accepted_Offers_ViewModels.FromSqlRaw("CALL accepted_offers_report({0});", pkg_id));
		}

		public async Task<List<packages_header>> GetAcceptedOffers(
			int project_id, int industry_id , CancellationToken cancellationToken)
        {
            var query =
                    from ao in _procurementDBContext.accepted_offers
                    join ph in _procurementDBContext.packages_header
                        on ao.pkg_header_id equals ph.id
                    where ph.project_id == project_id
                    select ph;

            if (industry_id > 0)
            {
                query = query.Where(ph => ph.industry_id == industry_id);
            }

            return await query
                .Distinct()
				.AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IQueryable<Awarded_Packages_ViewModel>> GetAwarded_Packages(int user_id)
        {
            return await Task.Run(() => _procurementDBContext.Awarded_Packages_ViewModels.FromSqlRaw("CALL awarded_pkgs({0});", user_id));
        }

        public async Task<IQueryable<Awarded_Packages_ViewModel>> GetNotAwarded_Packages(int user_id)
        {
            return await Task.Run(() => _procurementDBContext.Awarded_Packages_ViewModels.FromSqlRaw("CALL not_awarded_pkgs({0});", user_id));
        }
        public async Task<IQueryable<Rebidding_Packges_Details_ViewModel>> GetPackgesDetailsRebidded(int user_id, int pkg_id)
        {
            string sqlQuery = $"CALL rebidding_packg_details({user_id}, {pkg_id});";
            return await Task.Run(() => _procurementDBContext.Rebidding_Packges_Details_ViewModels.FromSqlRaw(sqlQuery));
        }
        public async Task<IQueryable<packages_rebidded>> GetPackgesForRebedding(int user_id)
        {
            return await Task.Run(() => _procurementDBContext.Packages_Rebiddeds.FromSqlRaw("CALL rebidding_packges({0});", user_id));
        }

        public async Task<IQueryable<price_comparison_vm>> PriceComparison(int pkg_id)
		{
			return await Task.Run(() => _procurementDBContext.Price_Comparison_Vms.FromSqlRaw("CALL price_comparison({0});", pkg_id));
		}

		public async Task<int> VendorBidding(VendorBiddingDTO vendorBiddingDTO)
		{
			int returned_result = 0;
			var pkg_details_id = vendorBiddingDTO.biddingSelectedRowsDTOs[0].pkg_details_id;

			var pkg_id = await _procurementDBContext.packages_details
				                   .Where(x => x.id == pkg_details_id)
				                   .Select(x => x.pkg_id)
								   .FirstOrDefaultAsync();

			var pkg_header = await _procurementDBContext.packages_header
				                     .Where(x => x.id == pkg_id)
									 .FirstOrDefaultAsync();
		
			pkg_header.bid = true;


			_procurementDBContext.Entry(pkg_header).State = EntityState.Modified;

			var result = await _procurementDBContext.SaveChangesAsync();

			
			if (result > 0)
			{
				foreach (var item in vendorBiddingDTO.biddingSelectedRowsDTOs)
				{
					var vendor_biddings = new vendor_biddings();

					vendor_biddings.price = item.price;
					vendor_biddings.pkg_details_id = item.pkg_details_id;
					vendor_biddings.user_id = vendorBiddingDTO.user_id;
                    vendor_biddings.advanced_payment = vendorBiddingDTO.advanced_payment;
                    vendor_biddings.file_path = vendorBiddingDTO.filePath;
                    vendor_biddings.delivery_date = vendorBiddingDTO.delivery_date;
					vendor_biddings.duration_days = vendorBiddingDTO.durationdays;
					vendor_biddings.works_payment = vendorBiddingDTO.workspayment;
					vendor_biddings.material_payment = vendorBiddingDTO.materialworks;
					vendor_biddings.transportation = vendorBiddingDTO.transportation;

                    await _procurementDBContext.vendor_biddings.AddAsync(vendor_biddings);
				}
                returned_result = await _procurementDBContext.SaveChangesAsync();

                if (returned_result > 0)
				{
					var biddings = await GetBiddingCount(pkg_id);

					var biddingList = await biddings.ToListAsync();

                    var tasks = biddingList.Select(item =>
                    {
                        var biddingViewModel = new SapBiddingViewModel
                        {
                            ITEM = int.Parse(item.line_item),
                            NO_OF_QOTS = item.bidding_count.ToString(),
                            PR = item.pr_num
                        };

                        return SapApi.UpdatBiddingToSAPAsync(biddingViewModel);
                    });

                    await Task.WhenAll(tasks);

                }
			}

			return returned_result;

		}

        public async Task<int> VendorRebidding(VendorRebiddingDTO vendorRebiddingDTO)
        {
			   int result = 0;

			   foreach (var item in vendorRebiddingDTO.biddingSelectedRowsDTOs)
				{
					var vendor_bidded = await _procurementDBContext.vendor_biddings
											.Where(x => x.id == item.bid_id)
											.FirstOrDefaultAsync();

					vendor_bidded.price = item.price;

					vendor_bidded.advanced_payment = vendorRebiddingDTO.advanced_payment;
					vendor_bidded.delivery_date = vendorRebiddingDTO.delivery_date;
					vendor_bidded.material_payment = vendorRebiddingDTO.materialworks;
					vendor_bidded.transportation = vendorRebiddingDTO.transportation;
					vendor_bidded.duration_days = vendorRebiddingDTO.durationdays;
					vendor_bidded.works_payment = vendorRebiddingDTO.workspayment;
					vendor_bidded.file_path = vendorRebiddingDTO.filePath;

					_procurementDBContext.Entry(vendor_bidded).State = EntityState.Modified;

					result = await _procurementDBContext.SaveChangesAsync();
				}

		   return result;
        }

		private async Task<IQueryable<BiddingCount_VM>> GetBiddingCount(int pkg_id)
		{
			return await Task.Run(() => _procurementDBContext.BiddingCount_VM.FromSqlRaw("CALL bidding_count({0});", pkg_id));
		}

	}
}