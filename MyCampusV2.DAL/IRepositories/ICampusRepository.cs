using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface ICampusRepository : IBaseRepository<campusEntity>
    {
        Task<campusPagedResult> GetAllCampuses(int pageNo, int pageSize, string keyword);
        Task<ICollection<regionEntity>> GetRegion();
        Task<ICollection<divisionEntity>> GetDivisionByRegion(int id);
        Task<campusEntity> GetCampusByID(int id);
        IQueryable<campusEntity> GetCampuses();
        Task<ICollection<educationalLevelEntity>> GetCampusIfActive(int id);
        Task<Boolean> UpdateWithBoolReturn(campusEntity campus, int user);
        Task<Boolean> AddWithBoolReturn(campusEntity campus, int user);
    }
}
