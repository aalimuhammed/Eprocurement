using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class DeletePackageController : Controller
    {
        private readonly IDeletePackage deletePackage;

        public DeletePackageController(IDeletePackage delete)
        {
            this.deletePackage = delete;
        }

        public async Task<IActionResult> DeletePackage(int id)
        {
            await deletePackage.DeletePackage(id);
            return RedirectToAction("GetPackages", "RetrievePackages", new { id = (int)HttpContext.Session.GetInt32("projectid") });
            // return Ok(deletePackage);
        }
    }
}
