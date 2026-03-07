using EPROCUREMENT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IGetBOQChapters
    {
        public Task<IEnumerable<boq_chapters>> loadboqchapters(int id);
    }
}
