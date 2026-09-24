using EPROCUREMENT.DTO;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
	public interface ICreatePackage
	{
		 Task<int> CreateSapPackage(CreatePackageDTO createPackageDTO);
         Task<int> CreateSapPackageManual(CreatePackageManualDTO createPackageManualDTO);
         Task<int> CreateSapSrvPackage(CreateSrvPackageDTO createSrvPackageDTO);
         Task<int> CreateSapSrvPackageManual(CreateSrvPackageDTOManual createSrvPackageDTO);
    }
}