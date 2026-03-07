using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Collections.Generic;
using System.IO;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class BiddingUsersController : Controller
    {
        private readonly IUsersActions usersActions;
        public BiddingUsersController(IUsersActions usersActions)
        {
            this.usersActions = usersActions;
        }
        public IActionResult Users()
        {
            var userlist = usersActions.BiddingUsers();
            return View(userlist);
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

		public async Task<IActionResult> DownloadOffer(string filename)
        {
			var folderName = Path.Combine("Resources", "Offers");
			var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

			var filePath = Path.Combine(pathToSave, filename);

			if (System.IO.File.Exists(filePath))
			{
				var memory = new MemoryStream();
				using (var stream = new FileStream(filePath, FileMode.Open))
				{
					await stream.CopyToAsync(memory);
				}
				memory.Position = 0;
				return File(memory, GetContentType(filename), filename);
			}
			else
			{
				// Handle the case where the file doesn't exist
				return NotFound();
			}

		}


		public IActionResult RefuseEmail(int id , string project_name , string PackageName)
		{
			var email_to_refuse = usersActions.Email(id).Result;
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("Rejection Mail", "procerp@outlook.com"));
			message.To.Add(new MailboxAddress("", email_to_refuse));
			message.Subject = "Rejection from SIAC E-Procurement";

			message.Body = new TextPart("plain")
			{
				Text = "Hello,Your offer for applying this package '"+PackageName+"' with Project '"+project_name+"'  is refused .Thanks",
			};

			using (var client = new SmtpClient())
			{
				client.Connect("smtp.office365.com", 587, false);
				client.Authenticate("procerp@outlook.com", "Pr0cErp@2023");

				client.Send(message);

				client.Disconnect(true);
			}
			var refused_user = usersActions.Refused(id).Result;

			if (refused_user == true)
			{
				TempData["message"] = "Rejection Mail has been sent!";
				return RedirectToAction("ProjectsHome", "ProjectOprations");
			}
			return RedirectToAction("ProjectsHome", "ProjectOprations");
		}


		public IActionResult AcceptEmail(int id, string project_name, string PackageName)
		{
			var email_to_refuse = usersActions.Email(id).Result;
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("Acceptance Mail", "procerp@outlook.com"));
			message.To.Add(new MailboxAddress("", email_to_refuse));
			message.Subject = "Acceptance from SIAC E-Procurement";

			message.Body = new TextPart("plain")
			{
				Text = "Hello,Your offer for applying this package '" + PackageName + "' with Project '" + project_name + "'  is accepted .Thanks",
			};

			using (var client = new SmtpClient())
			{
				client.Connect("smtp.office365.com", 587, false);
				client.Authenticate("procerp@outlook.com", "Pr0cErp@2023");

				client.Send(message);

				client.Disconnect(true);
			}
			var refused_user = usersActions.Refused(id).Result;

			if (refused_user == true)
			{
				TempData["message"] = "Acceptance Mail has been sent!";
				return RedirectToAction("ProjectsHome", "ProjectOprations");
			}
			return RedirectToAction("ProjectsHome", "ProjectOprations");
		}
	}
}
