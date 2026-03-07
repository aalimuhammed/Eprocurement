using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class ServicesMaterialController : Controller
    {
        private readonly IServiceMaterials _serviceMaterials;

        public ServicesMaterialController(IServiceMaterials serviceMaterials)
        {
            _serviceMaterials = serviceMaterials;
        }

        [HttpGet]
        public async Task<IActionResult> GetMaterialsServices(int industry_id)
        {
            var result = await _serviceMaterials.GetServiceMaterials(industry_id);
            return Ok(result);
        }

    }
}
