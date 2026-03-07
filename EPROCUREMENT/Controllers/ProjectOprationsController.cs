using EPROCUREMENT.Models;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class ProjectOprationsController : Controller
    {
        private readonly IProjectsList projects;

        private readonly IProjectsOperations projectsOperations;

        public ProjectOprationsController(IProjectsList projectsList , IProjectsOperations projectsOperations)
        {
            this.projects = projectsList;
            this.projectsOperations = projectsOperations;
        }
        public async Task<IActionResult> ProjectsHome()
        {
            var projectList = await projects.GetProjectList();
            return View(projectList);
        }

        public IActionResult InsertProject()
        {
            return View();
        }

        public async Task<IActionResult> createproject(projects projects )
        {
            await projectsOperations.createproject(projects);
            return RedirectToAction("ProjectsHome", "ProjectOprations");
            //return Ok();
        }


        public IActionResult GetProject(int id)
        {
            var project =    projectsOperations.GetProject(id);
            return View(project);
        }


        public async Task<IActionResult> UpdateProject(projects projects)
        {
            var update_prj = await projectsOperations.UpdateProject(projects);
            if (update_prj == true)
            {
                return RedirectToAction("ProjectsHome", "ProjectOprations");
            }
            return RedirectToAction("ProjectsHome", "ProjectOprations");

        }

        public async Task<IActionResult> DeleteProject(int id)
        {
            var deleted_prj = await projectsOperations.DeleteProject(id);
            return RedirectToAction("ProjectsHome", "ProjectOprations");
          //  return Ok(deleted_prj);
        }

    }
}
