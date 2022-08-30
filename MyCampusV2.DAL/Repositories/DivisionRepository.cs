using Microsoft.EntityFrameworkCore;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class DivisionRepository : BaseRepository<divisionEntity>, IDivisionRepository
    {
        private readonly MyCampusCardContext context;

        public DivisionRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<divisionEntity> GetByID(int id)
        {
            return await _context.DivisionEntity.Include(x => x.RegionEntity).Where(c => c.ID == id).FirstOrDefaultAsync();
        }

        public async Task<ICollection<divisionEntity>> GetByRegion(int id)
        {
            return await _context.DivisionEntity.Include(x => x.RegionEntity).Where(c => c.ID == id).ToListAsync();
        }

        public IQueryable<divisionEntity> GetAllDivision()
        {
            return _context.DivisionEntity.Include(x => x.RegionEntity);
        }
    }
}
