using EPROCUREMENT.DTO;
using EPROCUREMENT.Models;
using EPROCUREMENT.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IUsersActions
    {
        public IQueryable<user_header_vm> AppliedUsers();

        public IQueryable<DownloadFileViewModel> DownloadFileViewModels(string filename);
        public IQueryable<DownloadFileViewModel> DownloadFileViewModels(string filename , int id);
        public Task<string> Email(int id);

        public Task<user_header_vm>  EmailAndFiles(int id);
        public Task<bool> Acceptance(int id);

        public Task<bool> Refused(int id);

        public IQueryable<bidding_users_ViewModel> BiddingUsers();

        public Task<IQueryable<sap_users_vm>> ExistedUsers(int page, int pageSize, bool isSapVendorChecked, bool isEProcurementVendorChecked);

        public IQueryable<user_types_vm> UserTypes(int userId);

        public IQueryable<user_industry_vm> UserIndustry(int userId);

        public Task<IList<service_header>> GetServiceHeaders();

        public Task<IList<services_industries>> GetServicesIndustry(int header_id);

        public Task<int> CountVendors(bool isSapVendorChecked, bool isEProcurementVendorChecked);

        public Task<IQueryable<sap_users_vm>> SearchFilter(string search, bool isSapVendorChecked, bool isEProcurementVendorChecked);

        Task<int> GetSapVendorCount(CancellationToken cancellationToken = default);
        Task<int> GetVendorCount(CancellationToken cancellationToken = default);
        Task<CreateVendorDeepInsertionDTO> GetUsersForDeepInsertion(int id, CancellationToken cancellationToken = default);

        //Task<int> Bidding(int id, string file_name);
    }
}
