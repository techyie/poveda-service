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
    public class CollegeReportRepository : BaseReportRepository<collegeEntity>, ICollegeReportRepository
    {
        private readonly MyCampusCardReportContext context;

        public CollegeReportRepository(MyCampusCardReportContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<collegePagedResult> ExportColleges(string keyword)
        {
            var result = new collegePagedResult();

            if (keyword == null || keyword == "")
                result.colleges = await context.CollegeEntity
                    .Include(x => x.EducationalLevelEntity)
                    .ThenInclude(q => q.CampusEntity)
                    .OrderBy(c => c.College_Name).ToListAsync();
            else
                result.colleges = await _context.CollegeEntity
                    .Include(x => x.EducationalLevelEntity)
                    .ThenInclude(q => q.CampusEntity)
                    .OrderBy(c => c.College_Name)
                    .Where(q =>
                    q.College_Name.Contains(keyword)
                    || q.EducationalLevelEntity.Level_Name.Contains(keyword)
                    || q.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)).OrderByDescending(b => b.College_ID).ToListAsync();

            return result;
        }

    }
}
