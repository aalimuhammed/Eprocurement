using EPROCUREMENT.Models;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
	public class ExternalUserController : Controller
	{
		private readonly IReleasedPackages _releasedPackages;
		private readonly IUsersActions _usersActions;
        public ExternalUserController(
			IReleasedPackages releasedPackages , 
			IUsersActions usersActions)
        {
			_releasedPackages = releasedPackages;
			_usersActions = usersActions;
        }
        public IActionResult VendorPackages()
		{
			return View();
		}

		public IActionResult RefusedVendors()
		{
			return View();
		}
		public IActionResult RefusedVendorProfile()
		{
			if (TempData["RefusedVendorData"] is string vendorDataJson)
			{
				var vendorData = JsonConvert.DeserializeObject<VendorData>(vendorDataJson);
				return View(vendorData);
			}
			else
			{
				return null;
			}
		}
		public IActionResult AlreadyApplied()
		{
			return View();
		}
		public async Task<IActionResult> GetPackages(int pkg_id, CancellationToken cancellationToken = default)
		{
			return Ok(await _releasedPackages.GetReleased_Packages(pkg_id, cancellationToken));
		}
		public async Task<IActionResult> GetSelectedPackages(int project_id , int industry_id)
		{
			return Ok(await _releasedPackages.GetSelectedPackages(project_id, industry_id));
		}
		public async Task<IActionResult> GetAllReleased()
		{
            var vendor_id = HttpContext.Session.GetInt32("UserId");
			if (vendor_id == null)
			{
				return RedirectToAction("Login", "Account");
            }
            
			return Ok(await _releasedPackages.GetAllReleased(vendor_id.Value));
		}
		public async Task<IActionResult> GetUserIndustry()
		{
			var vendor_id = HttpContext.Session.GetInt32("UserId");

            if (vendor_id == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var industries = _usersActions.UserIndustry(vendor_id.Value);

			return Ok(await industries.ToListAsync());
		}
		public async Task<IActionResult> GetServiceHeaders()
		{
			return Ok(await _usersActions.GetServiceHeaders());
		}
		public async Task<IActionResult> GetIndustryDetails(int header_id)
		{
			return Ok(await _usersActions.GetServicesIndustry(header_id));
		}
	}
}
