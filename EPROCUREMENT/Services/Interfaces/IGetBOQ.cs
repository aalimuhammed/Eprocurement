using EPROCUREMENT.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IGetBOQ
    {
        public Task<IEnumerable<boq_standards>> loadboqstnd();
    }
}
