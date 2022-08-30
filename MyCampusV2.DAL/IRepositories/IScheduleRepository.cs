using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using System.Threading.Tasks;
using System.Linq;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IScheduleRepository : IBaseRepository<scheduleEntity>
    {
        Task<schedulePagedResult> GetAllSchedule(int pageNo, int pageSize, string keyword);
        Task<ICollection<scheduleEntity>> GetSchedules();
        Task<scheduleEntity> GetScheduleByID(int id);
        Task<fetcherSchedulePagedResult> GetScheduleByFetcherID(string id, int pageNo, int pageSize, string keyword);
        Task<fetcherScheduleDetailsPagedResult> GetStudentByFetcherScheduleID(string id, int pageNo, int pageSize, string keyword);
        Task<fetcherScheduleDetailsPagedResultVM> GetGroupByFetcherScheduleID(string id, int pageNo, int pageSize, string keyword);
        Task<fetcherScheduleEntity> GetFetcherScheduleByID(int id);
        Task<List<fetcherScheduleDetailsEntity>> GetFetcherScheduleGroup(int id);
        Task<fetcherScheduleEntity> GetFetcherSchedule(int fetcherId, int scheduleId);
        Task AddFetcherSchedule(fetcherScheduleEntity entity);
        Task DeleteFetcherSchedule(fetcherScheduleEntity entity, int user);
        Task<fetcherScheduleDetailsEntity> GetFetcherScheduleStudent(int scheduleId, int groupId, int personId);
        Task<fetcherScheduleDetailsEntity> GetFetcherScheduleStudentByID(int id);
        Task AddFetcherScheduleStudent(fetcherScheduleDetailsEntity entity);
        Task DeleteFetcherScheduleStudent(fetcherScheduleDetailsEntity entity, int user);
        Task DeleteFetcherScheduleGroup(List<fetcherScheduleDetailsEntity> entity, int user);
        IQueryable<fetcherGroupDetailsEntity> GetGroupsByScheduleGroupId(int groupId);
        IQueryable<fetcherScheduleDetailsEntity> GetFetcherScheduleStudentByGroupIDAndPersonID(int groupId, int personId);
        IQueryable<fetcherScheduleDetailsEntity> GetFetcherScheduleStudentByGroupID(int groupId);
    }
}
