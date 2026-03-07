using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EPROCUREMENT.Services.Implementation
{
    public class DeletePackage : IDeletePackage
    {
        private readonly ProcurementDBContext procurementDBContext;

        public DeletePackage(ProcurementDBContext procurementDB)
        {
            this.procurementDBContext = procurementDB;
        }

        async Task<int> IDeletePackage.DeletePackage(int id)
        {
            try
            {
                var package = await Task.Run(() => from a in procurementDBContext.proj_packages
                                                   where a.id == id
                                                   select a
                                                      ).Result.FirstOrDefaultAsync();

                procurementDBContext.Entry(package).State = EntityState.Deleted;
               return await procurementDBContext.SaveChangesAsync();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
