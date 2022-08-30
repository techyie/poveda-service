using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace MyCampusV2.DAL.Repositories
{
    public class PersonReportRepository : BaseReportRepository<personEntity>, IPersonReportRepository
    {
        private readonly MyCampusCardReportContext context;

        public PersonReportRepository(MyCampusCardReportContext Context) : base(Context)
        {
            context = Context;
        }
        
        public async Task<studentPagedResult> ExportAllStudents(string keyword, bool isCollege)
        {
            try
            {
                var result = new studentPagedResult();

                if (isCollege)
                {
                    if (keyword == null || keyword == "")
                    {
                        result.students = await _context.PersonEntity
                            .Include(a => a.StudentSectionEntity)
                                .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Include(b => b.StudentSectionEntity.YearSectionEntity)
                            .Include(c => c.EmergencyContactEntity)
                            .Include(d => d.CourseEntity)
                                .ThenInclude(y => y.CollegeEntity)
                                .ThenInclude(z => z.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Where(x => (x.CourseEntity.CollegeEntity.EducationalLevelEntity.hasCourse.Equals(true)))
                            .OrderBy(e => e.ID_Number).ToListAsync();
                    }
                    else if (keyword.Contains(","))
                    {
                        string[] fullname = keyword.Split(",");
                        result.students = await _context.PersonEntity
                            .Include(a => a.StudentSectionEntity)
                                .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Include(b => b.StudentSectionEntity.YearSectionEntity)
                            .Include(c => c.EmergencyContactEntity)
                            .Include(d => d.CourseEntity)
                                .ThenInclude(y => y.CollegeEntity)
                                .ThenInclude(z => z.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Where(x => ((x.ID_Number.Contains(keyword)
                                || (x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim()))
                                || x.CourseEntity.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.CourseEntity.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.YearSec_Name.Contains(keyword)
                                || x.CourseEntity.Course_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.StudentSectionEntity.Description.Contains(keyword))
                                && x.CourseEntity.CollegeEntity.EducationalLevelEntity.hasCourse.Equals(true)))
                            .OrderBy(e => e.ID_Number).ToListAsync();
                    }
                    else
                    {
                        result.students = await _context.PersonEntity
                            .Include(a => a.StudentSectionEntity)
                                .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Include(b => b.StudentSectionEntity.YearSectionEntity)
                            .Include(c => c.EmergencyContactEntity)
                            .Include(d => d.CourseEntity)
                                .ThenInclude(y => y.CollegeEntity)
                                .ThenInclude(z => z.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .OrderBy(e => e.ID_Number)
                            .Where(x => ((x.ID_Number.Contains(keyword)
                                || x.First_Name.Contains(keyword)
                                || x.Last_Name.Contains(keyword)
                                || x.CourseEntity.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.CourseEntity.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.YearSec_Name.Contains(keyword)
                                || x.CourseEntity.Course_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.StudentSectionEntity.Description.Contains(keyword))
                                && x.CourseEntity.CollegeEntity.EducationalLevelEntity.hasCourse.Equals(true))).ToListAsync();
                    }
                    return result;
                }
                else
                {
                    if (keyword == null || keyword == "")
                    {
                        result.students = await _context.PersonEntity
                            .Include(a => a.StudentSectionEntity)
                                .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Include(b => b.StudentSectionEntity.YearSectionEntity)
                            .Include(c => c.EmergencyContactEntity)
                            .Include(d => d.CourseEntity)
                                .ThenInclude(y => y.CollegeEntity)
                                .ThenInclude(z => z.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Where(x => (x.CourseEntity.CollegeEntity.EducationalLevelEntity.hasCourse.Equals(false)))
                            .OrderBy(e => e.ID_Number).ToListAsync();
                    }
                    else if (keyword.Contains(","))
                    {
                        string[] fullname = keyword.Split(",");
                        result.students = await _context.PersonEntity
                            .Include(a => a.StudentSectionEntity)
                                .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Include(b => b.StudentSectionEntity.YearSectionEntity)
                            .Include(c => c.EmergencyContactEntity)
                            .Include(d => d.CourseEntity)
                                .ThenInclude(y => y.CollegeEntity)
                                .ThenInclude(z => z.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Where(x => ((x.ID_Number.Contains(keyword)
                                || (x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim()))
                                || x.CourseEntity.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.CourseEntity.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.YearSec_Name.Contains(keyword)
                                || x.CourseEntity.Course_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.StudentSectionEntity.Description.Contains(keyword))
                                && x.CourseEntity.CollegeEntity.EducationalLevelEntity.hasCourse.Equals(false)))
                            .OrderBy(e => e.ID_Number).ToListAsync();
                    }
                    else
                    {
                        result.students = await _context.PersonEntity
                            .Include(a => a.StudentSectionEntity)
                                .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Include(b => b.StudentSectionEntity.YearSectionEntity)
                            .Include(c => c.EmergencyContactEntity)
                            .Include(d => d.CourseEntity)
                                .ThenInclude(y => y.CollegeEntity)
                                .ThenInclude(z => z.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .OrderBy(e => e.ID_Number)
                            .Where(x => ((x.ID_Number.Contains(keyword)
                                || x.First_Name.Contains(keyword)
                                || x.Last_Name.Contains(keyword)
                                || x.CourseEntity.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.CourseEntity.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.YearSec_Name.Contains(keyword)
                                || x.CourseEntity.Course_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.StudentSectionEntity.Description.Contains(keyword))
                                && x.CourseEntity.CollegeEntity.EducationalLevelEntity.hasCourse.Equals(false))).ToListAsync();
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public async Task<employeePagedResult> ExportAllEmployees(string keyword)
        {
            try
            {
                var result = new employeePagedResult();

                if (keyword == null || keyword == "")
                {
                    result.employees = await _context.PersonEntity
                    .Include(a => a.EmergencyContactEntity)
                    .Include(a => a.GovIdsEntity)
                    .Include(x => x.PositionEntity)
                    .ThenInclude(c => c.DepartmentEntity)
                    .ThenInclude(v => v.CampusEntity)
                    .OrderBy(e => e.ID_Number).ToListAsync();
                }
                else if (keyword.Contains(","))
                {
                    string[] fullname = keyword.Split(",");
                    result.employees = await _context.PersonEntity
                    .Include(x => x.PositionEntity)
                    .ThenInclude(c => c.DepartmentEntity)
                    .ThenInclude(v => v.CampusEntity)
                        .Where(x => (x.ID_Number.Contains(keyword)
                            || (x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim()))
                            || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
                            || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                            || x.PositionEntity.Position_Name.Contains(keyword)))
                        .OrderBy(e => e.ID_Number).ToListAsync();
                }
                else
                {
                    if (RemoveSpecialChar(keyword.Trim()).ToLower() == "teaching")
                    {
                        result.employees = await _context.PersonEntity
                                .Include(x => x.PositionEntity)
                                .ThenInclude(c => c.DepartmentEntity)
                                .ThenInclude(v => v.CampusEntity)
                                    .Where(x => (x.ID_Number.Contains(keyword)
                                        || x.First_Name.Contains(keyword)
                                        || x.Last_Name.Contains(keyword)
                                        || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
                                        || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                                        || x.PositionEntity.Position_Name.Contains(keyword)))
                                    .OrderBy(e => e.ID_Number).ToListAsync();
                    }
                    else if (RemoveSpecialChar(keyword.Trim()).ToLower() == "nonteaching")
                    {
                        result.employees = await _context.PersonEntity
                            .Include(x => x.PositionEntity)
                            .ThenInclude(c => c.DepartmentEntity)
                            .ThenInclude(v => v.CampusEntity)
                                .Where(x => (x.ID_Number.Contains(keyword)
                                    || x.First_Name.Contains(keyword)
                                    || x.Last_Name.Contains(keyword)
                                    || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
                                    || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                                    || x.PositionEntity.Position_Name.Contains(keyword)))
                                .OrderBy(e => e.ID_Number).ToListAsync();
                    }
                    else
                    {
                        result.employees = await _context.PersonEntity
                            .Include(a => a.EmergencyContactEntity)
                            .Include(a => a.GovIdsEntity)
                            .Include(x => x.PositionEntity)
                            .ThenInclude(c => c.DepartmentEntity)
                            .ThenInclude(v => v.CampusEntity)
                            .OrderBy(e => e.ID_Number)
                                .Where(x => (x.ID_Number.Contains(keyword)
                                    || x.First_Name.Contains(keyword)
                                    || x.Last_Name.Contains(keyword)
                                    || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
                                    || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                                    || x.PositionEntity.Position_Name.Contains(keyword))).ToListAsync();
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<visitorPagedResult> ExportAllVisitors(string keyword)
        {
            try
            {
                var result = new visitorPagedResult();

                if (keyword == null || keyword == "")
                    result.personvisitors = await _context.PersonEntity.Where(x => x.Person_Type == "O")
                        .OrderByDescending(x => x.ID_Number).ToListAsync();
                else
                    result.personvisitors = await _context.PersonEntity
                        .Where(x => x.Person_Type == "O" && (x.ID_Number.Contains(keyword)
                            || x.First_Name.Contains(keyword)
                            || x.Last_Name.Contains(keyword)))
                        .OrderBy(e => e.ID_Number).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        private string RemoveSpecialChar(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}