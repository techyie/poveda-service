using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories.Report;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class SectionReportRepository : BaseReportRepository<studentSectionEntity>, ISectionReportRepository
    {
        private readonly MyCampusCardReportContext context;

        public SectionReportRepository(MyCampusCardReportContext Context)
                : base(Context)
        {
            this.context = Context;
        }

        public async Task<studentSecPagedResult> ExportSection(string keyword)
        {
            var result = new studentSecPagedResult();

            if (keyword == null || keyword == "")
                result.studentSections = await _context.StudentSectionEntity
                    .Include(x => x.YearSectionEntity)
                    .ThenInclude(q => q.EducationalLevelEntity)
                    .ThenInclude(q => q.CampusEntity)
                    .Where(q => q.ToDisplay == true)
                    .OrderByDescending(c => c.Last_Updated).ToListAsync();
            else
                result.studentSections = await _context.StudentSectionEntity
                    .Include(x => x.YearSectionEntity)
                    .ThenInclude(q => q.EducationalLevelEntity)
                    .ThenInclude(q => q.CampusEntity)
                    .Where(q => q.ToDisplay == true &&
                    (q.Description.Contains(keyword)
                    || q.YearSectionEntity.YearSec_Name.Contains(keyword)
                    || q.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                    || q.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)))
                    .OrderByDescending(c => c.Last_Updated).ToListAsync();

            return result;

        }
    }
}