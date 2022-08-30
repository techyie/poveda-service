using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IGroupRepository : IBaseRepository<fetcherGroupEntity>
    {
        Task<fetcherGroupPagedResult> GetAllGroups(int pageNo, int pageSize, string keyword);
        IQueryable<fetcherGroupEntity> GetGroups();
        Task<fetcherGroupEntity> GetGroupByID(int id);
    }
}
