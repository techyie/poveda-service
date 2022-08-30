using MyCampusV2.Models.V2.entity;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IRegionRepository : IBaseRepository<regionEntity>
    {
        Task<regionEntity> GetByID(int id);
    }
}
