using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Net.Http;
using RestSharp;

namespace MyCampusV2.Services.Services
{
    public class AnnouncementsService : BaseService, IAnnouncementsService
    {
        private string _announcementsBatch = AppDomain.CurrentDomain.BaseDirectory + @"Announcements\";
        private ResultModel result = new ResultModel();

        public AnnouncementsService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user)
            : base(unitOfWork, audit, user)
        {

        }

        public async Task<ResultModel> AddAnnouncement(announcementsEntity announcementsEntity, int userId)
        {
            try
            {
                return await _unitOfWork.AnnouncementsRepository.AddAnnouncement(announcementsEntity, userId);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteAnnouncementsPermanent(string announcementCode, int userId)
        {
            try
            {
                return await _unitOfWork.AnnouncementsRepository.DeleteAnnouncementsPermanent(announcementCode, userId);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateAnnouncement(announcementsEntity announcementsEntity, int userId)
        {
            try
            {
                return await _unitOfWork.AnnouncementsRepository.UpdateAnnouncement(announcementsEntity, userId);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<announcementsPagedResult> GetAll(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.AnnouncementsRepository.GetAll(pageNo, pageSize, keyword);
        }

        public async Task<announcementsEntity> GetAnnouncementByAnnouncementCode(string code)
        {
            return await _unitOfWork.AnnouncementsRepository.GetAnnouncementByAnnouncementCode(code);
        }

        public async Task<IList<yearSectionEntity>> GetYearLevelList()
        {
            return await _unitOfWork.AnnouncementsRepository.GetYearLevelList();
        }

        public async Task<announcementsPagedResult> Export(string keyword)
        {
            return await _unitOfWork.AnnouncementsRepository.Export(keyword);
        }

        public async Task<ResultModel> PushNotification()
        {
            try
            {
                var condition = "'HEADS_UP_NOTIFICATION' in topics || 'GRADE_1' in topics";
                //var condition = "'GRADE_1' in topics";
                //var condition = "'HEADS_UP_NOTIFICATION' in topics";

                var response = await FirebaseMessaging.DefaultInstance.SendAsync(new Message
                {
                    //Topic = "HEADS_UP_NOTIFICATION",
                    //Topic = "GRADE_1",
                    Data = new Dictionary<string, string>
                                    {
                                        { "AnnouncementCode", "1" },
                                    },
                    Notification = new Notification
                    {
                        Title = "Poveda Batch 2020 - 2021 Graduation Celebration.",
                        Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                    },
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            ClickAction = "OPEN_ACTIVITY_ANNOUNCEMENT"
                        }
                    },
                    
                    Condition = condition
                });

                //var response = FirebaseMessaging.DefaultInstance.SendAsync(message).Result;

                return CreateResult("200", "All GOOD!", true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }
    }
}
