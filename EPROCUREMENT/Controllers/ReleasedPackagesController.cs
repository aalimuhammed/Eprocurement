using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
	public class ReleasedPackagesController : Controller
	{
		private readonly IReleasedPackages _releasedPackages;
		private readonly IWebHostEnvironment _env;

		public ReleasedPackagesController(IReleasedPackages releasedPackages, IWebHostEnvironment env)
		{
			_releasedPackages = releasedPackages;
			_env = env;
		}
		public IActionResult Released()
		{
            var userId = HttpContext.Session.GetInt32("AdminId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
		}

		[HttpGet]
		public async Task<IActionResult> GetReleased(
			int project_id, int industry_id, CancellationToken cancellationToken)
		{
			return Ok(await _releasedPackages.GetPackages_Headers(project_id, industry_id, cancellationToken));
		}

		[HttpGet]
		public async Task<IActionResult> GetReleasedApplied(int project_id, int industry_id)
		{
			return Ok(await _releasedPackages.GetAppliedPackages(project_id, industry_id));
		}


		[HttpGet]
		public async Task<IActionResult> GetForPriceComparison(int project_id, int industry_id)
		{
			return Ok(await _releasedPackages.GetPriceComparison(project_id, industry_id));
		}

		[HttpGet]
		public async Task<IActionResult> GetReleasedHeaders(int project_id, int industry_id)
		{
			return Ok(await _releasedPackages.GetReleased_Headers(project_id, industry_id));
		}

		[HttpGet]
		public async Task<IActionResult> GetVendors(int pkg_id)
		{
			return Ok(await _releasedPackages.GetVendors(pkg_id));
		}

		[HttpGet]
		public async Task<IActionResult> GetUserAssigned(int pkg_id, CancellationToken cancellationToken)
		{
			return Ok(await _releasedPackages.GetUserAssigned(pkg_id, cancellationToken));
		}

		[HttpGet]
		public async Task<IActionResult> GetReleasedDetails(int pkg_id)
		{
			return Ok(await _releasedPackages.GetPackages_Details(pkg_id));
		}

		[HttpPost]
		public async Task<IActionResult> CanceledReleased(int pkg_id, CancellationToken cancellationToken)
		{
			return Ok(await _releasedPackages.CancelReleasedPackage(pkg_id, cancellationToken));
		}

		[HttpGet]
		public async Task<IActionResult> DownloadPackageFile(string fileName)
		{
			// Construct the file path using _Environment
			var filePath = Path.Combine(_env.WebRootPath, "packages", fileName);

			if (!System.IO.File.Exists(filePath))
			{
				return NotFound(); // Return 404 if file does not exist
			}

			// Return the file for download
			//	return File(System.IO.File.ReadAllBytes(filePath), "application/octet-stream", fileName);
			return File(System.IO.File.OpenRead(filePath), "application/octet-stream", Path.GetFileName(filePath));

			// Return the file for download
			//return PhysicalFile(fileVirtualPath, "application/force-download", Path.GetFileName(fileVirtualPath));
		}


		[HttpGet]
		public async Task<IActionResult> DownloadBiddingFile(string fileName)
		{
			// Construct the file path using _Environment
			var filePath = Path.Combine(_env.WebRootPath, "offers", fileName);

			if (!System.IO.File.Exists(filePath))
			{
				return NotFound(); // Return 404 if file does not exist
			}

			// Return the file for download
			//	return File(System.IO.File.ReadAllBytes(filePath), "application/octet-stream", fileName);
			return File(System.IO.File.OpenRead(filePath), "application/octet-stream", Path.GetFileName(filePath));

			// Return the file for download
			//return PhysicalFile(fileVirtualPath, "application/force-download", Path.GetFileName(fileVirtualPath));
		}


        [HttpDelete]
        public async Task<IActionResult> DeleteAssignedUser(int pkg_id, int user_id, CancellationToken cancellationToken)
        {
            return Ok(await _releasedPackages.DeleteAssignedUser(pkg_id, user_id, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> AssignUser(int pkg_id, int user_id, CancellationToken cancellationToken)
        {
            return Ok(await _releasedPackages.AssignUserToPackage(pkg_id, user_id, cancellationToken));
        }
    }
}