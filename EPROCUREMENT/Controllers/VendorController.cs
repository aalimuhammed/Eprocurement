using AutoMapper;
using EPROCUREMENT.DTO;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class VendorController : Controller
    {

        private readonly IProjectsList projects;

        private readonly IVendorPackageDetails vendorPackage;

        private readonly IPackageDetails packageDetails;

        private readonly IRetrievePackage retrievePackage;

        private readonly IUploadOffer uploadOffer;

        private readonly IMapper _mapper;

        public VendorController(IProjectsList _projects , IRetrievePackage retrievePackage , IVendorPackageDetails vendorPackageDetails , IPackageDetails packageDetails , IUploadOffer upload , IMapper mapper)
        {
            this.projects = _projects;
           
            this.retrievePackage = retrievePackage;

            this.vendorPackage = vendorPackageDetails ;

            this.packageDetails = packageDetails;

            this.uploadOffer = upload;

            this._mapper = mapper;
        }

        //public async Task<IActionResult> GetVendor(int user_id)
        //{
        //    var 
        //}
        public async Task<IActionResult> Index()
        {
            var projectsList = await projects.GetProjectList();
            return View(projectsList);
        }


        public IActionResult VendorProjectPackagesDetails(int id)
        {
            //  int project_id = (int)HttpContext.Session.GetInt32("projectid_V");
            HttpContext.Session.SetInt32("projectid", id);
            var result =  retrievePackage.GetProjectPackages(id);
            return View(result);
          
        }


        public async Task<IActionResult> _VendorGetPackageDetails(int id)
        {
            int project_id = (int)HttpContext.Session.GetInt32("projectid");
           // int project_id = 1;

           
            var result = await packageDetails.PackageDetails(id, project_id);
            return PartialView(result);
        }


        public async Task<IActionResult> PackageFullDetail(int id)
        {
            HttpContext.Session.SetInt32("pkg_id", id);
            var result = await vendorPackage.VendorPackageInfo(id);

            return View(result);
        }

        public async Task<IActionResult> UploadUserOffer(PackageOfferDTO packages_Offer)
        {
            packages_Offer.package_id = (int)HttpContext.Session.GetInt32("pkg_id");
            packages_Offer.user_id = 3;

            var folderName = Path.Combine("Resources", "Offers");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            string dbPath = "";
            string fileName = "";
            if (packages_Offer.File.Length > 0 && packages_Offer.File != null)
            {
                fileName = ContentDispositionHeaderValue.Parse(packages_Offer.File.ContentDisposition).FileName.Trim('"');

                var fullPath = Path.Combine(pathToSave, fileName);
                dbPath = Path.Combine(folderName, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    packages_Offer.File.CopyTo(stream);
                }

            }

            packages_Offer.file_name = fileName;
            packages_Offer.file_path = dbPath;

            var result = await uploadOffer.UploadUserOffer(packages_Offer);

            if (result > 0)
            {
                TempData["message"] = "Thanks for your documents !";
            }
         
            return RedirectToAction("VendorProjectPackagesDetails", "Vendor", new { id = (int)HttpContext.Session.GetInt32("projectid") });
        }

        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "Resources/PackagesFiles", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }



    }
}
