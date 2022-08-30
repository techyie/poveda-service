using Microsoft.EntityFrameworkCore;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class RegionRepository : BaseRepository<regionEntity>, IRegionRepository
    {
        private readonly MyCampusCardContext context;

        public RegionRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<regionEntity> GetByID(int id)
        {
            return await _context.RegionEntity.Where(c => c.ID == id).FirstOrDefaultAsync();
        }
    }
}
