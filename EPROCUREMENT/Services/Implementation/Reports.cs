using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
    public class Reports : IReports
    {
        private readonly ProcurementDBContext _procurementDBContext;

        public Reports(ProcurementDBContext procurementDBContext)
        {
            _procurementDBContext = procurementDBContext;
        }
        public async Task<DataTable> GetAllVendorPricesByPackageHeaderIdAsync(int packageHeaderId)
        {
            using (var command = _procurementDBContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "All_Applied_Report";
                command.CommandType = CommandType.StoredProcedure;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@pkg_id";
                parameter.DbType = DbType.Int32;
                parameter.Value = packageHeaderId;
                command.Parameters.Add(parameter);

                await _procurementDBContext.Database.OpenConnectionAsync();
                var result = new DataTable();
                result.Load(await command.ExecuteReaderAsync());
                return result;
            }
        }

        public async Task<DataTable> GetShortListed(int package_header_id)
        {
			using (var command = _procurementDBContext.Database.GetDbConnection().CreateCommand())
			{
				command.CommandText = "ShortListed_Report";
				command.CommandType = CommandType.StoredProcedure;

				var parameter = command.CreateParameter();
				parameter.ParameterName = "@package_id";
				parameter.DbType = DbType.Int32;
				parameter.Value = package_header_id;
				command.Parameters.Add(parameter);

				await _procurementDBContext.Database.OpenConnectionAsync();
				var result = new DataTable();
				result.Load(await command.ExecuteReaderAsync());
				return result;
			}
		}
    }
}
