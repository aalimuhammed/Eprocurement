using EPROCUREMENT.DTO;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
	public interface ICreatePackage
	{
		public Task<int> CreateSapPackage(CreatePackageDTO createPackageDTO);

        public Task<int> CreateSapPackageManual(CreatePackageManualDTO createPackageManualDTO);

        public Task<int> CreateSapSrvPackage(CreateSrvPackageDTO createSrvPackageDTO);

        public Task<int> CreateSapSrvPackageManual(CreateSrvPackageDTOManual createSrvPackageDTO);
    }
}
