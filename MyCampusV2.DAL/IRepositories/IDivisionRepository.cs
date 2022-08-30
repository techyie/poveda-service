using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IDivisionRepository : IBaseRepository<divisionEntity>
    {
        Task<divisionEntity> GetByID(int id);
        IQueryable<divisionEntity> GetAllDivision();
        Task<ICollection<divisionEntity>> GetByRegion(int id);
    }
}
