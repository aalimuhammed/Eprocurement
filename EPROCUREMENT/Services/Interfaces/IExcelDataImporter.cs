using EPROCUREMENT.DTO;
using EPROCUREMENT.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IExcelDataImporter
    {
        public Task<bool> BulkInsertFromExcel(string filePath , ManualPackageDTO manualPackageDTO);
		public Task<int> AssignManualPackageForUsers(AssignManualDTO assignManualDTO);
		public Task<int> AcceptPackageManual(AcceptPackageManualDTO acceptPackageManualDTO);
		//public Task<IQueryable<ManualPackage_ViewModel>> GetManualPackages(ManualPackageDTO manualPackageDTO);
		public Task<IQueryable<ImportedPkgs_ViewModel>> GetImportedPkgs(ManualPackageDTO manualPackageDTO);
		public Task<IQueryable<ManualPackage_ViewModel>> GetImportedManualPackages(int pkgid);
	}
}