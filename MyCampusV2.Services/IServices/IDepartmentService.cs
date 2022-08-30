using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.IServices
{
    public interface IDepartmentService
    {
        Task<IList<departmentEntity>> GetDepartmentsUsingCampusId(int id);
        Task<departmentEntity> GetDepartmentById(int id);
        Task<ResultModel> AddDepartment(departmentEntity department);
        Task<ResultModel> UpdateDepartment(departmentEntity department);
        Task<ResultModel> DeleteDepartmentPermanent(int id, int user);
        Task<ResultModel> DeleteDepartmentTemporary(int id, int user);
        Task<ResultModel> RetrieveDepartment(departmentEntity department);
        Task<departmentPagedResult> GetAllDepartment(int pageNo, int pageSize, string keyword);
        Task<departmentPagedResult> ExportAllDepartments(string keyword);
        Task<BatchUploadResponse> BatchUpload(ICollection<departmentBatchUploadVM> departments, int user, int uploadID, int row);
    }
}
