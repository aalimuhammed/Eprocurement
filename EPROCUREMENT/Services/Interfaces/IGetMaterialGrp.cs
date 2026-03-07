using EPROCUREMENT.Models;
using EPROCUREMENT.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IGetMaterialGrp
    {
        public Task<List<MaterialGrpViewModel>> MaterialsGrp();

        public Task<IndustryViewModel> Get_Industry(int mtr_id);
    }
}
