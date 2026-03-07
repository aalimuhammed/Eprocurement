using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
    public class PackageDetail : IPackageDetails
    {
        private readonly ProcurementDBContext procuremenrDBContext;
        public PackageDetail(ProcurementDBContext procuremenrDBContext)
        {
            this.procuremenrDBContext = procuremenrDBContext;
        }
       public async Task<IEnumerable<PackageDetailsViewModel>> PackageDetails(int boq_stand_id , int project_id)
        {
            try
            {

                var package_details = await Task.Run(() => from pp in procuremenrDBContext.proj_packages
                                                           join bc in procuremenrDBContext.boq_chapters on pp.boq_cpt_id equals bc.id
                                                           join bs in procuremenrDBContext.boq_standard on bc.boq_id equals bs.id
                                                           join proj in procuremenrDBContext.projects on pp.prj_id equals proj.id
                                                           where bs.id == boq_stand_id && pp.prj_id == project_id
                                                           select new PackageDetailsViewModel
                                                           {
                                                               id = pp.id , 
                                                               name = bc.name ,
                                                               comments = pp.comments , 
                                                               start_date = pp.start_date , 
                                                               end_date = pp.end_date ,
                                                               file_name = pp.file_name
                                                           }).Result.AsNoTracking().ToListAsync();


                //var query = (from pp in procuremenrDBContext.proj_packages
                //             join bc in procuremenrDBContext.boq_chapters on pp.boq_cpt_id equals bc.id
                //             join bs in procuremenrDBContext.boq_standard on bc.boq_id equals bs.id
                //             join proj in procuremenrDBContext.projects on pp.prj_id equals proj.id
                //             where bs.id == boq_stand_id && pp.prj_id == project_id
                //             select new PackageDetailsViewModel
                //             {
                //                 id = pp.id,
                //                 name = bc.name,
                //                 comments = pp.comments,
                //                 start_date = pp.start_date,
                //                 end_date = pp.end_date,
                //                 file_name = pp.file_name
                //             }).ToList();

                return package_details;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
