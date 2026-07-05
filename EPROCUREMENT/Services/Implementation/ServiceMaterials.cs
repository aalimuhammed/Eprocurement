using EPROCUREMENT.DTO;
using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public async Task<int> AssignNewMaterialGroup(
            AssignNewMaterialGroupDTO assignNewMaterialGroupDTO, 
            CancellationToken cancellationToken = default)
        {
            var materialGrpExist = await _procurementDBContext.service_mtr_grp.AnyAsync
                (i => i.mtr_srv_grp_code == assignNewMaterialGroupDTO.MaterialGroup);

            if (materialGrpExist)
            {
                throw new InvalidOperationException(
                             $"Material group '{assignNewMaterialGroupDTO.MaterialGroup}' already exists.");
            }

            var newMaterialGroup = new service_mtr_grp
            {
                mtr_srv_id = assignNewMaterialGroupDTO.IndustryId,
                mtr_srv_grp_code = assignNewMaterialGroupDTO.MaterialGroup,
                mtr_srv_grp_desc = assignNewMaterialGroupDTO.Description
            };

            _procurementDBContext.service_mtr_grp.Add(newMaterialGroup);
            return await _procurementDBContext.SaveChangesAsync(cancellationToken);   
        }

        public async Task<string> GetHeaderName(int header_id, CancellationToken cancellationToken = default)
        {
            var headerName = await _procurementDBContext.service_header
                                        .Where(h => h.id == header_id)
                                        .Select(h => h.name)
                                        .FirstOrDefaultAsync(cancellationToken);
            return headerName;  
        }

        public async Task<List<MaterialServiceDTO>> GetServiceMaterials(int industry_id , CancellationToken cancellationToken)
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
                                                    .ToListAsync(cancellationToken);

            return materials_services;
        }
    }
}