using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.IServices
{
    public interface ICourseService
    {
        Task<IList<courseEntity>> GetCoursesUsingCollegeId(int id);
        Task<courseEntity> GetCourseById(int id);
        Task<ResultModel> AddCourse(courseEntity course);
        Task<ResultModel> UpdateCourse(courseEntity course);
        Task<ResultModel> DeleteCoursePermanent(int id, int user);
        Task<ResultModel> DeleteCourseTemporary(int id, int user);
        Task<ResultModel> RetrieveCourse(courseEntity course);
        Task<coursePagedResult> GetAllCourse(int pageNo, int pageSize, string keyword);
        Task<coursePagedResult> ExportAllCourses(string keyword);
        Task<BatchUploadResponse> BatchUpload(ICollection<courseBatchUploadVM> courses, int user, int uploadID, int row);
    }
}
