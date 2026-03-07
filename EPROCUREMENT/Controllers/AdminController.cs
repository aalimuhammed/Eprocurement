using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EPROCUREMENT.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProcurementDBContext procurementDBContext;
        private const int PageSize = 100;

        public AdminController(ProcurementDBContext procurementDB)
        {
            this.procurementDBContext = procurementDB;
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult SapPackages()
        {
            return View();
        }


        public IActionResult ShortListed()
        {
            return View();
        }

        public IActionResult PriceComparison()
        {
            return View();
        }

        public IActionResult AllApplied()
        {
            return View();
        }

        public IActionResult AcceptedOffers()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            var account = procurementDBContext.siac_admin.SingleOrDefault(x => x.username == loginModel.username && x.password == loginModel.password);
            if (account != null)
            {
                HttpContext.Session.SetInt32("AdminId", account.id);
                //return RedirectToAction("ProjectsHome", "ProjectOprations");
                return RedirectToAction("UsersList", "User");
            }
            else
            {
                // Login failed
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }
        }

        [HttpGet]
        public async Task<ActionResult> Search(string searchTerm)
        {
            IQueryable<sap_users> query;
            IQueryable<user_header> user_verifyed;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (Regex.IsMatch(searchTerm, @"^\d{15}$")) // Check for 15-digit search term
                {
                    query = procurementDBContext.sap_users
                        .Where(u => EF.Functions.Like(u.tax_id, $"%{searchTerm}%")); // Assuming id_number is the relevant field
                }
                else if (Regex.IsMatch(searchTerm, @"^\w{3}-\w{3}-\w{3}$"))
                {
                    query = procurementDBContext.sap_users
                        .Where(u => EF.Functions.Like(u.tax_id, $"%{searchTerm}%"));
                }
                else
                {
                    query = procurementDBContext.sap_users
                        .Where(u => EF.Functions.Like(u.fname, $"%{searchTerm}%"));
                }

                var sapUsersResult = await query.OrderBy(u => u.id).Take(10).ToListAsync();

                if (sapUsersResult.Any())
                {
                    return Ok(sapUsersResult);
                }

                if (Regex.IsMatch(searchTerm, @"^\d{15}$")) // Check for 15-digit search term
                {
                    user_verifyed = procurementDBContext.user_header
                                                .Where(u => EF.Functions.Like(u.tax_id, $"%{searchTerm}%")) // Assuming id_number is the relevant field
                                                .Where(h => h.verifyied == true);
                }
                else if (Regex.IsMatch(searchTerm, @"^\w{3}-\w{3}-\w{3}$"))
                {
                    user_verifyed = procurementDBContext.user_header
                                                .Where(u => EF.Functions.Like(u.tax_id, $"%{searchTerm}%"))
                                                .Where(h => h.verifyied == true);
                }
                else
                {
                    user_verifyed = procurementDBContext.user_header
                                                .Where(u => EF.Functions.Like(u.fname, $"%{searchTerm}%"))
                                                .Where(h => h.verifyied == true);
                }

                var userHeaderResult = await user_verifyed.OrderBy(u => u.id).Take(10).ToListAsync();

                if (userHeaderResult.Any())
                {
                    return Ok(userHeaderResult);
                }

                return Ok("No Data Found");
            }
            else
            {
                return BadRequest("Invalid search term format");
            }

        }

    }
}
