using EPROCUREMENT.Services.Helper;
using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.ViewModel;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class UserController : Controller
    {
        private readonly IUsersActions usersActions;
        public UserController(IUsersActions usersActions)
        {
            this.usersActions = usersActions;
        }
        public IActionResult UsersList()
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            if (userId == null)
            {
                return RedirectToAction("Login" , "Admin");
            }
            var user_list = usersActions.AppliedUsers();
            return View(user_list);
        }

        public async Task<IActionResult> ExistedUsers(
            int page = 1 , 
            int pageSize = 10, 
            bool isSapVendorChecked = false, 
            bool isEProcurementVendorChecked = false,
            CancellationToken cancellationToken = default)
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            var count = await usersActions.CountVendors(isSapVendorChecked , isEProcurementVendorChecked);
            var sapUsers =  await usersActions.ExistedUsers(page , pageSize , isSapVendorChecked , isEProcurementVendorChecked);

            var model = sapUsers.AsEnumerable().ToList();

            var values = new PaginatedList<sap_users_vm>(model, count, page, pageSize);

            var sapVendorsCount = await usersActions.GetSapVendorCount(cancellationToken);
            var eProcurementVendorsCount = await usersActions.GetVendorCount(cancellationToken);

            ViewBag.SapVendorsCount = sapVendorsCount;
            ViewBag.EProcurementVendorsCount = eProcurementVendorsCount;

            return View(values);
        }

        public async Task<IActionResult> ExistedUserFilter(
            int page = 1, 
            int pageSize = 10, 
            bool isSapVendorChecked = false, 
            bool isEProcurementVendorChecked = false)
        {
            var userId = HttpContext.Session.GetInt32("AdminId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            var count = await usersActions.CountVendors(isSapVendorChecked , isEProcurementVendorChecked);
            var sapUsers = await usersActions.ExistedUsers(page, pageSize, isSapVendorChecked, isEProcurementVendorChecked);

            var model = sapUsers.AsEnumerable().ToList();

            var values = new PaginatedList<sap_users_vm>(model, count, page, pageSize);

            return Json(values);
        }
        public async Task<IActionResult> CheckBoxFilters(
            int page = 1, 
            int pageSize = 10, 
            bool isSapVendorChecked = false, 
            bool isEProcurementVendorChecked = false)
        {
            var sapUsers = await usersActions.ExistedUsers(page, pageSize, isSapVendorChecked, isEProcurementVendorChecked);
            var model = sapUsers.AsEnumerable().ToList();

            var values = new PaginatedList<sap_users_vm>(model, 7000, page, pageSize);
            return Json(values);
        }
        public async Task<IActionResult> SearchFilter(string search , bool isSapVendorChecked , bool isEProcurementVendorChecked)
        {
            var users = await usersActions.SearchFilter(search , isSapVendorChecked , isEProcurementVendorChecked);

            return Json(users);
        }

        public IActionResult UsersTypes(int id)
        {
            var user_types = usersActions.UserTypes(id);
			return PartialView("_UserTypesPartial", user_types);
		}

		public IActionResult UsersIndustry(int id)
		{
			var user_types = usersActions.UserIndustry(id);
			return PartialView("_UserIndustryPartial", user_types);
		}

		public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            // string file_path = "D:/EProcurement/EPROCUREMENTMANAGEMENT/storage/app/public/";
            //live
             string file_path = "/home/siacdev/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public";

            //staging
           // string file_path = "/home/siacdevelop/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public";
            string input = null;
            string Taxresult = null;
            string commercialresult = null;
            string prevworkresult = null;
            string idcardresult = null;
            string electronic_invoice = null;
            string incomeresult = null;

			string path = null;
            //var userId = HttpContext.Session.GetInt32("UserId");
           
            var file = usersActions.DownloadFileViewModels(filename);
            foreach (var item in file)
            {
                input = item.FilePath;
            }

            if (input.Contains("tax/"))
            {
                string searchString = "tax/";

                int index = input.IndexOf(searchString);
                if (index != -1)
                {
                    Taxresult = input.Substring(0, index + searchString.Length);
                }

                path = Path.Combine(
                               file_path,
                               Taxresult, filename);
            }
            if (input.Contains("commercial/"))
            {
                string searchString = "commercial/";

                int index = input.IndexOf(searchString);
                if (index != -1)
                {
                    commercialresult = input.Substring(0, index + searchString.Length);
                }

                path = Path.Combine(
                               file_path,
                               commercialresult, filename);
            }

			if (input.Contains("prevwork/"))
			{
				string searchString = "prevwork/";

				int index = input.IndexOf(searchString);
				if (index != -1)
				{
					prevworkresult = input.Substring(0, index + searchString.Length);
				}

				path = Path.Combine(
							   file_path,
							   prevworkresult, filename);
			}

			if (input.Contains("electorinc_invoice/"))
			{
				string searchString = "electorinc_invoice/";

				int index = input.IndexOf(searchString);
				if (index != -1)
				{
					electronic_invoice = input.Substring(0, index + searchString.Length);
				}

				path = Path.Combine(
							   file_path,
							   electronic_invoice, filename);
			}

			if (input.Contains("income_tax/"))
			{
				string searchString = "income_tax/";

				int index = input.IndexOf(searchString);
				if (index != -1)
				{
					incomeresult = input.Substring(0, index + searchString.Length);
				}

				path = Path.Combine(
							   file_path,
							   incomeresult, filename);
			}

			if (input.Contains("idcard/"))
			{
				string searchString = "idcard/";

				int index = input.IndexOf(searchString);
				if (index != -1)
				{
					idcardresult = input.Substring(0, index + searchString.Length);
				}

				path = Path.Combine(
							   file_path,
							   idcardresult, filename);
			}

            if (input.Contains("iso_file/"))
            {
                string searchString = "iso_file/";

                int index = input.IndexOf(searchString);
                if (index != -1)
                {
                    idcardresult = input.Substring(0, index + searchString.Length);
                }

                path = Path.Combine(
                               file_path,
                               idcardresult, filename);
            }

            var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            
        }

        public async Task<IActionResult> DownloadVendorFiles(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            // string file_path = "D:/EProcurement/EPROCUREMENTMANAGEMENT/storage/app/public/";
            //live
            string file_path = "/home/siacdev/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public";

            //staging
            //string file_path = "/home/siacdevelop/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public";

            string input = null;
            string Taxresult = null;
            string commercialresult = null;
            string prevworkresult = null;
            string idcardresult = null;
            string electronic_invoice = null;
            string incomeresult = null;

            string path = null;
            var userId = HttpContext.Session.GetInt32("UserId");
            if (true)
            {

            }
            var file = usersActions.DownloadFileViewModels(filename);
            foreach (var item in file)
            {
                input = item.FilePath;
            }

            if (input.Contains("tax/"))
            {
                string searchString = "tax/";

                int index = input.IndexOf(searchString);
                if (index != -1)
                {
                    Taxresult = input.Substring(0, index + searchString.Length);
                }

                path = Path.Combine(
                               file_path,
                               Taxresult, filename);
            }
            if (input.Contains("commercial/"))
            {
                string searchString = "commercial/";

                int index = input.IndexOf(searchString);
                if (index != -1)
                {
                    commercialresult = input.Substring(0, index + searchString.Length);
                }

                path = Path.Combine(
                               file_path,
                               commercialresult, filename);
            }

            if (input.Contains("prevwork/"))
            {
                string searchString = "prevwork/";

                int index = input.IndexOf(searchString);
                if (index != -1)
                {
                    prevworkresult = input.Substring(0, index + searchString.Length);
                }

                path = Path.Combine(
                               file_path,
                               prevworkresult, filename);
            }

            if (input.Contains("electorinc_invoice/"))
            {
                string searchString = "electorinc_invoice/";

                int index = input.IndexOf(searchString);
                if (index != -1)
                {
                    electronic_invoice = input.Substring(0, index + searchString.Length);
                }

                path = Path.Combine(
                               file_path,
                               electronic_invoice, filename);
            }

            if (input.Contains("income_tax/"))
            {
                string searchString = "income_tax/";

                int index = input.IndexOf(searchString);
                if (index != -1)
                {
                    incomeresult = input.Substring(0, index + searchString.Length);
                }

                path = Path.Combine(
                               file_path,
                               incomeresult, filename);
            }

            if (input.Contains("idcard/"))
            {
                string searchString = "idcard/";

                int index = input.IndexOf(searchString);
                if (index != -1)
                {
                    idcardresult = input.Substring(0, index + searchString.Length);
                }

                path = Path.Combine(
                               file_path,
                               idcardresult, filename);
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));

        }

        public async Task<IActionResult> RefuseEmail(int id , string rejectionMsg)
        {
            var userEntity = usersActions.EmailAndFiles(id).Result;
			var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Rejection Mail", "it-solutions@siac-construction.com"));
            message.To.Add(new MailboxAddress("", userEntity.email));
            message.Subject = "Rejection from SIAC E-Procurement";

            message.Body = new TextPart("plain")
            {
                Text = $"Messrs. {userEntity.FullName}\n\nDear {userEntity.keyperson_name},\n\nThis email has been sent to you from the SIAC E-Procurement Platform to notify you that your submitted application was not accepted, due to the following reasons:\n\n{rejectionMsg}\n\nPlease address the above reasons and resubmit again.\n\nBest Regards,\n\nSIAC E-Procurement",
            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.office365.com", 587, false);
                client.Authenticate("it-solutions@siac-construction.com", "M&584666642409anTb12");

                client.Send(message);
                
                client.Disconnect(true);
            }
			var refused_user = await usersActions.Refused(id);

			if (refused_user == true)
			{
				TempData["message"] = "Rejection Mail has been sent!";
				return RedirectToAction("UsersList", "User");
			}
			return RedirectToAction("UsersList", "User");
		}

        // For SIAC ..
        public async Task<IActionResult> AcceptEmail(int id , CancellationToken cancellationToken)
        {
            string file_path = "/home/siacdev/Downloads/EPROCUREMENTMANAGEMENT/storage/app/public/userprofile/";
            string extractedTax = null;
            string extractedCommercial = null;

            var userEntity = await usersActions.EmailAndFiles(id);

            var tax_file = Path.Combine(file_path, userEntity.taxid_document_file_path);
            var commercial_file = Path.Combine(file_path, userEntity.commercial_register_document_file_path);
           //    var username = email_to_accept.username;

            string searchStringTax = "tax/";
            int indexTax = tax_file.IndexOf(searchStringTax);

            if (indexTax != -1)
            {
                extractedTax = Path.GetFileName(tax_file);
            }

            string searchStringCommercial = "commercial/";
            int indexCommercial = commercial_file.IndexOf(searchStringCommercial);

            if (indexCommercial != -1)
            {
                extractedCommercial = Path.GetFileName(commercial_file);
            }

            //var vendorDeepInsert = await usersActions.GetUsersForDeepInsertion(id, cancellationToken);
            //var result = await SapApi.CreateVendorDeepInsertAsync(vendorDeepInsert);
            //if(result.StatusCode == System.Net.HttpStatusCode.NotFound)
            //{
            //    TempData["message"] = "The Vendor not exist in the SAP so cannot accept the vendor";

            //    return RedirectToAction("UsersList", "User");
            //}

            //var extractedTaxPath = Path.Combine(file_path, "tax", extractedTax);
            //var extractedCommercialPath = Path.Combine(file_path, "commercial", extractedCommercial);

            var builder = new BodyBuilder();
			var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Acceptance Mail ", "it-solutions@siac-construction.com"));
            message.To.Add(new MailboxAddress("", userEntity.email));
            message.Subject = "Confirmation from SIAC E-Procurement";

            builder.TextBody = $"Dear Mr./Miss {userEntity.FullName}\n\nDear {userEntity.keyperson_name},\n\nThis email has been sent to you from the SIAC E-Procurement Platform to notify you that your submitted application has been accepted. Accordingly, you have been registered in SIAC’s Vendors Database.\n\nBest Regards,\n\nSIAC E-Procurement";
            
			//builder.Attachments.Add(extractedTaxPath);
             //builder.Attachments.Add(extractedCommercialPath);

            message.Body = builder.ToMessageBody();

			using (var client = new SmtpClient())
            {
                client.Connect("smtp.office365.com", 587, false);
                client.Authenticate("it-solutions@siac-construction.com", "M&584666642409anTb12");

                client.Send(message);
                client.Disconnect(true);
            }


            var verify_user = await usersActions.Acceptance(id);

            if (verify_user == true)
            {
				TempData["message"] = "Accept Mail has been sent!";

                //SendToFinance(extractedTaxPath , extractedCommercialPath);
                return RedirectToAction("UsersList", "User");
			}
			return RedirectToAction("UsersList", "User");

		}
        private void SendToFinance(string tax_file, string commercial_file)
        {

            var builder = new BodyBuilder();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Acceptance Mail ", "procerp@outlook.com"));
            message.To.Add(new MailboxAddress("", "siac.finance.dept@gmail.com"));
            message.Subject = "Confirmation from SIAC EProcurement";
            builder.TextBody = "Hello FI Team, Please find attached files and Register on SAP . Thanks";

            builder.Attachments.Add(tax_file);
            builder.Attachments.Add(commercial_file);

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.office365.com", 587, false);
                client.Authenticate("procerp@outlook.com", "Pr0cErp@2023");

                client.Send(message);
                client.Disconnect(true);
            }

        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
