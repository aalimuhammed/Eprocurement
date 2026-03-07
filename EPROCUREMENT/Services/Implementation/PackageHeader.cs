using EPROCUREMENT.DTO;
using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
    public class PackageHeader : IPackageHeader
    {
        private readonly ProcurementDBContext _procurementDBContext;
        public PackageHeader(ProcurementDBContext procurementDBContext)
        {
            _procurementDBContext = procurementDBContext;
        }
        public async Task<PackageProjectIndustryDTO> GetPackagesHeaderAsync(int pkg_id)
        {
            var pack_header = await (from ph in _procurementDBContext.packages_header
                                     join pr in _procurementDBContext.sap_projects on ph.project_id equals pr.Id 
                                     join ind in _procurementDBContext.services_industries on ph.industry_id equals ind.id
                                     where ph.id == pkg_id
                                     select new PackageProjectIndustryDTO
                                     {
                                         PackageName = ph.pkg_name,
                                         ProjectName = pr.proj_name,
                                         Industry_Name = ind.descr
                                     }).FirstOrDefaultAsync();

            return pack_header;
        }
    }
}
