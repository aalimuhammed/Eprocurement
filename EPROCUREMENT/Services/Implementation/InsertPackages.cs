using AutoMapper;
using EPROCUREMENT.DTO;
using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
    public class InsertPackages : IAddPackages
    {
        private readonly ProcurementDBContext procurementDBContext;

        private readonly IMapper _mapper;
        public InsertPackages(ProcurementDBContext procurementDBContext , IMapper mapper)
        {
            this.procurementDBContext = procurementDBContext;
            this._mapper = mapper;
        }

        public async Task<int> AddNewPackages(ProjectPackageDTO NewProjectPackage)
        {
            try
            {
                var proj_pack_mapped = _mapper.Map<proj_packages>(NewProjectPackage);

                var folderName = Path.Combine("Resources", "PackagesFiles");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                string dbPath = "";
                string fileName = "";
                if (NewProjectPackage.File.Length > 0)
                {
                     fileName = ContentDispositionHeaderValue.Parse(NewProjectPackage.File.ContentDisposition).FileName.Trim('"');

                    var fullPath = Path.Combine(pathToSave, fileName);
                    dbPath = Path.Combine(folderName, fileName);



                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        NewProjectPackage.File.CopyTo(stream);
                    }

                }

                proj_pack_mapped.file_name = fileName;
                proj_pack_mapped.file_path = dbPath;
                   
                procurementDBContext.Attach(proj_pack_mapped);

                procurementDBContext.Entry(proj_pack_mapped).State = Microsoft.EntityFrameworkCore.EntityState.Added;

                 var result =    await procurementDBContext.SaveChangesAsync();

                return result;

            }
            catch (System.Exception)
            {

                throw;
            }
        }

        
    }
}
