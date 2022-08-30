using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace MyCampusV2.DAL.IRepositories
{
    public interface IEmployeeTypeRepository : IBaseRepository<empTypeEntity>
    {
        Task<IList<empTypeEntity>> GetEmployeeTypesUsingCampusId(int id);
        Task<empTypeEntity> GetEmployeeTypeById(int id);
        Task<IList<empTypeEntity>> GetEmployeeTypes();
        Task<employeeTypePagedResult> GetAllEmployeeType(int pageNo, int pageSize, string keyword);
    }
}