using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace MyCampusV2.DAL.Repositories
{
    public class CollegeRepository : BaseRepository<collegeEntity>, ICollegeRepository
    {
        private readonly MyCampusCardContext context;

        public CollegeRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<IList<collegeEntity>> GetCollegesUsingEducationalLevelId(int id)
        {
            try
            {
                return await this.context.CollegeEntity
                    .Include(q => q.EducationalLevelEntity)
                    .ThenInclude(x => x.CampusEntity).Where(c => c.Level_ID == id && c.IsActive == true && c.ToDisplay == true).ToListAsync();
            }
            catch(Exception err)
            {
                return null;
            }
        }
        public async Task<collegeEntity> GetCollegeById(int id)
        {
            try
            {
                return await this.context.CollegeEntity
                    .Include(q => q.EducationalLevelEntity)
                    .ThenInclude(x => x.CampusEntity).Where(c => c.College_ID == id).SingleOrDefaultAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<collegePagedResult> GetAllCollege(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new collegePagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = _context.CollegeEntity
                        .Where(a => a.ToDisplay == true)
                        .Count();
                else
                    result.RowCount = _context.CollegeEntity
                        .Include(x => x.EducationalLevelEntity)
                        .ThenInclude(q => q.CampusEntity)
                        .Where(q => q.ToDisplay == true && (q.College_Name.Contains(keyword)
                        || q.EducationalLevelEntity.Level_Name.Contains(keyword)
                        || q.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.colleges = await _context.CollegeEntity
                        .Include(x => x.EducationalLevelEntity)
                        .ThenInclude(q => q.CampusEntity)
                        .Where(a => a.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.colleges = await _context.CollegeEntity
                        .Include(x => x.EducationalLevelEntity)
                        .ThenInclude(q => q.CampusEntity)
                        .Where(q => q.ToDisplay == true && (q.College_Name.Contains(keyword)
                        || q.EducationalLevelEntity.Level_Name.Contains(keyword)
                        || q.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<collegePagedResult> ExportAllColleges(string keyword)
        {
            try
            {
                var result = new collegePagedResult();

                if (keyword == null || keyword == "")
                {
                    result.colleges = await _context.CollegeEntity
                        .Include(q => q.EducationalLevelEntity)
                        .ThenInclude(x => x.CampusEntity)
                        .Where(b => b.ToDisplay == true)
                        .OrderBy(e => e.College_ID).ToListAsync();
                }
                else
                {
                    result.colleges = await _context.CollegeEntity
                        .Include(a => a.EducationalLevelEntity)
                        .ThenInclude(b => b.CampusEntity)
                        .Where(c => c.ToDisplay == true && (c.College_Name.Contains(keyword)
                            || c.EducationalLevelEntity.Level_Name.Contains(keyword)
                            || c.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)))
                       .OrderBy(e => e.College_ID).ToListAsync();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Boolean> UpdateCollegeWithBoolReturn(collegeEntity college, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.CollegeEntity.SingleOrDefault(x => x.College_ID == college.College_ID);

                    if (result != null)
                    {
                        result.College_Name = college.College_Name;
                        result.Level_ID = college.Level_ID;

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

        public async Task<Boolean> AddCollegeWithBoolReturn(collegeEntity college, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    college.Added_By = user;
                    college.Date_Time_Added = DateTime.Now;
                    college.Last_Updated = DateTime.Now;
                    college.Updated_By = user;

                    await _context.CollegeEntity.AddAsync(college);

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
