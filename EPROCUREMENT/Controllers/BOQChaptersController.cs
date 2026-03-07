using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class BOQChaptersController : Controller
    {
        private readonly IGetBOQChapters getBOQChapters;

        public BOQChaptersController(IGetBOQChapters getBOQChapters)
        {
            this.getBOQChapters = getBOQChapters;
        }

        public async Task<IActionResult> GetBOQChapters(int id)
        {
            var results = await getBOQChapters.loadboqchapters(id);
            return Ok(results);
        }
    }
}
