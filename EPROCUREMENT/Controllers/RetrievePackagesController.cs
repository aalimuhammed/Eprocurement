using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class RetrievePackagesController : Controller
    {

        private readonly IRetrievePackage retrievePackage;

        public RetrievePackagesController(IRetrievePackage retrieve)
        {
            this.retrievePackage = retrieve;
        }
        public IActionResult RetrievePackages(int id)
        {
           
            var result =  retrievePackage.GetProjectPackages(id);
            return View(result);
        }


        public IActionResult GetPackages(int id)
        {
            //TempData["project_id"] = id;
            HttpContext.Session.SetInt32("projectid", id);
            var result = retrievePackage.GetProjectPackages(id);
            return View(result);
        }
    }
}
