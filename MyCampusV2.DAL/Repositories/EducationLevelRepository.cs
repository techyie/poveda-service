using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class EducationLevelRepository : BaseRepository<educationalLevelEntity>, IEducationLevelRepository
    {
        private readonly MyCampusCardContext context;

        public EducationLevelRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<IList<educationalLevelEntity>> GetEducationalLevelsUsingCampusId(int id)
        {
            try
            {
                return await this.context.EducationalLevelEntity.Include(q => q.CampusEntity).Where(x => x.Campus_ID == id && x.IsActive == true && x.ToDisplay == true).ToListAsync();
            }
            catch(Exception err)
            {
                return null;
            }
        }

        public async Task<IList<educationalLevelEntity>> GetEducationalLevelsCollegeOnlyUsingCampusId(int id)
        {
            try
            {
                return await this.context.EducationalLevelEntity.Include(q => q.CampusEntity).Where(x => x.Campus_ID == id && x.hasCourse == true && x.IsActive == true && x.ToDisplay == true).ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<educationalLevelEntity> GetEducationalLevelById(int id)
        {
            try
            {
                return await this.context.EducationalLevelEntity.Include(q => q.CampusEntity).Where(x => x.Level_ID == id).SingleOrDefaultAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<educlevelPagedResult> GetAllEduclevel(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new educlevelPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.EducationalLevelEntity.Where(q => q.ToDisplay == true).Count();

                else if (keyword.ToLower().Contains("yes") || keyword.ToLower().Contains("no"))
                    result.RowCount = this.context.EducationalLevelEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.hasCourse.Equals(keyword.ToLower().Contains("yes") ? true : false) && q.ToDisplay == true).Count();

                else if (keyword.ToLower().Contains("inactive") || keyword.ToLower().Contains("active"))
                    result.RowCount = this.context.EducationalLevelEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.IsActive.Equals(keyword.ToLower().Contains("inactive") ? false : true) && q.ToDisplay == true).Count();

                else
                    result.RowCount = _context.EducationalLevelEntity
                        .Include(x => x.CampusEntity)
                        .Where(q =>  q.ToDisplay == true &&
                        (q.Level_Name.Contains(keyword)
                        || q.CampusEntity.Campus_Name.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.educlevels = await _context.EducationalLevelEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                else if (keyword.ToLower().Contains("yes") || keyword.ToLower().Contains("no"))
                    result.educlevels = await _context.EducationalLevelEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.hasCourse.Equals(keyword.ToLower().Contains("yes") ? true : false) && q.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                else if (keyword.ToLower().Contains("inactive") || keyword.ToLower().Contains("active"))
                    result.educlevels = await _context.EducationalLevelEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.IsActive.Equals(keyword.ToLower().Contains("inactive") ? false : true) && q.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                else
                    result.educlevels = await _context.EducationalLevelEntity
                        .Include(x => x.CampusEntity)
                        .Where(q =>  q.ToDisplay == true && 
                        (q.Level_Name.Contains(keyword)
                        || q.CampusEntity.Campus_Name.Contains(keyword)))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Boolean> UpdateWithBoolReturn(educationalLevelEntity educLevel, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.EducationalLevelEntity.SingleOrDefault(x => x.Level_ID == educLevel.Level_ID);

                    if (result != null)
                    {
                        result.Level_Status = educLevel.Level_Status.ToUpper() == "ACTIVE" ? "Active" : "Inactive";
                        result.Campus_ID = educLevel.Campus_ID;

                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;
                        result.IsActive = result.Level_Status.ToUpper() == "ACTIVE" ? true : false;
                        result.ToDisplay = true;
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

        public async Task<Boolean> AddWithBoolReturn(educationalLevelEntity educLevel, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    educLevel.Added_By = user;
                    educLevel.Date_Time_Added = DateTime.Now;
                    educLevel.Last_Updated = DateTime.Now;
                    educLevel.Updated_By = user;
                    educLevel.Level_Status = educLevel.Level_Status.ToUpper() == "ACTIVE" ? "Active" : "Inactive";

                    educLevel.IsActive = educLevel.Level_Status.ToUpper() == "ACTIVE" ? true : false;
                    educLevel.ToDisplay = true;

                    await _context.EducationalLevelEntity.AddAsync(educLevel);

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
