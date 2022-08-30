using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace MyCampusV2.DAL.Repositories
{
    public class YearSectionRepository : BaseRepository<yearSectionEntity>, IYearSectionRepository
    {
        private readonly MyCampusCardContext context;

        public YearSectionRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public IQueryable<yearSectionEntity> GetYearSections()
        {
            try
            {
                return _context.YearSectionEntity
                    .Include(a => a.EducationalLevelEntity)
                    .ThenInclude(b => b.CampusEntity)
                    .Where(c => c.ToDisplay == true);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IList<yearSectionEntity>> GetYearSectionsUsingEducationalLevelId(int id) 
        {
            try
            {
                return await this.context.YearSectionEntity
                    .Include(q => q.EducationalLevelEntity)
                    .ThenInclude(x => x.CampusEntity)
                    .Where(c => c.Level_ID == id && c.IsActive == true && c.ToDisplay == true).ToListAsync();
            }
            catch(Exception err)
            {
                return null;
            }
        }

        public async Task<yearSectionEntity> GetYearSectionById(int id)
        {
            try
            {
                return await _context.YearSectionEntity
                    .Include(q => q.EducationalLevelEntity)
                    .ThenInclude(x => x.CampusEntity)
                    .Where(c => c.YearSec_ID == id && c.ToDisplay == true).SingleOrDefaultAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<yearSectionPagedResult> GetAllYearSection(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new yearSectionPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = _context.YearSectionEntity.Where(a => a.ToDisplay == true).Count();
                else
                    result.RowCount = _context.YearSectionEntity
                        .Include(a => a.EducationalLevelEntity)
                        .ThenInclude(b => b.CampusEntity)
                        .Where(c => c.ToDisplay == true && (c.YearSec_Name.Contains(keyword)
                        || c.EducationalLevelEntity.Level_Name.Contains(keyword)
                        || c.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.yearSections = await _context.YearSectionEntity
                        .Include(a => a.EducationalLevelEntity)
                        .ThenInclude(b => b.CampusEntity)
                        .Where(c => c.ToDisplay == true)
                        .OrderByDescending(d => d.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.yearSections = await _context.YearSectionEntity
                        .Include(a => a.EducationalLevelEntity)
                        .ThenInclude(b => b.CampusEntity)
                        .Where(c => c.ToDisplay == true && (c.YearSec_Name.Contains(keyword)
                        || c.EducationalLevelEntity.Level_Name.Contains(keyword)
                        || c.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<yearSectionPagedResult> ExportAllYearSections(string keyword)
        {
            try
            {
                var result = new yearSectionPagedResult();

                if (keyword == null || keyword == "") 
                {
                    result.yearSections = await _context.YearSectionEntity
                        .Include(q => q.EducationalLevelEntity)
                        .ThenInclude(x => x.CampusEntity)
                        .Where(b => b.ToDisplay == true)
                        .OrderBy(e => e.YearSec_ID).ToListAsync();
                }
                else
                {
                    result.yearSections = await _context.YearSectionEntity
                        .Include(a => a.EducationalLevelEntity)
                        .ThenInclude(b => b.CampusEntity)
                        .Where(c => c.ToDisplay == true && (c.YearSec_Name.Contains(keyword)
                            || c.EducationalLevelEntity.Level_Name.Contains(keyword)
                            || c.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)))
                       .OrderBy(e => e.YearSec_ID).ToListAsync();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Boolean> UpdateYearSectionWithBoolReturn(yearSectionEntity yearsection, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.YearSectionEntity.SingleOrDefault(x => x.YearSec_ID == yearsection.YearSec_ID);

                    if (result != null)
                    {
                        result.YearSec_Name = yearsection.YearSec_Name;
                        result.Level_ID = yearsection.Level_ID;

                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;
                    }
                    _context.Entry(result).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(result).Property(x => x.Added_By).IsModified = false;
                    _context.Entry(result).Property(x => x.Last_Updated).IsModified = true;
                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public async Task<Boolean> AddYearSectionWithBoolReturn(yearSectionEntity yearsection, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    yearsection.Added_By = user;
                    yearsection.Date_Time_Added = DateTime.Now;
                    yearsection.Last_Updated = DateTime.Now;
                    yearsection.Updated_By = user;

                    await _context.YearSectionEntity.AddAsync(yearsection);

                    await _context.SaveChangesAsync();

                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return false;
                }
            }
            return true;
        }
    }
}
