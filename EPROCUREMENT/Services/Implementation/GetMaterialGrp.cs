using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace EPROCUREMENT.Services.Implementation
{
	public class GetMaterialGrp : IGetMaterialGrp
    {
        private readonly ProcurementDBContext _procurementDBContext;
        public GetMaterialGrp(ProcurementDBContext procurementDBContext) 
        {
            _procurementDBContext = procurementDBContext;
        }
        public async Task<List<MaterialGrpViewModel>> MaterialsGrp(CancellationToken cancellationToken = default)
        {
            var materials = await (from mtr in _procurementDBContext.service_mtr_grp
                                   select new MaterialGrpViewModel
                                   {
                                       id = mtr.mtr_srv_id ,
                                       name = mtr.mtr_srv_grp_code + " - " + mtr.mtr_srv_grp_desc
                                   }).ToListAsync(cancellationToken); 

            return materials;

        }
        public async Task<List<MaterialGrpViewModel>> GetMaterials(
            CancellationToken cancellationToken = default)
        {

            return await (
                from mtr in _procurementDBContext.service_mtr_grp
                join ind in _procurementDBContext.services_industries
                    on mtr.mtr_srv_id equals ind.id
                where ind.industry_code.StartsWith("M")
                select new MaterialGrpViewModel
                {
                    id = mtr.mtr_srv_id,
                    name = mtr.mtr_srv_grp_code + " - " + mtr.mtr_srv_grp_desc
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        }

        public async Task<List<MaterialGrpViewModel>> GetServices(CancellationToken cancellationToken = default)
        {
            return await (
                from mtr in _procurementDBContext.service_mtr_grp
                join ind in _procurementDBContext.services_industries
                    on mtr.mtr_srv_id equals ind.id
                where ind.industry_code.StartsWith("S") && mtr.mtr_srv_grp_desc != null && mtr.mtr_srv_grp_code != null
                select new MaterialGrpViewModel
                {
                    id = mtr.mtr_srv_id,
                    name = mtr.mtr_srv_grp_code + " - " + mtr.mtr_srv_grp_desc
                }
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        }
        public async Task<IndustryViewModel> Get_Industry(int mtr_id)
        {
			var industry = await(from ind in _procurementDBContext.services_industries
                                 where ind.id == mtr_id
								 select new IndustryViewModel
								  {
                                      name = ind.descr  + " - " + ind.english_desc
								  }).FirstOrDefaultAsync();

			return industry;
		}

    }
}