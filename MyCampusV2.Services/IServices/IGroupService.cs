using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface IGroupService
    {
        Task<ResultModel> AddGroup(fetcherGroupEntity entity);
        Task<ResultModel> UpdateGroup(fetcherGroupEntity entity);
        Task<ResultModel> DeleteGroupPermanent(int id, int user);
        Task<ResultModel> DeleteGroupTemporary(int id, int user);
        Task<fetcherGroupPagedResult> GetAllGroups(int pageNo, int pageSize, string keyword);
        Task<ICollection<fetcherGroupEntity>> GetGroups();
        Task<fetcherGroupEntity> GetGroupByID(int id);
    }
}
