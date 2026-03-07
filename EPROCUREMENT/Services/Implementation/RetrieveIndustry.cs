using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
    public class RetrieveIndustry : IRetrieveIndustry
    {
        private readonly ProcurementDBContext procurementDBContext;

        public RetrieveIndustry(ProcurementDBContext procurementDBContext)
        {
            this.procurementDBContext = procurementDBContext;
        }
        public async Task<IQueryable<sap_industry_ViewModel>> Sap_Industry()
        {
            return await Task.Run(() => procurementDBContext.Sap_Industry_ViewModels.FromSqlRaw("CALL sap_industries();"));
        }
    }
}
