using EPROCUREMENT.DTO;
using Microsoft.AspNetCore.Http;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IAddPackageChapters
    {
        public int AddChapters(IFormFile formFile, ProjectPackageDTO projectPackageDTO);
    }
}
