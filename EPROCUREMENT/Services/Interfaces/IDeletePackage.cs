using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IDeletePackage
    {
        public Task<int> DeletePackage(int id);
    }
}
