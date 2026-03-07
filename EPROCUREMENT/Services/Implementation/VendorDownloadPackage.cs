using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
    public class VendorDownloadPackage : IVenodrDownloadFile
    {
        private readonly ProcurementDBContext procuremenrDBContext;

        public VendorDownloadPackage(ProcurementDBContext procurementDB)
        {
            this.procuremenrDBContext = procurementDB;
        }

        public async Task<FileResult> DownloadPackage(int id)
        {
            try
            {
                var file_name = await Task.Run(() => from prp in procuremenrDBContext.proj_packages
                                                     where prp.id == id
                                                     select prp.file_name).Result.FirstAsync();

                var path = Path.Combine(
                               Directory.GetCurrentDirectory(),
                               "Resources/PackagesFiles", file_name);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return null;

             //   return File(memory, GetContentType(path), Path.GetFileName(path));

            }
            catch (System.Exception)
            {

                throw;
            }
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
