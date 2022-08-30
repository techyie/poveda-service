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
    public class CourseRepository : BaseRepository<courseEntity>, ICourseRepository
    {
        private readonly MyCampusCardContext context;

        public CourseRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<IList<courseEntity>> GetCoursesUsingCollegeId(int id)
        {
            try
            {
                return await this.context.CourseEntity
                    .Include(q => q.CollegeEntity)
                    .ThenInclude(x => x.EducationalLevelEntity)
                    .ThenInclude(x => x.CampusEntity)
                    .Where(v => v.College_ID == id && v.IsActive == true && v.ToDisplay == true).ToListAsync();
            }
            catch(Exception err)
            {
                return null;
            }
        }

        public async Task<courseEntity> GetCourseById(int id)
        {
            try
            {
                return await this.context.CourseEntity
                    .Include(q => q.CollegeEntity)
                    .ThenInclude(x => x.EducationalLevelEntity)
                    .ThenInclude(x => x.CampusEntity)
                    .Where(v => v.Course_ID == id).SingleOrDefaultAsync();
            }
            catch(Exception err)
            {
                return null;
            }
        }

        public async Task<coursePagedResult> GetAllCourse(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new coursePagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = _context.CourseEntity
                        .Where(q => q.ToDisplay == true && q.CollegeEntity.EducationalLevelEntity.hasCourse == true)
                        .Count();
                else
                    result.RowCount = _context.CourseEntity
                        .Include(c => c.CollegeEntity)
                        .ThenInclude(x => x.EducationalLevelEntity)
                        .ThenInclude(q => q.CampusEntity)
                        .Where(q => q.ToDisplay == true && q.CollegeEntity.EducationalLevelEntity.hasCourse == true && (q.Course_Name.Contains(keyword)
                        || q.CollegeEntity.College_Name.Contains(keyword)
                        || q.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                        || q.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.courses = await _context.CourseEntity
                        .Include(c => c.CollegeEntity)
                        .ThenInclude(x => x.EducationalLevelEntity)
                        .ThenInclude(q => q.CampusEntity)
                        .Where(y => y.ToDisplay == true && y.CollegeEntity.EducationalLevelEntity.hasCourse == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.courses = await _context.CourseEntity
                        .Include(c => c.CollegeEntity)
                        .ThenInclude(x => x.EducationalLevelEntity)
                        .ThenInclude(q => q.CampusEntity)
                        .Where(q => q.ToDisplay == true && q.CollegeEntity.EducationalLevelEntity.hasCourse == true && (q.Course_Name.Contains(keyword)
                        || q.CollegeEntity.College_Name.Contains(keyword)
                        || q.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                        || q.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<coursePagedResult> ExportAllCourses(string keyword)
        {
            try
            {
                var result = new coursePagedResult();

                if (keyword == null || keyword == "")
                {
                    result.courses = await _context.CourseEntity
                        .Include(a => a.CollegeEntity)
                        .ThenInclude(b => b.EducationalLevelEntity)
                        .ThenInclude(c => c.CampusEntity)
                        .Where(d => d.ToDisplay == true)
                        .OrderBy(e => e.Course_ID).ToListAsync();
                }
                else
                {
                    result.courses = await _context.CourseEntity
                        .Include(a => a.CollegeEntity)
                        .ThenInclude(b => b.EducationalLevelEntity)
                        .ThenInclude(c => c.CampusEntity)
                        .Where(d => d.ToDisplay == true && (d.Course_Name.Contains(keyword)
                            || d.CollegeEntity.College_Name.Contains(keyword)
                            || d.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                            || d.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)))
                       .OrderBy(e => e.Course_ID).ToListAsync();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Boolean> UpdateCourseWithBoolReturn(courseEntity course, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.CourseEntity.SingleOrDefault(x => x.Course_ID == course.Course_ID);

                    if (result != null)
                    {
                        result.Course_Name = course.Course_Name;
                        result.College_ID = course.College_ID;

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

        public async Task<Boolean> AddCourseWithBoolReturn(courseEntity course, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    course.Added_By = user;
                    course.Date_Time_Added = DateTime.Now;
                    course.Last_Updated = DateTime.Now;
                    course.Updated_By = user;

                    await _context.CourseEntity.AddAsync(course);

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
