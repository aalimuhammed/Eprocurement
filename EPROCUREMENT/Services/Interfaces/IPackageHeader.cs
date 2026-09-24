using EPROCUREMENT.DTO;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IPackageHeader
    {
         Task<PackageProjectIndustryDTO> GetPackagesHeaderAsync(int pkg_id);
         Task<int> GetMaxPackageId(int project_id , int industry_id);
    }
}
