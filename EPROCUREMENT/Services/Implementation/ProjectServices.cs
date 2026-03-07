using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EPROCUREMENT.Services.Implementation
{
    public class ProjectServices : IProjectsList
    {
        private readonly ProcurementDBContext procurementDBContext;
        public ProjectServices(ProcurementDBContext procurementDBContext)
        {
            this.procurementDBContext = procurementDBContext;
        }
        public async Task<IEnumerable<ProjectViewModel>> GetProjects()
        {
            try
            {

                var projectViewModels = await Task.Run(() =>  from pr in procurementDBContext.projects
                                  join pr_are in procurementDBContext.projects_area on pr.area_id equals pr_are.id
                                  select new ProjectViewModel
                                  { 
                                      id = pr.id ,
                                      project_numb = pr.project_numb , 
                                      project_name = pr.project_name , 
                                      start_date = pr.start_date ,
                                      end_date = pr.end_date,
                                      status = pr.status == false ? "TENDERING" : "AWARDED" ,
                                      area_name = pr_are.area_name , 
                                     // NoPkg =
                                  }).Result.AsNoTracking().ToListAsync();

            


                return projectViewModels;

            }
            catch (System.Exception)
            {

                throw;
            }
        }


        public async Task<IQueryable<ProjectViewModel>> GetProjectList()
        {
            //MySqlParameter pRojectID = new MySqlParameter("@project_id", proj_id);

            return await Task.Run(()=> procurementDBContext.ProjectViewModels.FromSqlRaw("CALL getptojectlist();"));

        }

        public async Task<IQueryable<sap_project_ViewModel>> Sap_Projects()
        {
            return await Task.Run(() => procurementDBContext.Sap_Project_ViewModels.FromSqlRaw("CALL sap_projects();"));
        }
    }
}
