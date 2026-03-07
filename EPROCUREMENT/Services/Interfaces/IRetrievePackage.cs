using EPROCUREMENT.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IRetrievePackage
    {
        public  IQueryable<ProjectPackageViewModel> GetProjectPackages(int proj_id);

    }
}
