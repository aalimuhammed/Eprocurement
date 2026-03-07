using EPROCUREMENT.DTO;
using EPROCUREMENT.Models;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IPackageHeader
    {
        public Task<PackageProjectIndustryDTO> GetPackagesHeaderAsync(int pkg_id);
    }
}
