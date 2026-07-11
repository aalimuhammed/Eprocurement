using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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
            var userId = HttpContext.Session.GetInt32("AdminId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        public IActionResult ShortListed()
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        public IActionResult PriceComparison()
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        public IActionResult AllApplied()
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        public IActionResult AcceptedOffers()
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }
        public IActionResult AssignMaterialGrp()
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            var account = await procurementDBContext.siac_admin
                .FirstOrDefaultAsync(x => x.username == loginModel.username);

            if (account == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(loginModel);
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(
                loginModel.password,
                account.password);

            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(loginModel);
            }

            HttpContext.Session.SetInt32("AdminId", account.id);

            return RedirectToAction("UsersList", "User");
        }

        // GET: /Admin/Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var admins = await procurementDBContext.siac_admin
                                .OrderBy(a => a.id)
                                .ToListAsync();

            ViewBag.Admins = admins;
            return View();
        }

        // POST: /Admin/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(siacadmin admin)
        {
            if (admin == null || string.IsNullOrWhiteSpace(admin.username))
            {
                TempData["ErrorMessage"] = "Username is required.";
                return RedirectToAction("Register");
            }
            var username = admin.username.ToLower();

            if (!(username.EndsWith("@siac-construction.com") ||
                  username.EndsWith("@siacholding.com")))
            {
                TempData["ErrorMessage"] = "Only siac-construction.com and siacholding.com emails are allowed.";
                return RedirectToAction("Register");
            }

            var exists = await procurementDBContext.siac_admin
                                .AnyAsync(x => x.username.ToLower() == username);
            if (exists)
            {
                TempData["ErrorMessage"] = "Username already exists.";
                return RedirectToAction("Register");
            }

            admin.password = BCrypt.Net.BCrypt.HashPassword(admin.password);
            procurementDBContext.siac_admin.Add(admin);
            await procurementDBContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Admin registered successfully.";
            return RedirectToAction("Register");
        }



        // POST: /Admin/EditAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdmin(int id, string username)
        {
            if (!(username.EndsWith("@siac-construction.com") ||
                  username.EndsWith("@siacholding.com")))
            {
                TempData["ErrorMessage"] = "Only siac-construction.com and siacholding.com emails are allowed.";
                return RedirectToAction("Register");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                TempData["ErrorMessage"] = "Username is required.";
                return RedirectToAction("Register");
            }

            var admin = await procurementDBContext.siac_admin.FindAsync(id);
            if (admin == null)
            {
                TempData["ErrorMessage"] = "Admin not found.";
                return RedirectToAction("Register");
            }

            var exists = await procurementDBContext.siac_admin
                                .AnyAsync(x => x.username == username && x.id != id);
            if (exists)
            {
                TempData["ErrorMessage"] = "Another admin already uses that username.";
                return RedirectToAction("Register");
            }

            admin.username = username;
            procurementDBContext.siac_admin.Update(admin);
            await procurementDBContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Admin updated successfully.";
            return RedirectToAction("Register");
        }

        // POST: /Admin/DeleteAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await procurementDBContext.siac_admin.FindAsync(id);
            if (admin == null)
            {
                TempData["ErrorMessage"] = "Admin not found.";
                return RedirectToAction("Register");
            }

            procurementDBContext.siac_admin.Remove(admin);
            await procurementDBContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Admin deleted.";
            return RedirectToAction("Register");
        }

        [HttpGet]
        public async Task<ActionResult> Search(string searchTerm)
        {
            IQueryable<sap_users> query;
            IQueryable<user_header> user_verifyed;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                //if (Regex.IsMatch(searchTerm, @"^\d{15}$"))
                //{
                //    query = procurementDBContext.sap_users
                //        .Where(u => EF.Functions.Like(u.tax_id, $"%{searchTerm}%"));
                //}
                //else if (Regex.IsMatch(searchTerm, @"^\w{3}-\w{3}-\w{3}$"))
                //{
                //    query = procurementDBContext.sap_users
                //        .Where(u => EF.Functions.Like(u.tax_id, $"%{searchTerm}%"));
                //}
                //else
                //{
                //    query = procurementDBContext.sap_users
                //        .Where(u => EF.Functions.Like(u.fname, $"%{searchTerm}%"));
                //}

                //var sapUsersResult = await query.OrderBy(u => u.id).Take(10).ToListAsync();

                //if (sapUsersResult.Any())
                //{
                //    return Ok(sapUsersResult);
                //}

                if (Regex.IsMatch(searchTerm, @"^\d{15}$"))
                {
                    user_verifyed = procurementDBContext.user_header
                                                .Where(u => EF.Functions.Like(u.tax_id, $"%{searchTerm}%"))
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
                                                .Where(u => EF.Functions.Like(u.company, $"%{searchTerm}%"))
                                                .Where(h => h.verifyied == true);
                }

                var userHeaderResult = await user_verifyed
                                    .OrderBy(u => u.id)
                                    .Take(10)
                                    .ToListAsync();

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

        [HttpGet]
        public async Task<ActionResult> ReAssignSearch(string searchTerm , int pkg_id , CancellationToken cancellationToken)
        {
            IQueryable<user_header> user_verifyed;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var packageUsers = await procurementDBContext.packages_details
                                            .Where(u => u.pkg_id == pkg_id)
                                            .Select(p => p.user_id)
                                        .ToListAsync(cancellationToken);


                if (Regex.IsMatch(searchTerm, @"^\d{15}$"))
                {
                    user_verifyed = procurementDBContext.user_header
                        .Where(u => EF.Functions.Like(u.tax_id, $"%{searchTerm}%"))
                        .Where(h => h.verifyied == true)
                        .Where(u => !packageUsers.Contains(u.id));
                }
                else if (Regex.IsMatch(searchTerm, @"^\w{3}-\w{3}-\w{3}$"))
                {
                    user_verifyed = procurementDBContext.user_header
                        .Where(u => EF.Functions.Like(u.tax_id, $"%{searchTerm}%"))
                        .Where(h => h.verifyied == true)
                        .Where(u => !packageUsers.Contains(u.id));
                }
                else
                {
                    user_verifyed = procurementDBContext.user_header
                        .Where(u => EF.Functions.Like(u.company, $"%{searchTerm}%"))
                        .Where(h => h.verifyied == true)
                        .Where(u => !packageUsers.Contains(u.id)); 
                }

                var userHeaderResult = await user_verifyed
                                    .OrderBy(u => u.id)
                                    .Take(10)
                                    .ToListAsync(cancellationToken);

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
