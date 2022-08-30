using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories.Report;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories.Report
{
    public class CampusReportRepository : BaseReportRepository<campusEntity>, ICampusReportRepository
    {
        private readonly MyCampusCardReportContext context;

        public CampusReportRepository(MyCampusCardReportContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<campusPagedResult> ExportCampus(string keyword)
        {
            var result = new campusPagedResult();

            if (keyword == null || keyword == "")
                result.campuses = await this.context.CampusEntity
                    .Include(x => x.DivisionEntity)
                    .ThenInclude(z => z.RegionEntity)
                    .OrderBy(c => c.Campus_Name)
                    .Where(q => q.ToDisplay == true).ToListAsync();
            else
                result.campuses = await this.context.CampusEntity
                    .Include(x => x.DivisionEntity)
                    .ThenInclude(z => z.RegionEntity)
                    .OrderBy(c => c.Campus_Name)
                    .Where(q => q.Campus_Name.Contains(keyword)
                    || q.Campus_Address.Contains(keyword)
                    || q.Campus_ContactNo.Contains(keyword)
                    || q.DivisionEntity.Name.Contains(keyword)
                    || q.DivisionEntity.RegionEntity.Name.Contains(keyword))
                    .Where(q => q.ToDisplay == true).ToListAsync();
            return result;
        }

    }
}
