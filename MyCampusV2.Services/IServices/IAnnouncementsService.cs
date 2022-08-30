using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface IAnnouncementsService
    {
        Task<ResultModel> AddAnnouncement(announcementsEntity announcementsEntity, int userId);
        //Task<ResultModel> UpdateMobileAppAccount(papAccountEntity papAccount, int userId);
        Task<announcementsEntity> GetAnnouncementByAnnouncementCode(string code);
        Task<announcementsPagedResult> GetAll(int pageNo, int pageSize, string keyword);
        Task<IList<yearSectionEntity>> GetYearLevelList();
        Task<ResultModel> DeleteAnnouncementsPermanent(string announcementCode, int userId);
        Task<ResultModel> UpdateAnnouncement(announcementsEntity announcementsEntity, int userId);
        Task<announcementsPagedResult> Export(string keyword);
        Task<ResultModel> PushNotification();
    }
}
