using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace MyCampusV2.Services.IServices
{
    public interface IGroupDetailsService
    {
        Task<ResultModel> AddStudentToGroup(fetcherGroupDetailsEntity entity);
        Task<ResultModel> DeleteGroup(int id, int personId, int user);
        Task<fetcherGroupDetailsPagedResult> GetAllStudentAssignedToGroup(int pageNo, int pageSize, string keyword, int groupId);
        Task<fetcherGroupDetailsEntity> GetGroupDetailById(int id, int personId);
    }
}
