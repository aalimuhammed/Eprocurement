using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using EPROCUREMENT.Services.Interfaces;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace EPROCUREMENT.Services.Implementation
{
    public class ProjectsOperations : IProjectsOperations
    {
        private readonly ProcurementDBContext procurementDBContext;

        public ProjectsOperations(ProcurementDBContext procurementDBContext)
        {
            this.procurementDBContext = procurementDBContext;
        }
        public async Task createproject(projects projects)
        {
            try
            {
                procurementDBContext.Attach(projects);
                procurementDBContext.Entry(projects).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                await procurementDBContext.SaveChangesAsync();

            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public async Task<bool> DeleteProject(int id)
        {
            try
            {
                var project_tobedeleted = (from a in procurementDBContext.projects
                                           where a.id == id
                                           select a).FirstOrDefault();

               // procurementDBContext.projects.Attach(project_tobedeleted);

                procurementDBContext.Entry(project_tobedeleted).State = Microsoft.EntityFrameworkCore.EntityState.Deleted; 
               //   procurementDBContext.projects.Remove(project_tobedeleted);
                var deleted =  await procurementDBContext.SaveChangesAsync();

                if (deleted > 0)
                {
                    return true;
                }
                return false;




            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public projects GetProject(int id)
        {
            var project_selected = (from a in procurementDBContext.projects
                                    where a.id == id
                                    select new projects
                                    {
                                        id = a.id,
                                        area_id = a.area_id,
                                        start_date = a.start_date , 
                                        project_name = a.project_name,
                                        project_numb = a.project_numb , 
                                        end_date = a.end_date , 
                                        status = a.status

                                    }).FirstOrDefault();


            var anymethod = (from a in procurementDBContext.projects
                             select a).Any();

            return project_selected;
        }

        public async Task<bool> UpdateProject(projects projects)
        {
            
            try
            {
                
             //   projects prj = procurementDBContext.projects.Where(p => p.id == projects.id).FirstOrDefault();
                if (projects != null)
                {
                    //prj.project_numb = projects.project_numb;
                    //prj.project_name = projects.project_name;
                    //prj.start_date = projects.start_date;
                    //prj.end_date = projects.end_date;
                    //prj.area_id = projects.area_id;
                    procurementDBContext.Entry(projects).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    var updated = await  procurementDBContext.SaveChangesAsync();

                    if (updated > 0)
                    {
                        return true;
                    }
                }
               
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
    }
}
