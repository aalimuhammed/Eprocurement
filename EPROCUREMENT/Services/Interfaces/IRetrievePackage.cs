using EPROCUREMENT.ViewModel;
using System.Linq;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IRetrievePackage
    {
         IQueryable<ProjectPackageViewModel> GetProjectPackages(int proj_id);
    }
}