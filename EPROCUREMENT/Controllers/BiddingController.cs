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
        private readonly IGraphEmailService _graphEmailService;
        private IWebHostEnvironment _environment;
        public BiddingController(
            IBidding bidding , 
            IWebHostEnvironment environment, 
            IUsersActions usersActions , 
            IPackageHeader packageHeader,IGraphEmailService graphEmailService)
        {
            _bidding = bidding;
            _environment = environment;
            _usersActions = usersActions;
            _packageHeader = packageHeader;
            _graphEmailService = graphEmailService;
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
            if (userId == null)
            {
                return RedirectToAction("Index", "VendorLogin");
            }
            var awarded_pkgs = await _bidding.GetAwarded_Packages(userId.Value);
            return Ok(awarded_pkgs);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotAwarded()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "VendorLogin");
            }
            var not_awarded_pkgs = await _bidding.GetNotAwarded_Packages(userId.Value);
            return Ok(not_awarded_pkgs);
        }

        [HttpGet]
        public async Task<IActionResult> GetPackagesForRebidding()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index" , "VendorLogin");
            }
            var rebidded_packgs = await _bidding.GetPackgesForRebedding(userId.Value);
            return Ok(rebidded_packgs);
        }

        [HttpGet]
        public async Task<IActionResult> GetPackagesRebiddedDetails(int pkg_id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "VendorLogin");
            }

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

            if (userId == null)
            {
                return RedirectToAction("Index", "VendorLogin");
            }

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

        private async Task<bool> SendAnEmail(int user_id, int pkg_id)
        {
            var userEntity = await _usersActions.EmailAndFiles(user_id);
            var packgHeader = await _packageHeader.GetPackagesHeaderAsync(pkg_id);

            var emailBody = $@"
                    <html>
                    <body>
                        <p>Messrs. {userEntity.FullName}</p>

                        <p>Dear {userEntity.keyperson_name},</p>

                        <p>
                            This email has been sent to you from the SIAC E-Procurement
                            Platform to inform you that your submitted Quotation was accepted.
                        </p>

                        <p>Accordingly, you have been awarded:</p>

                        <p>
                            <strong>The Package:</strong> {packgHeader.PackageName}<br />
                            <strong>of the Works:</strong> {packgHeader.Industry_Name}<br />
                            <strong>in Project:</strong> {packgHeader.ProjectName}
                        </p>

                        <p>
                            Best Regards,<br />
                            SIAC E-Procurement
                        </p>
                    </body>
                    </html>";

            var emailSent = await _graphEmailService.SendEmailAsync(
                userEntity.email,
                "Confirmation from SIAC E-Procurement",
                emailBody
            );

            return emailSent ? true : false;
        }
    }

}
