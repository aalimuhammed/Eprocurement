using EPROCUREMENT.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IServiceMaterials
    {
        public Task<List<MaterialServiceDTO>> GetServiceMaterials(int industry_id);
    }
}
