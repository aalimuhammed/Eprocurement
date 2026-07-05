using EPROCUREMENT.DTO;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
	public class CreatePackageController : Controller
	{
		private readonly ICreatePackage _createPackage;
		private IWebHostEnvironment _environment;

		public CreatePackageController(ICreatePackage createPackage , IWebHostEnvironment hostingEnvironment)
        {
            _createPackage = createPackage;
			_environment = hostingEnvironment;
		}


		[HttpPost]
		public async Task<IActionResult> SavePackage(CreatePackageDTO createPackageDTO, IFormFile file)
		{
			//var adminId = HttpContext.Session.GetInt32("AdminId");
			createPackageDTO.assigned_by = 1;
			if (file != null)
			{
				string filePath = Path.Combine(this._environment.WebRootPath, "packages", file.FileName);
				string relativePath = filePath.Substring(filePath.IndexOf("wwwroot")).Replace('\\', '/');
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}
				createPackageDTO.filePath = relativePath;
			}
			var result = await _createPackage.CreateSapPackage(createPackageDTO);
			return Ok(result);
		}

        [HttpPost]
        [RequestFormLimits(ValueCountLimit = 20000)]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> SavePackageManual(
            [FromForm] CreatePackageManualDTO createPackageManualDTO, 
			IFormFile file)
		{
            createPackageManualDTO.assigned_by = 1;

			if (file != null)
			{
				string filePath = Path.Combine(this._environment.WebRootPath, "packages", file.FileName);
				string relativePath = filePath.Substring(filePath.IndexOf("wwwroot")).Replace('\\', '/');
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}

				createPackageManualDTO.filePath = relativePath;
			}

			var result = await _createPackage.CreateSapPackageManual(createPackageManualDTO);
			return Ok(result);
		}

		[HttpPost]
        public async Task<IActionResult> SaveSrvPackage(CreateSrvPackageDTO createSrvPackageDTO, IFormFile file)
        {
            createSrvPackageDTO.assigned_by = 1;
            if (file != null)
            {
                string filePath = Path.Combine(this._environment.WebRootPath, "packages", file.FileName);
                string relativePath = filePath.Substring(filePath.IndexOf("wwwroot")).Replace('\\', '/');
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // Alternatively, you can pass the file data to your service method if needed.
                createSrvPackageDTO.filePath = relativePath;
            }
            var result = await _createPackage.CreateSapSrvPackage(createSrvPackageDTO);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSrvPackageManual(CreateSrvPackageDTOManual createPackageManualDTO, IFormFile file)
        {
            // var adminId = HttpContext.Session.GetInt32("AdminId");
            createPackageManualDTO.assigned_by = 1;

            if (file != null)
            {
                string filePath = Path.Combine(this._environment.WebRootPath, "packages", file.FileName);
                string relativePath = filePath.Substring(filePath.IndexOf("wwwroot")).Replace('\\', '/');
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                createPackageManualDTO.filePath = relativePath;
            }

            var result = await _createPackage.CreateSapSrvPackageManual(createPackageManualDTO);
            return Ok(result);
        }


		[HttpPost]
		public async Task<IActionResult> UpdatePackageManualExcel(CreatePackageManualDTO createPackageManualDTO, IFormFile file)
		{
			var adminId = HttpContext.Session.GetInt32("AdminId");
			createPackageManualDTO.assigned_by = 1;

			if (file != null && file.Length > 0)
			{
				string filePath = Path.Combine(this._environment.WebRootPath, "packages", file.FileName);
				string relativePath = filePath.Substring(filePath.IndexOf("wwwroot")).Replace('\\', '/');
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}
				createPackageManualDTO.filePath = relativePath;
			}

			var result = await _createPackage.CreateSapPackageManual(createPackageManualDTO);
			return Ok(result);
		}

	}
}