using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EPROCUREMENT.DTO;
using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.ViewModel;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace EPROCUREMENT.Services.Implementation
{
    public class GetAppliedUsers : IUsersActions
    {
        private readonly ProcurementDBContext procurementDB;

        public GetAppliedUsers(ProcurementDBContext procurementDBContext)
        {
            procurementDB = procurementDBContext;
        }

		public  IQueryable<user_header_vm> AppliedUsers()
        {
            try
            {
                var applied_users =  procurementDB
                                .User_Header_Vms
                                .FromSqlRaw("CALL GetUsers()");

                return applied_users;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<IQueryable<sap_users_vm>> ExistedUsers(int page, int pageSize, bool isSapVendorChecked, bool isEProcurementVendorChecked)
        {
            try
            {
                var sap_existed_users = await procurementDB.Sap_Users_Vms
                                               .FromSqlRaw("CALL call_combinations({0}, {1}, {2}, {3})",
                                                             page, 
                                                             pageSize,
                                                             isSapVendorChecked,
                                                             isEProcurementVendorChecked)
                                              .AsNoTracking()
                                              .ToListAsync();

                return sap_existed_users.AsQueryable();


            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<IQueryable<sap_users_vm>> SearchFilter(string search, bool isSapVendorChecked, bool isEProcurementVendorChecked)
        {
            var sap_existed_users = await procurementDB.Sap_Users_Vms
                                         .FromSqlRaw("CALL call_search({0}, {1}, {2})", 
                                         search ,
                                         isSapVendorChecked, 
                                         isEProcurementVendorChecked)// Passing page and pageSize as arguments
                                        .AsNoTracking() // Optional: disable tracking for better performance
                                        .ToListAsync();

            return sap_existed_users.AsQueryable();
        }


        public IQueryable<user_types_vm> UserTypes(int userId)
		{
			try
			{
				var userIdParameter = new MySqlParameter("@user_id", userId);
                var users_types = procurementDB.User_Types_Vms.FromSqlRaw("CALL pr_user_types(@user_id)", userIdParameter);
                return users_types;
			}
			catch (System.Exception)
			{

				throw;
			}
		}


		public IQueryable<user_industry_vm> UserIndustry(int userId)
		{
			try
			{
				var userIdParameter = new MySqlParameter("@user_id", userId);
				var users_industry = procurementDB.User_Industry_Vms.FromSqlRaw("CALL pr_user_industry(@user_id)", userIdParameter);
				return users_industry;
			}
			catch (System.Exception)
			{
				throw;
			}
		}

		public async Task<string> Email(int id)
		{
			var email = await Task.Run(() => from a in procurementDB.user_header
								 where a.id == id
								 select a.email).Result.FirstOrDefaultAsync();

            return email;
		}

		public async Task<bool> Acceptance(int id)
		{
            if (id > 0)
            {
                var userToAccept = await procurementDB.user_header
                           .Where(a => a.id == id)
                           .FirstOrDefaultAsync();

                userToAccept.verifyied = true;

                procurementDB.user_header.Update(userToAccept);

                var updated = await procurementDB.SaveChangesAsync();

                return updated > 0;
            }
            

            return false;
        }

		public async Task<bool> Refused(int id)
		{
			var userTRefuse = await procurementDB.user_header
							  .Where(a => a.id == id)
							  .FirstOrDefaultAsync();

			userTRefuse.refused = true;

			procurementDB.user_header.Update(userTRefuse);

			var updated = await procurementDB.SaveChangesAsync();

			if (updated > 0)
			{
				return true;
			}
			return false;
		}

        public async Task<user_header_vm> EmailAndFiles(int id)
        {
            var email = await Task.Run(() => from a in procurementDB.user_header
                                             where a.id == id
                                             select new user_header_vm
                                             {
                                                 email = a.email,
                                                 commercial_register_document_file_path = a.commercial_register_document_file_path,
                                                 taxid_document_file_path = a.taxid_document_file_path,
                                                 FullName = a.fname , 
												 keyperson_name = a.keyperson_name
                                                 
                                             }).Result.FirstOrDefaultAsync();

            return email;
        }

        public IQueryable<DownloadFileViewModel> DownloadFileViewModels(string filename)
        {
            try
            {
                var file_path = procurementDB.DownloadFileViewModel.FromSqlRaw("CALL FilePath('"+filename+"')");

                return file_path;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

		public IQueryable<bidding_users_ViewModel> BiddingUsers()
		{
			try
			{
				var bidding_users = procurementDB.Bidding_Users_ViewModels.FromSqlRaw("CALL bidding_users()");

				return bidding_users;
			}
			catch (System.Exception)
			{

				throw;
			}
		}

		public async Task<IList<service_header>> GetServiceHeaders()
		{
			var headers = await (from a in procurementDB.service_header
								 select new
								 {
									 a,
									 Descr = a.name + " - " + a.english_desc
								 }).ToListAsync();

			// Manually map the results back to service_header
			var result = headers.Select(x =>
			{
				x.a.name = x.Descr;
				return x.a;
			}).ToList();

			return result;
		}

		public async Task<IList<services_industries>> GetServicesIndustry(int header_id)
		{
			var details = await (from a in procurementDB.services_industries
                                where a.header_id == header_id
								 select new
								 {
									 a,
									 Descr = a.descr + " - " + a.english_desc
								 }).ToListAsync();

			// Manually map the results back to service_header
			var result = details.Select(x =>
			{
				x.a.descr = x.Descr;
				return x.a;
			}).ToList();

			return result;
		}

		public IQueryable<DownloadFileViewModel> DownloadFileViewModels(string filename, int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> CountVendors(bool isSapVendorChecked , bool isEProcurementVendorChecked)
        {
            if (isSapVendorChecked)
            {
                return await procurementDB.Sap_Users_Vms.Where(x => x.sap_code != null).CountAsync();
            }

            if (isEProcurementVendorChecked)
            {
                return await procurementDB.Sap_Users_Vms.Where(x => x.sap_code == null).CountAsync();
            }

            // If both isSapVendorChecked and isEProcurementVendorChecked are false, return the total count.
            return await procurementDB.Sap_Users_Vms.CountAsync();
        }

        public async Task<int> GetSapVendorCount(CancellationToken cancellationToken = default)
        {
            var sapVendorCount = await procurementDB.Sap_Users_Vms
                                .Where(x => x.sap_code != null)
                                .CountAsync(cancellationToken);

            return sapVendorCount;
        }

        public async Task<int> GetVendorCount(CancellationToken cancellationToken = default)
        {
            var vendorCount = await procurementDB.Sap_Users_Vms
                                .Where(x => x.sap_code == null)
                                .CountAsync(cancellationToken);
            return vendorCount;
        }
        public async Task<CreateVendorDeepInsertionDTO> GetUsersForDeepInsertion(
    int id, CancellationToken cancellationToken = default)
        {
            // Fetch data
            var query = await (
                from uh in procurementDB.user_header
                join ud in procurementDB.user_detail on uh.id equals ud.user_id
                join si in procurementDB.services_industries on ud.industries_details equals si.id
                where uh.id == id
                select new
                {
                    uh.fname,
                    uh.lname,
                    uh.company,
                    uh.tax_id,
                    uh.email,
                    uh.phone,
                    uh.fax,
                    uh.phone_two,
                    uh.SalesPersonEmail,
                    uh.keyperson_name,
                    uh.keyperson_mail,
                    uh.keyperson_phone,
                    uh.areas_id,
                    uh.moneybudget,
                    uh.iso_verifyed,
                    uh.engineersno,
                    uh.projectno,
                    IndustryCode = si.industry_code
                }
            ).ToListAsync(cancellationToken);

            if (!query.Any())
                return null;

            var first = query.First();

            // Parse moneybudget safely
            decimal moneyBudget = 0;
            decimal.TryParse(first.moneybudget, out moneyBudget);

            // Determine category using a helper method
            string category = GetCategory(
                first.areas_id,
                moneyBudget,
                first.iso_verifyed,
               int.Parse( first.engineersno),
               int.Parse( first.projectno)
            );

            // Map category to BbType
            string bbType = category switch
            {
                "A" => "0001",
                "B" => "0002",
                "C" => "0003",
                "D" => "0004",
                _ => "0004"
            };

            return new CreateVendorDeepInsertionDTO
            {
                Name = $"{first.fname} {first.lname}",
                Tax_Id = first.tax_id,
                Email = first.email,
                Mobile = first.phone,
                Fax = first.fax,
                Telephone = first.phone_two,
                SalesPersonEmail = first.SalesPersonEmail,
                KeyPersonName = first.keyperson_name,
                KeyPersonEmail = first.keyperson_mail,
                KeyPersonMobile = first.keyperson_phone,
                BpType = bbType,
                Industries = query.Select(x => x.IndustryCode).Distinct().ToList()
            };
        }
        private string GetCategory(int? areaId, decimal moneyBudget, bool isoVerified, int engineersNo, decimal projectNo)
        {
            // Area 3 rules
            if (areaId == 3)
            {
                if (moneyBudget >= 100_000_000 && isoVerified && engineersNo >= 6) return "A";
                if (moneyBudget >= 30_000_000 && isoVerified  && engineersNo >= 4) return "B";
                if (moneyBudget >= 10_000_000 && !isoVerified && engineersNo >= 3) return "C";
                if (moneyBudget >= 300_000 && !isoVerified && engineersNo >= 1) return "D";

                // fallback based on money only
                if (moneyBudget >= 100_000_000) return "A";
                if (moneyBudget >= 30_000_000) return "B";
                if (moneyBudget >= 10_000_000) return "C";
                if (moneyBudget >= 300_000) return "D";
            }

            // Area 1 rules
            if (areaId == 1)
            {
                if (isoVerified)
                {
                    if (moneyBudget >= 6_000_000 && projectNo >= 20_000_000 && engineersNo >= 6) return "A";
                    if (moneyBudget >= 3_000_000 && projectNo >= 10_000_000 && engineersNo >= 4) return "B";
                }
                else
                {
                    if (moneyBudget >= 1_000_000 && projectNo >= 5_000_000 && engineersNo >= 3) return "C";
                    if (moneyBudget >= 400_000 && projectNo >= 1_000_000 && engineersNo >= 1) return "D";
                }

                // fallback based on money only
                if (moneyBudget >= 6_000_000) return "A";
                if (moneyBudget >= 3_000_000) return "B";
                if (moneyBudget >= 1_000_000) return "C";
                if (moneyBudget >= 400_000) return "D";
            }

            return "D"; // default
        }
    }
}
