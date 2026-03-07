using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using EPROCUREMENT.ViewModel;
using System.Linq;
using System.Threading.Tasks;
using EPROCUREMENT.Models;
using EPROCUREMENT.DTO;

namespace EPROCUREMENT.Services.Implementation
{
    public class VendorPackageDetails : IVendorPackageDetails
    {
        private readonly ProcurementDBContext procuremenrDBContext;
        public VendorPackageDetails(ProcurementDBContext procurementDB)
        {
            procuremenrDBContext = procurementDB;
        }

        public async Task<PackageOfferDTO> VendorPackageInfo(int id)
        {
            try
            {

                var package_details = await Task.Run(() => from pp in procuremenrDBContext.proj_packages
                                                           join proj in procuremenrDBContext.projects on pp.prj_id equals proj.id
                                                           where pp.id == id
                                                           select new PackageOfferDTO
                                                           {
                                                               id = pp.id,
                                                               project_name = proj.project_name , 
                                                               comments = pp.comments , 
                                                               end_date = pp.end_date , 
                                                               start_date = pp.start_date , 
                                                               file_name = pp.file_name
                                                           }).Result.FirstAsync();

                return package_details;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
