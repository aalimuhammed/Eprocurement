using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
    public class GetBOQ : IGetBOQ
    {
        private readonly ProcurementDBContext procurementDBContext;

        public GetBOQ(ProcurementDBContext procurementDBContext)
        {
            this.procurementDBContext = procurementDBContext;
        }
        public async Task<IEnumerable<boq_standards>> loadboqstnd()
        {
            try
            {
                var boq_stands = await Task.Run(() => from a in procurementDBContext.boq_standard
                                                      select a
                                                      ).Result.AsNoTracking().ToListAsync();



                return boq_stands;

            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
