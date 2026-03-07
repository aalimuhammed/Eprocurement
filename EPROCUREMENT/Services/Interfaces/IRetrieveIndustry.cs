using EPROCUREMENT.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
	public interface IRetrieveIndustry
	{
        public Task<IQueryable<sap_industry_ViewModel>> Sap_Industry();
    }
}
