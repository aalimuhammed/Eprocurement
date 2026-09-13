using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
	public class AlreadyApplied : IAlreadyApplied
	{
		private readonly ProcurementDBContext _procurementDBContext;

        public AlreadyApplied(ProcurementDBContext procurementDBContext)
        {
			_procurementDBContext = procurementDBContext;	
        }
        public async Task<IQueryable<AlreadyApplied_Header_ViewModel>> AlreadyAppliedHeader(int user_id)
		{
			  return await Task.Run(() => _procurementDBContext.AlreadyApplied_Header_ViewModels.FromSqlRaw("CALL AlreadyApplied_Header({0});", user_id));
		}
	}
}
