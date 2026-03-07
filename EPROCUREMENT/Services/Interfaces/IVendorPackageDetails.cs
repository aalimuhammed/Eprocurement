using EPROCUREMENT.DTO;
using EPROCUREMENT.Models;
using EPROCUREMENT.ViewModel;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IVendorPackageDetails
    {
        // public Task<VendorPackageDetailsViewModel> VendorPackageInfo(int id);

        public Task<PackageOfferDTO> VendorPackageInfo(int id);
    }
}
