using System;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IDepartmentRepository : IBaseRepository<departmentEntity>
    {
        Task<IList<departmentEntity>> GetDepartmentsUsingCampusId(int id);
        Task<departmentEntity> GetDepartmentById(int id);
        Task<departmentPagedResult> GetAllDepartment(int pageNo, int pageSize, string keyword);
        Task<departmentPagedResult> ExportAllDepartments(string keyword);
        Task<Boolean> UpdateDepartmentWithBoolReturn(departmentEntity department, int user);
        Task<Boolean> AddDepartmentWithBoolReturn(departmentEntity department, int user);
    }
}
