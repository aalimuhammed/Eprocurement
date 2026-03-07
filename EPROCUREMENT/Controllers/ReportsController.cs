using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReports _reports;

        public ReportsController(IReports reports)
        {
            _reports = reports;
        }

        [HttpGet]
        public async Task<IActionResult> ShortListedVendors(int package_header_id)
        {
			var dataTable = await _reports.GetShortListed(package_header_id);

			// Populate ReportResult object
			var result = new AllApplied_ViewModel
			{
				Rows = dataTable.AsEnumerable()
								.Select(row => dataTable.Columns.Cast<DataColumn>()
															   .ToDictionary(col => col.ColumnName, col => row[col]))
								.ToList(),
				Columns = dataTable.Columns.Cast<DataColumn>()
										   .Select(col => col.ColumnName)
										   .ToList()
			};

			return Ok(result);
		}

        [HttpGet]
        public async Task<IActionResult> AllAppliedVendors(int package_header_id)
        {
            var dataTable = await _reports.GetAllVendorPricesByPackageHeaderIdAsync(package_header_id);

            // Populate ReportResult object
            var result = new AllApplied_ViewModel
            {
                Rows = dataTable.AsEnumerable()
                                .Select(row => dataTable.Columns.Cast<DataColumn>()
                                                               .ToDictionary(col => col.ColumnName, col => row[col]))
                                .ToList(),
                Columns = dataTable.Columns.Cast<DataColumn>()
                                           .Select(col => col.ColumnName)
                                           .ToList()
            };

            return Ok(result);
        }
    }
}