using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.Repositories;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories.Report
{
    public class EducationLevelReportRepository : BaseReportRepository<educationalLevelEntity>, IEducationLevelReportRepository
    {
        private readonly MyCampusCardReportContext context;

        public EducationLevelReportRepository(MyCampusCardReportContext Context)
                : base(Context)
        {
            this.context = Context;
        }

        public async Task<educlevelPagedResult> ExportEducationalLevels(string keyword)
        {
            var result = new educlevelPagedResult();

            if (keyword == null || keyword == "")
                result.educlevels = await _context.EducationalLevelEntity
                    .Include(x => x.CampusEntity)
                    .Where(q => q.ToDisplay == true)
                    .OrderBy(c => c.Level_Name).ToListAsync();
            else if (keyword.ToLower().Contains("yes") || keyword.ToLower().Contains("no"))
                result.educlevels = await _context.EducationalLevelEntity
                    .Include(x => x.CampusEntity)
                    .Where(q => q.ToDisplay == true && q.hasCourse.Equals(keyword.ToLower().Contains("yes") ? true : false))
                    .OrderBy(c => c.Level_Name).ToListAsync();
            else
                result.educlevels = await _context.EducationalLevelEntity
                    .Include(x => x.CampusEntity)
                    .OrderBy(c => c.Level_Name)
                    .Where(q => q.ToDisplay == true 
                    && (q.Level_Name.Contains(keyword)
                    || q.CampusEntity.Campus_Name.Contains(keyword))).ToListAsync();

            return result;
        }

    }
}
