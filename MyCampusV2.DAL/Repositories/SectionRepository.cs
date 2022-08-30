using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class SectionRepository : BaseRepository<sectionEntity>, ISectionRepository
    {
        private readonly MyCampusCardContext context;

        public SectionRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<sectionEntity> GetByID(int id)
        {
            return await _context.SectionEntity.Include(x => x.YearLevelEntity).ThenInclude(q => q.EducationalLevelEntity).Where(c => c.Section_ID == id).FirstOrDefaultAsync();
        }
        public async Task<ICollection<sectionEntity>> GetByYearLevel(int id)
        {
            return await _context.SectionEntity.Include(x => x.YearLevelEntity).ThenInclude(v => v.EducationalLevelEntity).ThenInclude(g => g.CampusEntity).Where(c => c.Year_Level_ID == id && c.IsActive == true).ToListAsync();
        }

        public IQueryable<sectionEntity> GetAllSection()
        {
            return _context.SectionEntity.Include(x => x.YearLevelEntity).ThenInclude(c => c.EducationalLevelEntity).ThenInclude(z => z.CampusEntity);
        }

        public Task<sectionEntity> FindData(string section, long yearlevelid)
        {
            return _context.SectionEntity.Include(q => q.YearLevelEntity)
                .Where(x => x.Section_Name == section && x.Year_Level_ID == yearlevelid && x.IsActive == true).FirstOrDefaultAsync();
        }

        public async Task<studentSecPagedResult> GetAllSectionList(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new studentSecPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = _context.SectionEntity.Count();
                else
                    result.RowCount = _context.SectionEntity
                        .Include(b => b.YearLevelEntity)
                        .ThenInclude(h => h.EducationalLevelEntity)
                        .ThenInclude(q => q.CampusEntity)
                        .Where(q =>
                        q.Section_Name.Contains(keyword)
                        || q.YearLevelEntity.Year_Level_Name.Contains(keyword)
                        || q.YearLevelEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                        || q.YearLevelEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.sections = await _context.SectionEntity
                        .Include(b => b.YearLevelEntity)
                        .ThenInclude(h => h.EducationalLevelEntity)
                        .ThenInclude(q => q.CampusEntity)
                        .OrderBy(c => c.Section_Name)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.sections = await _context.SectionEntity
                        .Include(b => b.YearLevelEntity)
                        .ThenInclude(h => h.EducationalLevelEntity)
                        .ThenInclude(q => q.CampusEntity)
                        .Where(q =>
                        q.Section_Name.Contains(keyword)
                        || q.YearLevelEntity.Year_Level_Name.Contains(keyword)
                        || q.YearLevelEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                        || q.YearLevelEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword))
                        .OrderBy(c => c.Section_Name)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ICollection<sectionEntity>> DuplicateRecordChecker(string name, string desc, int yearid, int educid, int campusid)
        {
            return await _context.SectionEntity
                .Include(q => q.YearLevelEntity)
                .ThenInclude(q => q.EducationalLevelEntity)
                .Where(section => section.Section_Name == name 
                 && section.YearLevelEntity.Year_Level_ID == yearid
                 && section.YearLevelEntity.Level_ID == educid
                 && section.YearLevelEntity.EducationalLevelEntity.Campus_ID == campusid
                 && section.IsActive == true).ToListAsync();
        }

        public async Task<ICollection<educationalLevelEntity>> CheckboxChecker(int value1, int value2)
        {
            return await _context.EducationalLevelEntity.Where(educlevel => educlevel.Campus_ID == value1 && educlevel.Level_ID == value2).ToListAsync();
        }

        public async Task<ICollection<yearLevelEntity>> CheckboxCheckerOne(int value1, int value2)
        {
            return await _context.YearLevelEntity.Where(yearlevel => yearlevel.Level_ID == value1 && yearlevel.Year_Level_ID == value2).ToListAsync();
        }

        public async Task<ICollection<personEntity>> GetCountSectionIfActive(int id)
        {
            return await _context.PersonEntity.Where(section => section.StudSec_ID == id && section.IsActive == true).ToListAsync();
        }
    }
}
