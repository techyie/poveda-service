using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IGroupDetailsRepository : IBaseRepository<fetcherGroupDetailsEntity>
    {
        Task<fetcherGroupDetailsPagedResult> GetAllStudentAssignedToGroup(int pageNo, int pageSize, string keyword, int groupId);
        Task<fetcherGroupDetailsEntity> GetGroupDetailById(int id, int personId);
    }
}
