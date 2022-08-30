
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface IEmployeeSubTypeService
    {
        Task<IList<employeeSubTypeEntity>> GetEmployeeSubTypesUsingEmployeeTypeId(int id);
        Task<employeeSubTypeEntity> GetEmployeeSubTypeById(int id);
        Task<ResultModel> AddEmployeeSubType(employeeSubTypeEntity employeeSubType);
        Task<ResultModel> UpdateEmployeeSubType(employeeSubTypeEntity employeeSubType);
        Task<ResultModel> DeleteEmployeeSubTypePermanent(employeeSubTypeEntity employeeSubType);
        Task<ResultModel> DeleteEmployeeSubTypeTemporary(employeeSubTypeEntity employeeSubType);
        Task<ResultModel> RetrieveEmployeeSubType(employeeSubTypeEntity employeeSubType);
        Task<employeeSubTypePagedResult> GetAllEmployeeSubType(int pageNo, int pageSize, string keyword);
        Task<IList<employeeSubTypeEntity>> GetEmployeeSubTypes();
    }
}