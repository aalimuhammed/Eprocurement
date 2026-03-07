using EPROCUREMENT.ViewModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IProjectsList
    {
        public Task<IEnumerable<ProjectViewModel>> GetProjects();


        public Task<IQueryable<ProjectViewModel>> GetProjectList();


        public Task<IQueryable<sap_project_ViewModel>> Sap_Projects();
    }
}
