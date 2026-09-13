using EPROCUREMENT.DTO;
using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using EPROCUREMENT.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class VendorLoginController : Controller
    {
        private readonly ProcurementDBContext procurementDBContext;
        public VendorLoginController(ProcurementDBContext procurementDBContext)
        {
            this.procurementDBContext = procurementDBContext;
        }
        public IActionResult Index()
        {
			string errorMessage = TempData["ErrorMessage"] as string;

			// Pass error message to the view
			ViewBag.ErrorMessage = errorMessage;

            string successMessage = TempData["SuccessMsgReset"] as string;

            // Pass error message to the view
            ViewBag.successMessagereset = successMessage;

            return View();
        }
        public IActionResult ResetPassword()
        {
            string errorMessage = TempData["ErrorMessage"] as string;

            // Pass error message to the view
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }

        public IActionResult CheckCode()
        {
            string errorMessage = TempData["ErrorMessageCode"] as string;

            // Pass error message to the view
            ViewBag.ErrorMessageCode = errorMessage;

            string successMessage = TempData["SuccessMsgCode"] as string;

            // Pass success message to the view
            ViewBag.SuccessMsgCode = successMessage;

            return View();
        }

        public IActionResult VendorChangePassword()
        {
            return View();
        }

        public async Task<IActionResult> ExternalVendorProfile()
        {
			var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "VendorLogin");
            }
            string errorMessage = TempData["ErrorMessageCode"] as string;

            ViewBag.ErrorMessageCode = errorMessage;

            var vendorData = await procurementDBContext.user_header
							   .Where(x => x.id == userId.Value)
							   // x.verifyied == true)
							   .Select(x => new VendorData
							   {
								   Name = x.fname,
								   Address = x.address,
                                   uploaded_data = x.uploaded_data,
								   Email = x.email,
                                   Fax = x.fax,
                                   Telephone = x.phone , 
                                   SalesPersonEmail = x.SalesPersonEmail,
                                   refused = x.refused,
                                   verifyed = x.verifyied,
                                   area_id = x.areas_id,
                                   money = x.moneybudget,
                                   equipment = x.equipment,
                                   engineer_num = x.engineersno , 
                                   iso = x.iso_verifyed, 
                                   tax_file = x.taxid_document_file_path , 
                                   commercial_file = x.commercial_register_document_file_path,
                                   idcard = x.idcard_document,
                                   prev_work_file = x.prev_work_document_file_path,
                                   electronic_invoice_file = x.electronic_invoice_document,
								   income_tax_file = x.income_tax_document,
								   // Vendor = x.sap_code,
								   id = x.id
							   }).FirstOrDefaultAsync();


                string taxid_document_file_path = vendorData.tax_file;
                string delimiter = "userprofile/tax/";

                // Find the index of the last occurrence of the delimiter
                int lastIndex = taxid_document_file_path.LastIndexOf(delimiter);

                // If the delimiter is found, extract the substring after it
                string Taxresult = lastIndex >= 0 ? taxid_document_file_path.Substring(lastIndex + delimiter.Length) : taxid_document_file_path;
                vendorData.tax_file = Taxresult;


                string prevwork_document_file_path = vendorData.prev_work_file;
                string prevworkdelimiter = "userprofile/prevwork/";

                // Find the index of the last occurrence of the delimiter
                int prevworklastIndex = prevwork_document_file_path.LastIndexOf(prevworkdelimiter);

                // If the delimiter is found, extract the substring after it
                string Prevresult = lastIndex >= 0 ? prevwork_document_file_path.Substring(prevworklastIndex + prevworkdelimiter.Length) : prevwork_document_file_path;

                vendorData.prev_work_file = Prevresult;


                if (vendorData.electronic_invoice_file != null)
                {
                    string electroice_document_file_path = vendorData.electronic_invoice_file;
                    string electronicdelimiter = "userprofile/electorinc_invoice/";

                    // Find the index of the last occurrence of the delimiter
                    int electroniclastIndex = electroice_document_file_path.LastIndexOf(electronicdelimiter);

                    // If the delimiter is found, extract the substring after it
                    string Electronicresult = electroniclastIndex >= 0 ? electroice_document_file_path.Substring(electroniclastIndex + electronicdelimiter.Length) : electroice_document_file_path;

                    vendorData.electronic_invoice_file = Electronicresult;
                }

                if (vendorData.commercial_file != null)
                {
                    string commercial_document_file_path = vendorData.commercial_file;
                    string commercialdelimiter = "userprofile/commercial/";

                    // Find the index of the last occurrence of the delimiter
                    int commerciallastIndex = commercial_document_file_path.LastIndexOf(commercialdelimiter);

                    // If the delimiter is found, extract the substring after it
                    string commercialresult = commerciallastIndex >= 0 ? commercial_document_file_path.Substring(commerciallastIndex + commercialdelimiter.Length) : commercial_document_file_path;

                    vendorData.commercial_file = commercialresult;
                }

                if (vendorData.income_tax_file != null)
                {

                    string income_tax_document_file_path = vendorData.income_tax_file;
                    string income_taxldelimiter = "userprofile/income_tax/";

                    // Find the index of the last occurrence of the delimiter
                    int income_taxlastIndex = income_tax_document_file_path.LastIndexOf(income_taxldelimiter);

                    // If the delimiter is found, extract the substring after it
                    string income_taxresult = income_taxlastIndex >= 0 ? income_tax_document_file_path.Substring(income_taxlastIndex + income_taxldelimiter.Length) : income_tax_document_file_path;

                    vendorData.income_tax_file = income_taxresult;

                }



                if (vendorData.refused == true)
                {
                    TempData["RefusedVendorData"] = JsonConvert.SerializeObject(vendorData);
                    return RedirectToAction("RefusedVendorProfile", "ExternalUser");
                }

                else
                {
                    if (vendorData.area_id == 3)
                    {
                        // return to ExternalUserKSA with passing vendorData Object
                        return RedirectToAction("ExternalUserKSA", vendorData);
                    }
                    return View(vendorData);
                }
        }

        public IActionResult ExternalUserKSA(VendorData vendorData)
        {
			if (vendorData.idcard != null)
			{

				string idcard_tax_document_file_path = vendorData.idcard;
				string idcard_taxldelimiter = "userprofile/idcard/";

				// Find the index of the last occurrence of the delimiter
				int idcard_taxlastIndex = idcard_tax_document_file_path.LastIndexOf(idcard_taxldelimiter);

				// If the delimiter is found, extract the substring after it
				string idcard_taxresult = idcard_taxlastIndex >= 0 ? idcard_tax_document_file_path.Substring(idcard_taxlastIndex + idcard_taxldelimiter.Length) : idcard_tax_document_file_path;

				vendorData.idcard = idcard_taxresult;

			}

			return View(vendorData);
        }


        [HttpPost]
        public async Task<IActionResult> VendorProfile(sap_users loginModel)
        {
            var user = await procurementDBContext.user_header
                              .Where(x => x.email == loginModel.email &&
                                          x.tax_id == loginModel.tax_id)
                              .FirstOrDefaultAsync();

			if (user == null)
			{
				TempData["ErrorMessage"] = "Invalid login attempt.";
				return RedirectToAction("Index");
			}

			bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginModel.password , user.password);
			if (!isPasswordValid)
			{
				TempData["ErrorMessage"] = "Invalid login attempt.";
				return RedirectToAction("Index");
			}

			var vendorData = new VendorData
            {
                Name = user.fname,
                Address = user.address,
                Email = user.email,
                uploaded_data = user.uploaded_data,
                verifyed = user.verifyied,
                id = user.id,
                refused = user.refused
            };


            if (vendorData.uploaded_data && !vendorData.refused && !vendorData.verifyed)
            {
                return Redirect("https://eproc.siac-construction.com/confirmed");
            }

            if (vendorData.uploaded_data == false)
            {
                return Redirect("https://eproc.siac-construction.com/login");
            }

            HttpContext.Session.SetInt32("UserId", vendorData.id);
            TempData["userId"] = vendorData.id;

            return vendorData.refused ?
                RedirectToAction("RefusedVendors", "ExternalUser") :
                RedirectToAction("VendorPackages", "ExternalUser");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateVendor(
            VendorData vendorData , 
            IFormFile tax_file , 
            IFormFile prev_work_file , 
            IFormFile commercial_file , 
            IFormFile electronic_invoice_file , 
            IFormFile income_tax_file , 
            IFormFile idcard)
        {
            //string sap_code = TempData["sap_code"].ToString();

            //var (csrfToken, sessionIdValue) = await SapApi.GetcsrfToken();

            //var updated =  await SapApi.UpdateVendor(csrfToken, sessionIdValue, sap_code , vendorData);

            // return Ok(updated);

            var userId = HttpContext.Session.GetInt32("UserId");


            var vendor = await procurementDBContext.user_header
                     .AsNoTracking()
                     .FirstOrDefaultAsync(x => x.id == userId.Value);

            if (tax_file != null)
            {
                //live
                string directoryPath = "/home/siacdev/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public/userprofile/tax";

                //staging
                //string directoryPath = "/home/siacdevelop/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public/userprofile/tax";


                // Ensure the directory exists, if not create it
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Combine the directory path with the file name
                string filePath = Path.Combine(directoryPath, tax_file.FileName);

                // Create a new file stream where you'll save the uploaded file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    // Copy the contents of the uploaded file to the new file stream
                    await tax_file.CopyToAsync(stream);
                }
                vendor.taxid_document_file_path = "userprofile/tax/" + tax_file.FileName;
            }

            if (prev_work_file != null)
            {
                //live-directory
                 string directoryPath = "/home/siacdev/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public/userprofile/prevwork";

                //staging-directory
               // string directoryPath = "/home/siacdevelop/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public/userprofile/prevwork";
                // Ensure the directory exists, if not create it
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Combine the directory path with the file name
                string filePath = Path.Combine(directoryPath, prev_work_file.FileName);

                // Create a new file stream where you'll save the uploaded file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    // Copy the contents of the uploaded file to the new file stream
                    await prev_work_file.CopyToAsync(stream);
                }
                vendor.prev_work_document_file_path = "userprofile/prevwork/" + prev_work_file.FileName;
            }

            if (commercial_file != null)
            {
                // live directory
                 string directoryPath = "/home/siacdev/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public/userprofile/commercial";

                //staging
                //string directoryPath = "/home/siacdevelop/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public/userprofile/commercial";

                // Ensure the directory exists, if not create it
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Combine the directory path with the file name
                string filePath = Path.Combine(directoryPath, commercial_file.FileName);

                // Create a new file stream where you'll save the uploaded file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    // Copy the contents of the uploaded file to the new file stream
                    await commercial_file.CopyToAsync(stream);
                }
                vendor.commercial_register_document_file_path = "userprofile/commercial/" + commercial_file.FileName;
            }


            if (electronic_invoice_file != null)
            {
                // live
                string directoryPath = "/home/siacdev/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public/userprofile/electorinc_invoice";

                //staging 
                //string directoryPath = "/home/siacdevelop/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public/userprofile/electorinc_invoice";

                // Ensure the directory exists, if not create it
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Combine the directory path with the file name
                string filePath = Path.Combine(directoryPath, electronic_invoice_file.FileName);

                // Create a new file stream where you'll save the uploaded file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    // Copy the contents of the uploaded file to the new file stream
                    await electronic_invoice_file.CopyToAsync(stream);
                }
                vendor.electronic_invoice_document = "userprofile/electorinc_invoice/" + electronic_invoice_file.FileName;
            }

            if (income_tax_file != null)
            {
                // live
                string directoryPath = "/home/siacdev/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public/userprofile/income_tax";

                // staging
                //string directoryPath = "/home/siacdevelop/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public/userprofile/income_tax";

                // Ensure the directory exists, if not create it
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Combine the directory path with the file name
                string filePath = Path.Combine(directoryPath, income_tax_file.FileName);

                // Create a new file stream where you'll save the uploaded file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    // Copy the contents of the uploaded file to the new file stream
                    await income_tax_file.CopyToAsync(stream);
                }
                vendor.income_tax_document = "userprofile/income_tax/" + income_tax_file.FileName;
            }

            if (idcard != null)
            {
                //live
                 string directoryPath = "/home/siacdev/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public/userprofile/idcard";

                //staging
               // string directoryPath = "/home/siacdevelop/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public/userprofile/idcard";

                // Ensure the directory exists, if not create it
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Combine the directory path with the file name
                string filePath = Path.Combine(directoryPath, idcard.FileName);

                // Create a new file stream where you'll save the uploaded file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    // Copy the contents of the uploaded file to the new file stream
                    await idcard.CopyToAsync(stream);
                }
                vendor.idcard_document = "userprofile/idcard/" + idcard.FileName;
            }


            vendor.fname = vendorData.Name;
            vendor.address = vendorData.Address;
            vendor.email = vendorData.Email;
            vendor.SalesPersonEmail = vendorData.SalesPersonEmail;
            vendor.phone = vendorData.Telephone;
            vendor.fax = vendorData.Fax;
            vendor.engineersno = vendorData.engineer_num;

            procurementDBContext.Attach(vendor);
            procurementDBContext.Entry(vendor).State = EntityState.Modified;

            // Save changes to the database
            await procurementDBContext.SaveChangesAsync();

            if (vendorData.selectedIndustry != null)
            {


                var vendor_details = await procurementDBContext.user_detail
                                    .Where(x => x.user_id == userId.Value)
                                    .ToListAsync();

                var type = vendor_details[0].type_id;
               
                procurementDBContext.user_detail.RemoveRange(vendor_details);
                await procurementDBContext.SaveChangesAsync();

                string result = string.Join("", vendorData.selectedIndustry);

                if (result.Contains(','))
                {
                    string[] values = result.Split(',');

                    foreach (var industry in values)
                    {
                        var userdetails = new user_detail();
                        userdetails.user_id = userId.Value;
                        userdetails.type_id = type;
                        userdetails.industries_details = int.Parse(industry);
                        procurementDBContext.Entry(userdetails).State = EntityState.Added;

                        await procurementDBContext.SaveChangesAsync();
                    }

                }
                else
                {
                        foreach (var industry in vendorData.selectedIndustry)
                        {

                            var userdetails = new user_detail();
                            userdetails.user_id = userId.Value;
                            userdetails.type_id = type;
                            userdetails.industries_details = int.Parse(industry);

                            procurementDBContext.Entry(userdetails).State = EntityState.Added;

                            await procurementDBContext.SaveChangesAsync();
                        }
                 
                }
            }


            //   <a href="@Url.Action("ExternalVendorProfile" , "VendorLogin")" class="dropdown-item">Profile</a>
            return RedirectToAction("ExternalVendorProfile", "VendorLogin");
        }


        public async Task<IActionResult> VendorResetPassword()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var vendorData = await procurementDBContext.user_header
                           .Where(x => x.id == userId.Value)
                           .Select(x => new ResetPasswordViewModel
						   {
                               Password = x.password,
                               Id = x.id
                           }).FirstOrDefaultAsync();

            return View(vendorData);
        }

		[HttpPost]
		public async Task<IActionResult> UpdateVendorPassword (ResetPasswordViewModel resetPasswordDTO)
        {
             var user_model = await procurementDBContext.user_header
                .Where(x => x.id == resetPasswordDTO.Id)
                .FirstOrDefaultAsync();

			var hashedPassword = BCrypt.Net.BCrypt.HashPassword(resetPasswordDTO.Password);

			// Update the user's password with the hashed value
			user_model.password = hashedPassword;

			procurementDBContext.Attach(user_model);
			procurementDBContext.Entry(user_model).State = EntityState.Modified;

            var save = await procurementDBContext.SaveChangesAsync();

            if (save > 0)
            {
                return RedirectToAction("VendorResetPassword");
            }
            else
            {
                return BadRequest();
            }

		}

        [HttpPost]
        public async Task<IActionResult> ResetPasswordAction(string newpassword)
        {
            if (HttpContext.Session.GetInt32("resetuserid") != null)
            {
                var userId = HttpContext.Session.GetInt32("resetuserid").Value;

                var user_model = await procurementDBContext.user_header
                    .Where(x => x.id == userId)
                    .FirstOrDefaultAsync();

				var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newpassword);

				// Update the user's password with the hashed value
				user_model.password = hashedPassword;

                procurementDBContext.Attach(user_model);
                procurementDBContext.Entry(user_model).State = EntityState.Modified;

                var save = await procurementDBContext.SaveChangesAsync();
                if (save > 0)
                {
                    TempData["SuccessMsgReset"] = "The password has been reset. You can now log in with your new password";
                    return RedirectToAction("Index");
                }
            }
            return Unauthorized();
          
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailResetPasswod(ResetPasswordVendorDTO resetPasswordVendorDTO)
        {
            var user_id = await procurementDBContext.user_header
                                                     .Where(x => x.email == resetPasswordVendorDTO.Email && 
                                                          x.tax_id == resetPasswordVendorDTO.TaxId)
                                                     .Select(z => z.id)
                                                     .FirstOrDefaultAsync();

            if (user_id ==  0)
            {
                TempData["ErrorMessage"] = "Your Data doesnt exist";
                return RedirectToAction("ResetPassword");
            }

            var guid = Guid.NewGuid().ToString("N").ToUpper();

            //Save to Database First
            reset_password reset_Password = new reset_password();

            reset_Password.user_id = user_id;
            reset_Password.code = guid;

            procurementDBContext.Attach(reset_Password);

            procurementDBContext.Entry(reset_Password).State = EntityState.Added;

            var save = await procurementDBContext.SaveChangesAsync();

            if (save > 0)
            {
                SendingMail(resetPasswordVendorDTO.Email, guid);
                TempData["SuccessMsgCode"] = "The Code has been sent to your email.";
                return RedirectToAction("CheckCode");
            }
            return BadRequest();

        }

        [HttpPost]
        public async Task<IActionResult> CheckCodeForVendor(string code)
        {
            var user_id = await procurementDBContext.reset_password.
                                                     Where(x => x.code.Contains(code)).Select(z => z.user_id).FirstOrDefaultAsync();

            if (user_id > 0)
            {
                HttpContext.Session.SetInt32("resetuserid", user_id);
                return RedirectToAction("VendorChangePassword");
            }
            TempData["ErrorMessageCode"] = "You Entered invalid Code";
            return RedirectToAction("CheckCode");

        }

        private void SendingMail(string email , string code)
        {
                var builder = new BodyBuilder();
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Reset Password Mail ", "it-solutions@siac-construction.com"));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Reset Password from SIAC E-Procurement";

            builder.TextBody = "Here is the code to reset your password:\r\n" + code + "\r\n\r\n";
            builder.TextBody += "Please click on the following link to reset your password:\r\n";
            builder.TextBody += "https://eproc.siac-construction.com:9443/VendorLogin/CheckCode\r\n";


            message.Body = builder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.office365.com", 587, false);
                    client.Authenticate("it-solutions@siac-construction.com", "M&584666642409anTb12");

                    client.Send(message);
                    client.Disconnect(true);
                }
            }
        }
    }
