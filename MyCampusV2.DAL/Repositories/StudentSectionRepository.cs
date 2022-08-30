using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
    public class StudentSectionRepository : BaseRepository<studentSectionEntity>, IStudentSectionRepository
    {
        private readonly MyCampusCardContext context;

        public StudentSectionRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<IList<studentSectionEntity>> GetStudentSectionsUsingYearSectionId(int id)
        {
            try
            {
                return await this.context.StudentSectionEntity
                        .Include(x => x.YearSectionEntity)
                        .ThenInclude(c => c.EducationalLevelEntity)
                        .ThenInclude(v => v.CampusEntity)
                        .Where(z => z.YearSec_ID == id && z.IsActive == true && z.ToDisplay == true).ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<studentSectionEntity> GetStudentSectionById(int id)
        {
            try
            {
                return await this.context.StudentSectionEntity
                        .Include(x => x.YearSectionEntity)
                        .ThenInclude(c => c.EducationalLevelEntity)
                        .ThenInclude(v => v.CampusEntity)
                        .Where(z => z.StudSec_ID == id).SingleOrDefaultAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }


        public async Task<studentSecPagedResult> GetAllStudentSection(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new studentSecPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.StudentSectionEntity.Where(q => q.IsActive == true).Count();
                else
                    result.RowCount = this.context.StudentSectionEntity
                        .Include(x => x.YearSectionEntity)
                        .ThenInclude(q => q.EducationalLevelEntity)
                        .ThenInclude(q => q.CampusEntity)
                        .Where(q => q.ToDisplay == true
                        && (q.Description.Contains(keyword)
                        || q.YearSectionEntity.YearSec_Name.Contains(keyword)
                        || q.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                        || q.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;


                List<studentSectionEntity> test = await this.context.StudentSectionEntity.ToListAsync();

                if (keyword == null || keyword == "")
                    result.studentSections = await this.context.StudentSectionEntity
                        .Include(x => x.YearSectionEntity)
                        .ThenInclude(q => q.EducationalLevelEntity)
                        .ThenInclude(q => q.CampusEntity)
                        .Where(q => q.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.studentSections = await _context.StudentSectionEntity
                        .Include(x => x.YearSectionEntity)
                        .ThenInclude(q => q.EducationalLevelEntity)
                        .ThenInclude(q => q.CampusEntity)
                        .Where(q => q.ToDisplay == true
                        && (q.Description.Contains(keyword)
                        || q.YearSectionEntity.YearSec_Name.Contains(keyword)
                        || q.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                        || q.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<Boolean> UpdateWithBoolReturn(studentSectionEntity section, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.StudentSectionEntity.SingleOrDefault(x => x.StudSec_ID == section.StudSec_ID);

                    if (result != null)
                    {
                        result.Description = section.Description;
                        result.Start_Time = section.Start_Time;
                        result.End_Time = section.End_Time;
                        result.Half_Day = section.Half_Day;
                        result.Grace_Period = section.Grace_Period;
                        result.YearSec_ID = section.YearSec_ID;
                        result.IsActive = true;

                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;
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

        public async Task<Boolean> AddWithBoolReturn(studentSectionEntity section, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    section.Added_By = user;
                    section.Date_Time_Added = DateTime.Now;
                    section.Last_Updated = DateTime.Now;
                    section.Updated_By = user;
                    section.ToDisplay = true;

                    await _context.StudentSectionEntity.AddAsync(section);

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
