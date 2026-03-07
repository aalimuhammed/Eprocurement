using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectsList projects;

        public ProjectController(IProjectsList projects)
        {
            this.projects = projects;
        }
        public async Task <IActionResult> Projects()
        {

            var projectsList = await projects.GetProjectList();
            return View(projectsList);
        }

        public async Task<IActionResult> SapProjects()
        {
            var projectsList = await projects.Sap_Projects();
            return Ok(projectsList);
        }
    }
}
