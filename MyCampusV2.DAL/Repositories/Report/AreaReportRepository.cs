using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories.Report;
using MyCampusV2.Models.V2.entity;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories.Report
{
    public class AreaReportRepository : BaseReportRepository<areaEntity>, IAreaReportRepository
    {
        private readonly MyCampusCardReportContext context;

        public AreaReportRepository(MyCampusCardReportContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<areaPagedResult> ExportAreas(string keyword)
        {
            var result = new areaPagedResult();

            if (keyword == null || keyword == "")
                result.areas = await this.context.areaEntity
                    .Include(x => x.CampusEntity)
                    .OrderBy(c => c.Area_Name).ToListAsync();
            else
                result.areas = await _context.areaEntity
                    .Include(x => x.CampusEntity)
                    .OrderBy(c => c.Area_Name)
                    .Where(q => q.Area_Name.Contains(keyword)
                    || q.Area_Description.Contains(keyword)
                    || q.CampusEntity.Campus_Name.Contains(keyword)).ToListAsync();

            return result;
        }
    }
}
