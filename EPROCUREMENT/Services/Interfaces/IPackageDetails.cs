using EPROCUREMENT.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IPackageDetails
    {
        public Task<IEnumerable<PackageDetailsViewModel>> PackageDetails(int boq_stand_id , int project_id);
    }
}
