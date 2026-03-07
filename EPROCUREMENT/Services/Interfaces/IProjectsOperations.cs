using EPROCUREMENT.Models;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IProjectsOperations
    {
        Task createproject(projects projects);

        Task<bool> UpdateProject(projects projects);

         projects GetProject(int id);

        Task<bool> DeleteProject(int id);

       
    }
}
