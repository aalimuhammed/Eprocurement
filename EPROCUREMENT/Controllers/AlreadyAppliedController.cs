using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
	public class AlreadyAppliedController : Controller
	{
		private readonly IAlreadyApplied _alreadyApplied;

        public AlreadyAppliedController(IAlreadyApplied alreadyApplied)
        {
            _alreadyApplied = alreadyApplied;
        }

        [HttpGet]
        public async Task<IActionResult> AlreadyAppliedHeader()
        {
			var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "VendorLogin");
            }
            return Ok(await _alreadyApplied.AlreadyAppliedHeader(userId.Value));
		}
    }
}