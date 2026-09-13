using EPROCUREMENT.DTO;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
	public class RevokedPackagesController : Controller
	{
		private readonly IRevokePackages _revokePackages;
        public RevokedPackagesController(IRevokePackages revokePackages)
        {
			_revokePackages = revokePackages;
        }
		public IActionResult RevokePackages()
		{
            var userId = HttpContext.Session.GetInt32("AdminId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
		}

        [HttpPost]
		public async Task<IActionResult> Revoke(RevokingPackageDTO revokePackages)
		{
		 	return Ok( await _revokePackages.revokePackages(revokePackages));
		}

		[HttpPost]
		public async Task<IActionResult> RevokeManual(RevokingPackageManualDTO revokePackages)
		{
			return Ok(await _revokePackages.revokePackagesManula(revokePackages));
		}

		[HttpGet]
		public async Task<IActionResult> GetRevokes(
			int project_id , int industry_id , CancellationToken cancellationToken)
		{
			return Ok(await _revokePackages.GetRevokedPackages(
				project_id , industry_id , cancellationToken));
		}

		[HttpPost]
		public async Task<IActionResult> CancelRevoke(CancelRevokeDTO cancelRevoke)
		{
			return Ok(await _revokePackages.CancelRevoke(cancelRevoke));
		}

	}
}