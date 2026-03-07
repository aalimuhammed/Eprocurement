using EPROCUREMENT.Models;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class MainController : Controller
    {
        private readonly IMailService mailService;
        public MainController(IMailService mailService)
        {
            this.mailService = mailService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromForm] MailRequest request)
        {
            try
            {
                await mailService.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}
