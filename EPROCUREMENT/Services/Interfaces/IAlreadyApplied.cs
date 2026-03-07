using EPROCUREMENT.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
	public interface IAlreadyApplied
	{
		public Task<IQueryable<AlreadyApplied_Header_ViewModel>> AlreadyAppliedHeader(int user_id);
	}
}
