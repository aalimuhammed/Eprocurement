using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EPROCUREMENT.Controllers
{
	public class LoginController : Controller
	{
		private readonly ProcurementDBContext procurementDBContext;

		public LoginController(ProcurementDBContext procurementDB)
		{
			this.procurementDBContext = procurementDB;
		}
		public IActionResult Login()
		{
			return View();
		}




		[HttpPost]
		public IActionResult Login(LoginModel loginModel)
		{
            var account = procurementDBContext.sap_users.SingleOrDefault(x => x.username == loginModel.username && x.password == loginModel.password);
            if (account != null)
            {
                // Login successful

                //return RedirectToAction("" ,)
                HttpContext.Session.SetInt32("UserId", account.id);
                return RedirectToAction("Index", "Vendor");
            }
            else
            {
                // Login failed
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }
        }
	}
}
