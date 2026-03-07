using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IVenodrDownloadFile
    {
        public Task<FileResult> DownloadPackage(int id);
    }
}
