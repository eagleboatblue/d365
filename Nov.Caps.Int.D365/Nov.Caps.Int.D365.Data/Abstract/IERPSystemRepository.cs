using System.Threading.Tasks;
using Nov.Caps.Int.D365.Models.ERPSystems;

namespace Nov.Caps.Int.D365.Data.Abstract
{
    public interface IERPSystemRepository
    {
        Task<ERPSystem> GetByIDAsync(int id);
    }
}
