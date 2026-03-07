using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Linq;
using EFCore.BulkExtensions;
using EPROCUREMENT.Models;
using EPROCUREMENT.DTO;
using EPROCUREMENT.ViewModel;

namespace EPROCUREMENT.Services.Implementation
{
    public class ExcelDataService : IExcelDataImporter
    {
        private readonly ProcurementDBContext _procurementDBContext;

        public ExcelDataService(ProcurementDBContext procurementDBContext)
        {
            _procurementDBContext = procurementDBContext;
        }

		public async Task<int> AcceptPackageManual(AcceptPackageManualDTO acceptPackageManualDTO)
		{
			int result = 0;

			var pkg_header = await _procurementDBContext.packages_header.Where(z => z.id == acceptPackageManualDTO.Id).FirstOrDefaultAsync();

			pkg_header.file_path = acceptPackageManualDTO.filepath;

			_procurementDBContext.Entry(pkg_header).State = EntityState.Modified;

			result = await _procurementDBContext.SaveChangesAsync();

			return result;
		}

		public async Task<int> AssignManualPackageForUsers(AssignManualDTO assignManualDTO)
		{
			int result = 0;

			var pkg_header = await _procurementDBContext.packages_header.Where(z => z.id == assignManualDTO.Id).FirstOrDefaultAsync();

			pkg_header.file_path = assignManualDTO.filepath;

			_procurementDBContext.Entry(pkg_header).State = EntityState.Modified;

			var update_header  = await _procurementDBContext.SaveChangesAsync();

			if (update_header > 0)
			{
				var packg_details = await _procurementDBContext.packages_details.Where(x => x.pkg_id == assignManualDTO.Id).ToListAsync();
				//var packageDetails = await (
				//					from pd in _procurementDBContext.packages_details
				//					join rp in _procurementDBContext.revoked_packages
				//					on pd.serial equals rp.serial
				//					where pd.pkg_id == assignManualDTO.Id
				//					select pd
				//                      	).ToListAsync();

				var userCount = assignManualDTO.user_Id.Count;
				var currentIndex = 0;

				foreach (var item in packg_details)
				{
					var currentUserIndex = currentIndex % userCount;
					var currentUser = assignManualDTO.user_Id[currentUserIndex];

					item.user_id = currentUser;
					_procurementDBContext.Entry(item).State = EntityState.Modified;
					//result = await _procurementDBContext.SaveChangesAsync();

					currentIndex++; // Move to the next user ID for the next package detail
				}
				result = await _procurementDBContext.SaveChangesAsync();

				return result;
			}
			return result;

		}

		public async Task<bool> BulkInsertFromExcel(string filePath , ManualPackageDTO manualPackageDTO)
        {
            try
            {
                int result = 0;
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
					ExcelPackage.LicenseContext = LicenseContext.Commercial;
					ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

					int maxId = await _procurementDBContext.packages_header.AnyAsync()
						 ? await _procurementDBContext.packages_header.MaxAsync(u => u.id)
						 : 0;

					int pkg_id = await _procurementDBContext.packages_header
										.Where(x => x.project_id == manualPackageDTO.project_id
												 && x.industry_id == manualPackageDTO.industry_id)
										.AnyAsync()
										? await _procurementDBContext.packages_header
											.Where(x => x.project_id == manualPackageDTO.project_id
													 && x.industry_id == manualPackageDTO.industry_id)
											.MaxAsync(u => u.pkg_num)
										: 0;


					string appendedValue = (pkg_id + 1).ToString("00");

					// Concatenate the package name with the formatted appended value
					string pkgName = manualPackageDTO.pack_name + "/" + appendedValue;

					// Create a new packages_header instance
					var packageheader = new packages_header
					{
						pkg_name = pkgName,
						excl_import = true,
						project_id = manualPackageDTO.project_id,
						industry_id = manualPackageDTO.industry_id,
						assigned_by = 1,
						assign_type = 'M',
						pkg_num = pkg_id + 1
					};

					await _procurementDBContext.packages_header.AddAsync(packageheader);
					var header_insert = await _procurementDBContext.SaveChangesAsync();

                    if (header_insert > 0)
                    {

						for (int row = 2; row <= rowCount; row++) // Assuming row 1 is header
						{
							//var entity = new package_manual
							//{
							//	serial = worksheet.Cells[row, 1].Value.ToString(),
							//	material_description = worksheet.Cells[row, 2].Value.ToString(),
							//	uom = worksheet.Cells[row, 3].Value.ToString(),
							//	qty = worksheet.Cells[row, 4].Value.ToString(),
							//	industry_id = manualPackageDTO.industry_id,
							//	project_id = manualPackageDTO.project_id,
							//	pack_no = pkg_id + 1,
							//	package_name = manualPackageDTO.pack_name
							//};

							var package_details = new packages_details()
							{
								serial = worksheet.Cells[row, 1].Value.ToString(),
								pkg_id = maxId + 1,
								mtr_uom = worksheet.Cells[row, 3].Value.ToString(),
								mtr_desc = worksheet.Cells[row, 2].Value.ToString(),
								mtr_qty = worksheet.Cells[row, 4].Value.ToString(),
								user_id = 0,
						    };
						

							await _procurementDBContext.packages_details.AddAsync(package_details);

							result = await _procurementDBContext.SaveChangesAsync();


						}
						if (result > 0)
						{
							return true;
						}
						return false;
					}
					return false;

				}
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return false;
            }
        }

		public async Task<IQueryable<ImportedPkgs_ViewModel>> GetImportedPkgs(ManualPackageDTO manualPackageDTO)
		{
			string sqlQuery = $"CALL import_pkg_manual({manualPackageDTO.project_id}, {manualPackageDTO.industry_id});";
			return await Task.Run(() => _procurementDBContext.ImportedPkgs_ViewModels.FromSqlRaw(sqlQuery));
		}

		//public async Task<IQueryable<ManualPackage_ViewModel>> GetManualPackages(ManualPackageDTO manualPackageDTO)
  //      {
  //          string sqlQuery = $"CALL pkg_manual({manualPackageDTO.project_id}, {manualPackageDTO.industry_id});";
  //          return await Task.Run(() => _procurementDBContext.ManualPackage_ViewModels.FromSqlRaw(sqlQuery));
  //      }

		public async Task<IQueryable<ManualPackage_ViewModel>> GetImportedManualPackages(int pkgid)
		{
			return await Task.Run(() => _procurementDBContext.ManualPackage_ViewModels.FromSqlRaw("CALL pkg_manual({0});", pkgid));
		}
	}
}
