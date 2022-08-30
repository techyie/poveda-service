using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyCampusV2.Models.V2.entity;

namespace MyCampusV2.Services.IServices
{
    public interface IScheduleService
    {
        Task<ResultModel> AddSchedule(scheduleEntity scheduleEntity);
        Task<ResultModel> UpdateSchedule(scheduleEntity scheduleEntity);
        Task<ResultModel> DeleteSchedulePermanent(int id, int user);
        Task<ResultModel> DeleteScheduleTemporary(int id, int user);
        Task<ResultModel> RetrieveSchedule(scheduleEntity scheduleEntity);
        Task<schedulePagedResult> GetAllSchedule(int pageNo, int pageSize, string keyword);
        Task<ICollection<scheduleEntity>> GetSchedules();
        Task<scheduleEntity> GetScheduleByID(int id);
        Task<fetcherSchedulePagedResult> GetScheduleByFetcherID(string id, int pageNo, int pageSize, string keyword);
        Task<ResultModel> AddFetcherSchedule(fetcherScheduleEntity entity);
        Task<ResultModel> DeleteFetcherSchedule(int id, int user);
        Task<fetcherScheduleDetailsPagedResult> GetStudentByFetcherScheduleID(string id, int pageNo, int pageSize, string keyword);
        Task<fetcherScheduleDetailsPagedResultVM> GetGroupByFetcherScheduleID(string id, int pageNo, int pageSize, string keyword);
        Task<ResultModel> AddFetcherScheduleStudent(fetcherScheduleDetailsEntity entity);
        Task<ResultModel> DeleteFetcherScheduleStudent(int id, int user);
        Task<ResultModel> DeleteFetcherScheduleGroup(int id, int user);
    }
}
