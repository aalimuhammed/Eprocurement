using EPROCUREMENT.DTO;
using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
    public class ServiceMaterials : IServiceMaterials
    {
        private readonly ProcurementDBContext _procurementDBContext;

        public ServiceMaterials(ProcurementDBContext procurementDBContext)
        {
            _procurementDBContext = procurementDBContext;   
        }
        public async Task<List<MaterialServiceDTO>> GetServiceMaterials(int industry_id)
        {
            var materials_services = await _procurementDBContext.service_mtr_grp
                                                    .Join(
                                                        _procurementDBContext.services_industries,
                                                        mtr_grp => mtr_grp.mtr_srv_id,
                                                        srv_ind => srv_ind.id,
                                                        (mtr_grp, srv_ind) => new { MtrGrp = mtr_grp, SrvInd = srv_ind }
                                                    )
                                                    .Where(x => x.MtrGrp.mtr_srv_id == industry_id)
                                                    .Select(i => new MaterialServiceDTO
                                                    {
                                                        MtrSrvGrpCode = i.MtrGrp.mtr_srv_grp_code,
                                                        IndustryCode = i.SrvInd.industry_code
                                                    })
                                                    .ToListAsync();

            return materials_services;
        }
    }
}