using EPROCUREMENT.DTO;
using EPROCUREMENT.sap;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class PackageDetailsController : Controller
    {
        private readonly IPackageDetails packageDetails;

        public PackageDetailsController(IPackageDetails packageDetails)
        {
            this.packageDetails = packageDetails; 
        }

        public IActionResult PackagesDetails()
        {
            return View();
        }
        public async Task<IActionResult> _GetPackageDetails(int id)
        {
            int project_id = (int)HttpContext.Session.GetInt32("projectid"); 
            var result = await packageDetails.PackageDetails(id , project_id);
            return PartialView(result);
        }


		[HttpPost]
		public async Task<IActionResult> Packages(SapPackagesApiDTO sapPackagesApiDTO) 
		{
            var checkingMaterial = sapPackagesApiDTO.inudstries[0].IndustryCode.Contains('M');
            var projectId = sapPackagesApiDTO.project_num;

            var indParam = checkingMaterial ? "1" : "2";
            if(indParam == "1")
            {
                // live
                 var packagesResponses = await PackagesApi.PostToSapApi(sapPackagesApiDTO, "http://prd-app.siac-construction.com:8000/ze-proq_package?project=" + projectId + "&ind=" + indParam);

                //sand
               // var packagesResponses = await PackagesApi.PostToSapApi(sapPackagesApiDTO, "http://snd.siac-construction.com:8080/ze-proq_package?project=" + projectId + "&ind=" + indParam);

                return Ok(packagesResponses);
            }
            //live
             var packagesResponsesService = await PackagesApi.PostToSapApiService(sapPackagesApiDTO, "http://prd-app.siac-construction.com:8000/ze-proq_package?project=" + projectId + "&ind=" + indParam);
            
            //sand
           //var packagesResponsesService = await PackagesApi.PostToSapApiService(sapPackagesApiDTO, "http://snd.siac-construction.com:8080/ze-proq_package?project=" + projectId + "&ind=" + indParam);

            return Ok(packagesResponsesService);
        }

	}
}