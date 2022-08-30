using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface INotificationRepository : IBaseRepository<notificationEntity>
    {
        Task<notificationEntity> GetByGuid(string guid);
        Task<notificationEntity> GetById(int id);
        IQueryable<personEntity> GetAllActivePerson();
        Task<ResultModel> AddPersonalNotification(notificationEntity entity, eventLoggingEntity eventEntity, ICollection<terminalEntity> terminalEntity);
        Task<ResultModel> AddNotification(notificationEntity entity, eventLoggingEntity eventEntity);
        Task<notificationPagedResultVM> GetGeneralNotificationsPagination(int pageNo, int pageSize, string keyword);
        Task<notificationPagedResultVM> GetPersonalNotificationsPagination(int pageNo, int pageSize, string keyword);
    }
}
