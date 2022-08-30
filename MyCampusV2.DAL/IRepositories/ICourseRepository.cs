using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface ICourseRepository : IBaseRepository<courseEntity>
    {
        Task<IList<courseEntity>> GetCoursesUsingCollegeId(int id);
        Task<courseEntity> GetCourseById(int id);
        Task<coursePagedResult> GetAllCourse(int pageNo, int pageSize, string keyword);
        Task<coursePagedResult> ExportAllCourses(string keyword);
        Task<Boolean> UpdateCourseWithBoolReturn(courseEntity course, int user);
        Task<Boolean> AddCourseWithBoolReturn(courseEntity course, int user);
    }
}
