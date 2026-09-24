using EPROCUREMENT.DTO;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IAddPackages
    {
        public Task<int> AddNewPackages(ProjectPackageDTO NewProjectPackage);

    }
}
