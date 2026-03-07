using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Interfaces;
using System.Linq;

namespace EPROCUREMENT.Services.Implementation
{
    public class VendorLogin : IVendorLogin
    {

        private readonly ProcurementDBContext procurementDBContext;

        public VendorLogin(ProcurementDBContext procurementDB)
        {
            this.procurementDBContext = procurementDB;
        }
        public int Login(string username, string password)
        {
            try
            {
                var user_login = (from a in procurementDBContext.user_header
                                  where a.username == username && a.password == password
                                  select a).FirstOrDefault();

                if (user_login != null)
                {
                    return 1;
                }
                return 0;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
