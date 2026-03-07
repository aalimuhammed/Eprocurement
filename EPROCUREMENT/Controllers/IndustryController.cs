using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class IndustryController : Controller
    {
        private readonly IRetrieveIndustry retrieveIndustry;

        public IndustryController(IRetrieveIndustry retrieveIndustry)
        {
            this.retrieveIndustry = retrieveIndustry;
        }

        public async Task<IActionResult> SapIndustry()
        {
            var sap_Industries = await retrieveIndustry.Sap_Industry();
            return Ok(sap_Industries);
        }
    }
}
