using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using EPROCUREMENT.DTO;
using Microsoft.AspNetCore.Hosting;

namespace EPROCUREMENT.Controllers
{
    public class ImportExcelController : Controller
    {
        private readonly IExcelDataImporter _excelDataImporter;
		private IWebHostEnvironment _environment;

		public ImportExcelController(IExcelDataImporter excelDataImporter , IWebHostEnvironment webHostEnvironment)
        {
            _excelDataImporter = excelDataImporter;
            _environment = webHostEnvironment;
        }
        public IActionResult ImportExcelManual ()
        {
            return View();
        }

        public IActionResult GetImportedManual()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcelProcess(IFormFile file , ManualPackageDTO manualPackageDTO)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file.");
                return View("ImportExcelManual");
            }

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("File", "Please upload an Excel file.");
                return View("ImportExcelManual");
            }

            try
            {
                // Save the uploaded file to a temporary location
                var filePath = Path.GetTempFileName();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Perform bulk insertion
                bool success = await _excelDataImporter.BulkInsertFromExcel(filePath , manualPackageDTO);
                //if (!success)
                //{
                //    //ModelState.AddModelError("File", "Failed to process the Excel file.");
                //    //return RedirectToAction("ImportExcelManual");

                //    return Ok(success);
                //}

                //return RedirectToAction("ImportExcelManual");

                return Ok(success);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("File", $"An error occurred: {ex.Message}");
                return RedirectToAction("ImportExcelManual");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetImportedExcel(int rowId)
        {
            var manual_package_list = await _excelDataImporter.GetImportedManualPackages(rowId);
            return Ok(manual_package_list);
        }

        [HttpGet]
        public async Task<IActionResult> GetImportedPkgs(ManualPackageDTO manualPackageDTO)
        {
            var pkgImported = await _excelDataImporter.GetImportedPkgs(manualPackageDTO);
            return Ok(pkgImported);
		}

        [HttpPost]
        public async Task<IActionResult> AssignManualUsers(AssignManualDTO assignManualDTO , IFormFile file) 
        {
			if (file != null && file.Length > 0)
			{
				string filePath = Path.Combine(this._environment.WebRootPath, "packages", file.FileName);
				string relativePath = filePath.Substring(filePath.IndexOf("wwwroot")).Replace('\\', '/');
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}
				assignManualDTO.filepath = relativePath;
			}
			return Ok(await _excelDataImporter.AssignManualPackageForUsers(assignManualDTO));
        }

        [HttpPost]
        public async Task<IActionResult> AcceptPackageManaual(AcceptPackageManualDTO acceptPackageManualDTO , IFormFile file)
        {
			if (file != null && file.Length > 0)
			{
				string filePath = Path.Combine(this._environment.WebRootPath, "packages", file.FileName);
				string relativePath = filePath.Substring(filePath.IndexOf("wwwroot")).Replace('\\', '/');
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}
				acceptPackageManualDTO.filepath = relativePath;
			}

			return Ok(await _excelDataImporter.AcceptPackageManual(acceptPackageManualDTO));
		}
    }
}
