using EPROCUREMENT.ViewModel;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IReports
    {
        public Task<DataTable> GetShortListed(int package_header_id);

        public Task<DataTable> GetAllVendorPricesByPackageHeaderIdAsync(int packageHeaderId);
    }
}
