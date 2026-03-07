using EPROCUREMENT.DTO;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace EPROCUREMENT.Controllers
{
	public class BiddingController : Controller
	{
		private readonly IBidding _bidding;
        private readonly IUsersActions _usersActions;
        private readonly IPackageHeader _packageHeader;
        private IWebHostEnvironment _environment;
        public BiddingController(IBidding bidding , 
            IWebHostEnvironment environment, 
            IUsersActions usersActions , 
            IPackageHeader packageHeader)
        {
            _bidding = bidding;
            _environment = environment;
            _usersActions = usersActions;
            _packageHeader = packageHeader;
        }

        public IActionResult ReBidding()
        {
            return View();
        }


        public IActionResult AwardedPackages()
        {
            return View();
        }

        public IActionResult NotAwardedPackages()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAwardedPackages()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var awarded_pkgs = await _bidding.GetAwarded_Packages(userId.Value);
            return Ok(awarded_pkgs);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotAwarded()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var not_awarded_pkgs = await _bidding.GetNotAwarded_Packages(userId.Value);
            return Ok(not_awarded_pkgs);
        }

        [HttpGet]
        public async Task<IActionResult> GetPackagesForRebidding()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var rebidded_packgs = await _bidding.GetPackgesForRebedding(userId.Value);
            return Ok(rebidded_packgs);
        }

        [HttpGet]
        public async Task<IActionResult> GetPackagesRebiddedDetails(int pkg_id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var details = await _bidding.GetPackgesDetailsRebidded(userId.Value , pkg_id);
            return Ok(details);
        }

        [HttpGet]
        public async Task<IActionResult> GetAcceptedOffers(int project_id , int industry_id)
        {
            return Ok(await _bidding.GetAcceptedOffers(project_id , industry_id));
        }

		[HttpGet]
		public async Task<IActionResult> GetAcceptedOffersDetails(int pkg_id)
		{
			return Ok(await _bidding.AcceptedoffersDetails(pkg_id));
		}

		[HttpPost]
        public async Task<IActionResult> VendorBidding(VendorBiddingDTO vendorBiddingDTO , IFormFile file)
        {
			var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
				vendorBiddingDTO.user_id = userId.Value;

                string filePath = Path.Combine(this._environment.WebRootPath, "offers", file.FileName);
                string relativePath = filePath.Substring(filePath.IndexOf("wwwroot")).Replace('\\', '/');
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                vendorBiddingDTO.filePath = relativePath;

                var result = await _bidding.VendorBidding(vendorBiddingDTO);
                return Ok(result);
			}
            return null;
		}

        [HttpGet]
        public async Task<IActionResult> PriceComparison(int pkg_id)
        {
            var prices_listed = await _bidding.PriceComparison(pkg_id);
            return Ok(prices_listed);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRebidding(VendorRebiddingDTO vendorRebiddingDTO, IFormFile file)
        {
            if (file != null)
            {
                string filePath = Path.Combine(this._environment.WebRootPath, "offers", file.FileName);
                string relativePath = filePath.Substring(filePath.IndexOf("wwwroot")).Replace('\\', '/');
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                vendorRebiddingDTO.filePath = relativePath;
            }

            var result = await _bidding.VendorRebidding(vendorRebiddingDTO);
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> AcceptBidding(int pkg_id , int user_id)
        {
            var result = await _bidding.AcceptBidding(pkg_id, user_id);

            if (result > 0)
            {
                return Ok(await SendAnEmail(user_id , pkg_id));
            }
            return Ok(result);
        }

        private async Task<int> SendAnEmail(int user_id , int pkg_id)
        {
            var userEntity = _usersActions.EmailAndFiles(user_id).Result;
            var packgHeader = _packageHeader.GetPackagesHeaderAsync(pkg_id).Result;

            var builder = new BodyBuilder();
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Acceptance of Offer Mail ", "e-procurement@siac-egypt.com"));
            message.To.Add(new MailboxAddress("", userEntity.email));
            message.Subject = "Confirmation from SIAC E-Procurement";

            builder.TextBody = $"Messrs. {userEntity.FullName}\n\nDear {userEntity.keyperson_name},\n\nThis email has been sent to you from the SIAC E-Procurement Platform to inform you that your submitted Quotation was accepted. Accordingly, you have been awarded:\n\nThe Package: {packgHeader.PackageName}\nof the Works: {packgHeader.Industry_Name}\nin Project: {packgHeader.ProjectName}\n\nBest Regards,\n\nSIAC E-Procurement";

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.office365.com", 587, false);
                client.Authenticate("e-procurement@siac-egypt.com", "abdoali123Ali@#");

                await client.SendAsync(message);
                client.Disconnect(true);
                return 1;
            }
        }
    }

}
