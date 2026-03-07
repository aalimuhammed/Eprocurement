using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class BOQStandsController : Controller
    {
        private readonly IGetBOQ getBOQ;

        public BOQStandsController(IGetBOQ getBOQ)
        {
            this.getBOQ = getBOQ;
        }


        public async Task<IActionResult> boqstands()
        {
            var results = await getBOQ.loadboqstnd();
            return Ok(results);
        }
        
    }
}
