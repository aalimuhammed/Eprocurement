using AutoMapper;
using EPROCUREMENT.DTO;
using EPROCUREMENT.Models;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class AddPackageController : Controller
    {
        private readonly IMapper _mapper;

        private readonly IAddPackages _addPackages;

        public AddPackageController(IMapper mapper , IAddPackages addPackages)
        {
            _mapper = mapper;
            _addPackages = addPackages;
        }

        public IActionResult _AddPackagePartialView()
        {
            return  PartialView();
        }

        
        public async Task<IActionResult> AddProjectPackage( ProjectPackageDTO projectPackageDTO)
        {
            // var proj_pack_mapped = _mapper.Map<proj_packages>(projectPackageDTO);
            // proj_pack_mapped.prj_id = 1;
            projectPackageDTO.prj_id = (int)HttpContext.Session.GetInt32("projectid");
            var resilt =  await _addPackages.AddNewPackages(projectPackageDTO);

            return RedirectToAction("GetPackages", "RetrievePackages", new { id = (int)HttpContext.Session.GetInt32("projectid") });
        }




   
    }
}
