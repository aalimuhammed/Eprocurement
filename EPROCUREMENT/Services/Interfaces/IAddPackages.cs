using EPROCUREMENT.DTO;
using EPROCUREMENT.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IAddPackages
    {
        public Task<int> AddNewPackages(ProjectPackageDTO NewProjectPackage);

    }
}
