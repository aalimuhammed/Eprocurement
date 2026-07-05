using EPROCUREMENT.Enums;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
	public class ViewOnlyController : Controller
    {
        private readonly IGetMaterialGrp _getMaterialGrp;
        private readonly IServiceMaterials _serviceMaterials;
        public ViewOnlyController(
            IGetMaterialGrp getMaterialGrp, 
            IServiceMaterials serviceMaterials)
        {
            _getMaterialGrp = getMaterialGrp;
            _serviceMaterials = serviceMaterials;
        }
		public IActionResult CheckInudstryByMaterialGrp()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> GetMaterialsGrp()
        {
			var materials_grps = await _getMaterialGrp.MaterialsGrp();

			return Ok(materials_grps);
		}

		[HttpGet]
		public async Task<IActionResult> GetIndustry(int mtr_id)
		{
			var industry = await _getMaterialGrp.Get_Industry(mtr_id);
			return Ok(industry);
		}

        [HttpGet]
        public async Task<IActionResult> GetMTROrServiceGrp(int paramValue , CancellationToken cancellationToken)
        {
			if (paramValue == 1)
			{
                var materials = await _getMaterialGrp.GetMaterials(cancellationToken);
				return Ok(materials);
            }

            var services = await _getMaterialGrp.GetServices(cancellationToken);
            return Ok(services);
        }

        [HttpGet]
        public async Task<IActionResult> GetMTROrServices(IndustryType type, CancellationToken cancellationToken)
        {
            var result = await _getMaterialGrp.GetMTROrService(type, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMTRORServiceHeaderName(int header_id, CancellationToken cancellationToken)
        {
            var result = await _serviceMaterials.GetHeaderName(header_id, cancellationToken);
            return Ok(result);
        }
    }
}
