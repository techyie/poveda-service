using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IOfficeRepository : IBaseRepository<officeEntity>
    {
        Task<IList<officeEntity>> GetOfficesUsingCampusId(int id);
        Task<officeEntity> GetOfficeById(int id);
        Task<officePagedResult> GetAllOffice(int pageNo, int pageSize, string keyword);
        Task<Boolean> UpdateWithBoolReturn(officeEntity office, int user);
        Task<Boolean> AddWithBoolReturn(officeEntity office, int user);
    }
}
