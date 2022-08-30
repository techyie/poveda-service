using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace MyCampusV2.DAL.IRepositories
{
    public interface IEmployeeSubTypeRepository : IBaseRepository<employeeSubTypeEntity>
    {
        Task<IList<employeeSubTypeEntity>> GetEmployeeSubTypesUsingEmployeeTypeId(int id);
        Task<employeeSubTypeEntity> GetEmployeeSubTypeById(int id);
        Task<IList<employeeSubTypeEntity>> GetEmployeeSubTypes();
        Task<employeeSubTypePagedResult> GetAllEmployeeSubType(int pageNo, int pageSize, string keyword);
    }
}