using EPROCUREMENT.DTO;
using EPROCUREMENT.Models;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IUploadOffer
    {
        public Task<int> UploadUserOffer(PackageOfferDTO packages_Offer);
    }
}
