using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.DAL.IRepositories.Report;
using MyCampusV2.Models.V2.entity;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories.Report
{
    public class PositionReportRepository : BaseReportRepository<positionEntity>, IPositionReportRepository
    {
        private readonly MyCampusCardReportContext context;

        public PositionReportRepository(MyCampusCardReportContext Context) : base(Context)
        {
            this.context = Context;
        }

        public async Task<positionPagedResult> ExportPositions(string keyword)
        {
            var result = new positionPagedResult();

            if (keyword == null || keyword == "")
                result.positions = await _context.PositionEntity
                    .Include(x => x.DepartmentEntity)
                    .ThenInclude(x => x.CampusEntity)
                    .OrderBy(c => c.Position_Name).ToListAsync();
            else
                result.positions = await _context.PositionEntity
                    .Include(x => x.DepartmentEntity)
                    .ThenInclude(x => x.CampusEntity)
                    .OrderBy(c => c.Position_Name)
                    .Where(q => q.Position_Name.Contains(keyword)
                    || q.Position_Name.Contains(keyword)
                    || q.DepartmentEntity.Department_Name.Contains(keyword)
                    || q.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)).ToListAsync();

            return result;
        }

    }
}
