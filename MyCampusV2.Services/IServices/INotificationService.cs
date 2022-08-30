using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface INotificationService
    {
        Task<ICollection<notificationEntity>> GetAll();

        Task<notificationEntity> GetById(int id);

        Task<notificationEntity> GetByGuid(string guid);

        notificationEntity GetByIdSync(int id);
        Task AddNotification(notificationEntity notification, int userID);
        Task UpdateNotification(notificationEntity notification, int user);
        Task DeleteNotification(long id, int user);
        Task<ICollection<notificationEntity>> GetAllPersonalNotifications();
        Task<notificationEntity> GetByIdPersonalNotification(long id);
        Task<ICollection<personEntity>> GetAllActivePerson();

        Task<BatchUploadResponse> BatchGeneralNotificationUpload(ICollection<notificationBatchUploadVM> notifications, int user, int uploadID, int row);
        Task<BatchUploadResponse> BatchPersonalNotificationUpload(ICollection<personalNotificationBatchUploadVM> notifications, int user, int uploadID, int row);

        Task<notificationPagedResult> GetAllGeneralNotifList(int pageNo, int pageSize, string keyword);
        Task<personalNotificationPagedResult> GetAllPersonalNotifList(int pageNo, int pageSize, string keyword);

        Task<ResultModel> AddGeneralNotification(notificationEntity notification);
        Task<ResultModel> UpdateGeneralNotification(notificationEntity notification);
        Task<ResultModel> DeleteGeneralNotification(int id, int user);

        Task<ResultModel> AddPersonalNotification(notificationEntity notification);
        Task<ResultModel> UpdatePersonalNotification(notificationEntity notification);
        Task<ResultModel> DeletePersonalNotification(notificationEntity notification);

        Task<notificationPagedResultVM> GetGeneralNotificationsPagination(int pageNo, int pageSize, string keyword);

        Task<notificationPagedResultVM> GetPersonalNotificationsPagination(int pageNo, int pageSize, string keyword);
    }
}
