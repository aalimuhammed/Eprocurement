using EPROCUREMENT.DTO;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
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

        [HttpPost]
        public async Task<IActionResult> AssignNewMaterialGroup(
            [FromBody] AssignNewMaterialGroupDTO assignNewMaterialGroupDTO, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _serviceMaterials.AssignNewMaterialGroup(assignNewMaterialGroupDTO, cancellationToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
