using EPROCUREMENT.DTO;
using EPROCUREMENT.Enums;
using EPROCUREMENT.ViewModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IGetMaterialGrp
    {
         Task<List<MaterialGrpViewModel>> MaterialsGrp(CancellationToken cancellationToken = default);

         Task<List<MaterialGrpViewModel>> GetServices(CancellationToken cancellationToken = default);

        Task<List<MaterialGrpViewModel>> GetMaterials(CancellationToken cancellationToken = default);

        Task<List<SelectedMaterialServicesDTO>> GetMTROrService(IndustryType industryType, CancellationToken cancellationToken = default);

        Task<IndustryViewModel> Get_Industry(int mtr_id);
    }
}
