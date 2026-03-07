using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using EPROCUREMENT.Services.Interfaces;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EPROCUREMENT.Services.Implementation
{
    public class GetBOQChapters : IGetBOQChapters
    {
        private readonly ProcurementDBContext procurementDBContext;

        public GetBOQChapters(ProcurementDBContext procurementDBContext)
        {
            this.procurementDBContext = procurementDBContext;
        }
        public async Task<IEnumerable<boq_chapters>> loadboqchapters(int id)
        {
            try
            {
                var boq_chpts = await Task.Run(() => from a in procurementDBContext.boq_chapters
                                                     where a.boq_id == id
                                                     select a).Result.AsNoTracking().ToListAsync();

                return boq_chpts;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
