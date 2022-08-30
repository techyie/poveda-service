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
    public class CourseReportRepository : BaseReportRepository<courseEntity>, ICourseReportRepository
    {
        private readonly MyCampusCardReportContext context;

        public CourseReportRepository(MyCampusCardReportContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<coursePagedResult> ExportCourses(string keyword)
        {
            var result = new coursePagedResult();

            if (keyword == null || keyword == "")
                result.courses = await _context.CourseEntity
                    .Include(c => c.CollegeEntity)
                    .ThenInclude(x => x.EducationalLevelEntity)
                    .ThenInclude(q => q.CampusEntity)
                    .OrderBy(c => c.Course_Name).ToListAsync();
            else
                result.courses = await _context.CourseEntity
                    .Include(c => c.CollegeEntity)
                    .ThenInclude(x => x.EducationalLevelEntity)
                    .ThenInclude(q => q.CampusEntity)
                    .OrderBy(c => c.Course_Name)
                    .Where(q =>
                    q.Course_Name.Contains(keyword)
                    || q.CollegeEntity.College_Name.Contains(keyword)
                    || q.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                    || q.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)).OrderByDescending(b => b.Course_ID).ToListAsync();

            return result;
        }

    }

}