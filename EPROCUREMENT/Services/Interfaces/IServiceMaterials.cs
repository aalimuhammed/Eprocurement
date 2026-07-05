using EPROCUREMENT.DTO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IServiceMaterials
    {
        Task<List<MaterialServiceDTO>> GetServiceMaterials(int industry_id , CancellationToken cancellationToken = default);
        Task<string> GetHeaderName(int header_id, CancellationToken cancellationToken = default);
        Task<int> AssignNewMaterialGroup(AssignNewMaterialGroupDTO assignNewMaterialGroupDTO, CancellationToken cancellationToken = default);
    }
}