using EPROCUREMENT.DTO;
using EPROCUREMENT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
	public interface IRevokePackages
	{
		public Task<int> revokePackages(RevokingPackageDTO revokePackagesDTO);

		public Task<List<revoked_packages>> GetRevokedPackages(int project_id , int industry_id);

		public Task<int> CancelRevoke(CancelRevokeDTO cancelRevokeDTO);

		public Task<int> revokePackagesManula(RevokingPackageManualDTO revokePackagesManualDTO);
	}
}
