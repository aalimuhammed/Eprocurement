using EPROCUREMENT.DTO;
using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using EPROCUREMENT.sap;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
	public class RevokePackages : IRevokePackages
	{
		private readonly ProcurementDBContext _procurementDBContext;

        public RevokePackages(ProcurementDBContext procurementDBContext)
        {
			_procurementDBContext = procurementDBContext;
        }

		public async Task<int> CancelRevoke(CancelRevokeDTO cancelRevokeDTO)
		{
			var result = 0;

			foreach (var item in cancelRevokeDTO.line_id)
			{
				var revoked_package = await _procurementDBContext.revoked_packages
				                            	.Where(x => x.id == item)
				                            	.FirstOrDefaultAsync();

				SapCancelRevokeDTO sapCancelRevokeDTO = new SapCancelRevokeDTO();
				sapCancelRevokeDTO.pr_num = int.Parse (revoked_package.pr_num);
				sapCancelRevokeDTO.line_item = int.Parse(revoked_package.line_item);

				revoked_package.cancelled = true;

				_procurementDBContext.Entry(revoked_package).State = EntityState.Modified;

				result = await _procurementDBContext.SaveChangesAsync();

				await SapApi.CancelRevokeSAP(sapCancelRevokeDTO);
			}

			return result;
		}

		public async Task<List<revoked_packages>> GetRevokedPackages(
			int project_id, int? industry_id , CancellationToken cancellationToken)
		{
            var query = _procurementDBContext.revoked_packages
                  .Where(x => x.project_id == project_id && x.cancelled != true);

            if (industry_id.HasValue && industry_id > 0)
            {
                query = query.Where(x => x.industry_id == industry_id.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }
		public async Task<int> revokePackages(RevokingPackageDTO revokePackagesDTO)
		{
			int result = 0;

			if (revokePackagesDTO.selected_rows != null)
			{
				foreach (var item in revokePackagesDTO.selected_rows)
				{
					var revoke_package = new revoked_packages();
					revoke_package.mtr_code = item.mat_code;
					revoke_package.mtr_uom = item.m_uom;
					revoke_package.mtr_batch = item.m_batch;
					revoke_package.mtr_qty = item.m_req_qty;
					revoke_package.mtr_long_desc = item.m_long_desc;
					revoke_package.line_item = item.pr_item;
					revoke_package.mtr_desc = item.mat_desc;
					revoke_package.m_grp = item.m_grp;
					revoke_package.pr_num = item.pr;
					revoke_package.pr_date = DateTime.Parse(item.pr_req_date);
					revoke_package.project_id = revokePackagesDTO.project_id;
					revoke_package.industry_id = revokePackagesDTO.industry_id;

					await _procurementDBContext.revoked_packages.AddAsync(revoke_package);
					result =  await _procurementDBContext.SaveChangesAsync();

                    var SapDTO = new SapStatusDTO();
                    //SapDTO.EPACKAGE = pkgName;
                    SapDTO.PR = item.pr;
                    SapDTO.ITEM = int.Parse(item.pr_item);
                    SapDTO.STATUS = "Revoked";

                    await SapApi.SaveToSAPAsync(SapDTO);
                }
			}


			//var revokePackage = new revoked_packages();
			//revokePackage.project_id = revokePackagesDTO.project_id;
			//revokePackage.industry_id = revokePackagesDTO.industry_id;

			//await _procurementDBContext.revoked_packages.AddAsync(revokePackage);
			//result =  await _procurementDBContext.SaveChangesAsync();

			return result;
		}

		public async Task<int> revokePackagesManula(RevokingPackageManualDTO revokePackagesManualDTO)
		{
			int result = 0;

			if (revokePackagesManualDTO.selected_rows != null)
			{
				foreach (var item in revokePackagesManualDTO.selected_rows)
				{
					var revoked_pkg_manual = new revoked_packages();
					revoked_pkg_manual.mtr_desc = item.mat_desc;
					revoked_pkg_manual.mtr_uom = item.m_uom;
					revoked_pkg_manual.mtr_qty = item.m_qty;
					revoked_pkg_manual.serial = item.serial;
					revoked_pkg_manual.project_id = revokePackagesManualDTO.project_id;
					revoked_pkg_manual.industry_id = revokePackagesManualDTO.industry_id;

					await _procurementDBContext.revoked_packages.AddAsync(revoked_pkg_manual);
					result = await _procurementDBContext.SaveChangesAsync();
				}
			}

			return result;
		}
	}
}