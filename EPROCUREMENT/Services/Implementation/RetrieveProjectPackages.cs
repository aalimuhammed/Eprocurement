using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.ViewModel;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Linq;

namespace EPROCUREMENT.Services.Implementation
{
    public class RetrieveProjectPackages : IRetrievePackage
    {
        private readonly ProcurementDBContext _procurementDBContext;
        public RetrieveProjectPackages(ProcurementDBContext procurementDBContext)
        {
            this._procurementDBContext = procurementDBContext;
        }
        public  IQueryable<ProjectPackageViewModel> GetProjectPackages(int proj_id)
        {
            MySqlParameter pRojectID = new MySqlParameter("@project_id", proj_id);

            return  _procurementDBContext.ProjectPackageViewModel.FromSqlRaw("CALL GETProjectPackages({0});", pRojectID);
        } 
    }
}