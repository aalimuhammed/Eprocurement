using EPROCUREMENT.DTO;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
		public async Task<IActionResult> GetRevokes(int project_id , int industry_id)
		{
			return Ok(await _revokePackages.GetRevokedPackages(project_id , industry_id));
		}

		[HttpPost]
		public async Task<IActionResult> CancelRevoke(CancelRevokeDTO cancelRevoke)
		{
			return Ok(await _revokePackages.CancelRevoke(cancelRevoke));
		}

	}
}
