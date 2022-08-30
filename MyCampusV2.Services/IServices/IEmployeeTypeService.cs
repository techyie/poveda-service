using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{ 
    public interface IEmployeeTypeService
    {
        Task<IList<empTypeEntity>> GetEmployeeTypesUsingCampusId(int id);
        Task<empTypeEntity> GetEmployeeTypeById(int id);
        Task<ResultModel> AddEmployeeType(empTypeEntity employeeType);
        Task<ResultModel> UpdateEmployeeType(empTypeEntity employeeType);
        Task<ResultModel> DeleteEmployeeTypePermanent(empTypeEntity employeeType);
        Task<ResultModel> DeleteEmployeeTypeTemporary(empTypeEntity employeeType);
        Task<ResultModel> RetrieveEmployeeType(empTypeEntity employeeType);
        Task<employeeTypePagedResult> GetAllEmployeeType(int pageNo, int pageSize, string keyword);
        Task<IList<empTypeEntity>> GetEmployeeTypes();
    }
}
