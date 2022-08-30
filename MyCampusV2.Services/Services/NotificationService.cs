using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Services
{
    public class NotificationService : BaseService, INotificationService
    {
        private string _genNotifBatch = AppDomain.CurrentDomain.BaseDirectory + @"GeneralNotification\";
        private string _personalNotifBatch = AppDomain.CurrentDomain.BaseDirectory + @"PersonalNotification\";
        private ResultModel result = new ResultModel();

        public NotificationService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public IQueryable<notificationEntity> GetData(IQueryable<notificationEntity> query)
        {
            //return !_user.MasterAccess ? query.Where(o => o.tbl_terminal.tbl_area.tbl_campus.Campus_ID == _user.Campus) : query;
            return null;
        }

        public async Task<ICollection<notificationEntity>> GetAll()
        {
            try
            {
                //return await GetData(_unitOfWork.NotificationRepository.GetAllNotification()).ToListAsync();
                //return await _unitOfWork.NotificationRepository.GetAllNotification().ToListAsync();
                return null;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<ICollection<personEntity>> GetAllActivePerson()
        {
            try
            {
                return await _unitOfWork.NotificationRepository.GetAllActivePerson().Where(q => q.IsActive == true && (q.Person_Type.Equals("E") || q.Person_Type.Equals("S"))).ToListAsync();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<ICollection<notificationEntity>> GetAllPersonalNotifications()
        {
            try
            {
                //return await GetData(_unitOfWork.NotificationRepository.GetAllPersonalNotifications()).Where(q => q.Person_ID != 0).ToListAsync();
                //return await _unitOfWork.NotificationRepository.GetAllPersonalNotifications().Where(q => q.Person_ID != 0).ToListAsync();
                return null;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<notificationEntity> GetByIdPersonalNotification(long id)
        {
            try
            {
                //return await _unitOfWork.NotificationRepository.GetAllPersonalNotifications().Where(q => q.Notification_ID == id).FirstOrDefaultAsync();
                return null;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<notificationEntity> GetById(int id)
        {
            //return await GetData(_unitOfWork.NotificationRepository.GetAllNotification()).Where(x => x.ID == id).FirstOrDefaultAsync();
            //return await _unitOfWork.NotificationRepository.GetAllNotification().Where(x => x.Notification_ID == id).FirstOrDefaultAsync();
            return await _unitOfWork.NotificationRepository.GetById(id);
        }

        public async Task<notificationEntity> GetByGuid(string guid)
        {
            return await _unitOfWork.NotificationRepository.GetByGuid(guid);
        }

        public notificationEntity GetByIdSync(int id)
        {
            //return _unitOfWork.NotificationRepository.GetByID_(id).Result;
            return null;
        }

        public async Task<notificationEntity> GetNotification(long id)
        {
            return await _unitOfWork.NotificationRepository.FindAsync(x => x.Notification_ID == id);
        }

        public async Task AddPersonalNotificationFromBatchUpload(notificationEntity notification, int user) 
        {
            try
            {
                var terminals = await _unitOfWork.TerminalRepository.GetAll()
                    .Where(x => x.Terminal_Category == Encryption.Encrypt("PC/Display Terminal") && x.ToDisplay == true).ToListAsync();

                if (!terminals.Count.Equals(0))
                {
                    string guid = Guid.NewGuid().ToString();

                    foreach (int t in terminals.Select(x => x.Terminal_ID))
                    {
                        notificationEntity newnotification = new notificationEntity();

                        newnotification.Added_By = user;
                        newnotification.Date_Time_Added = DateTime.Now;

                        newnotification.Updated_By = user;
                        newnotification.Last_Updated = DateTime.Now;

                        newnotification.IsActive = true;
                        newnotification.ToDisplay = true;

                        newnotification.Terminal_ID = t;
                        newnotification.Notification_Message = notification.Notification_Message;
                        newnotification.Date_To_Display_From = notification.Date_To_Display_From;
                        newnotification.Date_To_Display_To = notification.Date_To_Display_To;
                        newnotification.Person_ID = notification.Person_ID;
                        newnotification.GUID = guid;

                        personEntity person = await _unitOfWork.PersonRepository.FindAsync(x => x.Person_ID == notification.Person_ID && x.ToDisplay == true);

                        await _unitOfWork.NotificationRepository.AddAsyn(newnotification);
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Notification_Personal, "Add Personal Notification", "ADD PERSONAL NOTIFICATION", true,
                        string.Format("Added: Personal Notification ID Number: {2} Message: {0} Terminal: {1}", notification.Notification_Message, notification.Terminal_ID, person.ID_Number), DateTime.UtcNow.ToLocalTime()));
                    }
                }
                else
                {
                    throw new Exception("No existing terminal. Please add terminal first.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AddGeneralNotificationFromBatchUpload(notificationEntity notification, int user)
        {
            //try
            //{
            //    var terminals = await _unitOfWork.TerminalRepository.GetAll()
            //        .Where(x => x.Terminal_Category_ID == 1 && x.IsActive == true).ToListAsync();

            //    if (!terminals.Count.Equals(0))
            //    {
            //        foreach (long t in terminals.Select(x => x.ID))
            //        {
            //            //await Task.Delay(TimeSpan.FromMilliseconds(500));

            //            notificationEntity newnotification = new notificationEntity();

            //            newnotification.Added_By = user;
            //            newnotification.Date_Time_Added = DateTime.Now;

            //            newnotification.Updated_By = user;
            //            newnotification.Last_Updated = DateTime.Now;

            //            newnotification.Terminal_ID = t;
            //            newnotification.Message = notification.Message;
            //            newnotification.Date_From = notification.Date_From;
            //            newnotification.Date_To = notification.Date_To;

            //            await _unitOfWork.NotificationRepository.AddAsyn(newnotification);
            //            await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Notification_General,
            //                string.Format("Added: General Notification Message: {0} Terminal: {1}", notification.Message, notification.Terminal_ID));
            //        }
            //    }
            //    else
            //    {
            //        throw new Exception("No existing terminal. Please add terminal first.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        public async Task UpdateGeneralNotificationFromBatchUpload(notificationEntity notification, int user)
        {
            //try
            //{
            //    var notifs = await _unitOfWork.NotificationRepository.GetAll()
            //        .Where(x => x.Date_From == Convert.ToDateTime(notification.Date_From) 
            //        && x.Date_To == Convert.ToDateTime(notification.Date_To) 
            //        && x.Person_ID == 0 && x.IsActive == true).ToListAsync();

            //    foreach(var t in notifs)
            //    {
            //        //await Task.Delay(TimeSpan.FromMilliseconds(500));

            //        notificationEntity _notification = new notificationEntity();
            //        _notification.Message = notification.Message;
            //        _notification.Date_From = notification.Date_From;
            //        _notification.Date_To = notification.Date_To;
            //        _notification.Terminal_ID = t.Terminal_ID;
            //        _notification.ID = t.ID;
            //        _notification.Person_ID = 0;
            //        _notification.Last_Updated = DateTime.Now;
            //        _notification.Updated_By = user;

            //        await _unitOfWork.NotificationRepository.UpdateAsyn(_notification, _notification.ID);
            //        await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Notification_General,
            //            string.Format("Updated: General Notification Message: {0} Terminal: {1}", notification.Message, notification.Terminal_ID));
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        public async Task<BatchUploadResponse> BatchGeneralNotificationUpload(ICollection<notificationBatchUploadVM> notifications, int user, int uploadID, int row)
        {
            try
            {
                ImportLog importLog = new ImportLog();
                var response = new BatchUploadResponse();

                string fileName = "GeneralNotification_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

                response.Details = new List<BatchUploadResponse.UploadDetails>();
                response.Success = 0;
                response.Failed = 0;
                response.Total = notifications.Count();
                response.FileName = fileName;

                int i = 1 + row;
                foreach (var notificationVM in notifications)
                {
                    i++;

                    //var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == notificationVM.campus_Name && x.IsActive == true);

                    //if (campus == null)
                    //{
                    //    response.Details.Add(new BatchUploadResponse.UploadDetails { Item = notificationVM, Message = "Row " + i.ToString() + " ---> Campus " + notificationVM.campus_Name + " does not exist.", isError = true });
                    //    response.Failed++;
                    //    continue;
                    //}

                    //var area = await _unitOfWork.AreaRepository.FindAsync(x => x.Area_Name == notificationVM.area_Name && x.Campus_ID == campus.Campus_ID && x.IsActive == true);

                    //if (area == null)
                    //{
                    //    response.Details.Add(new BatchUploadResponse.UploadDetails { Item = notificationVM, Message = "Row " + i.ToString() + " ---> Area " + notificationVM.campus_Name + " under Campus " + notificationVM.campus_Name + " does not exist.", isError = true });
                    //    response.Failed++;
                    //    continue;
                    //}

                    //var terminal = await _unitOfWork.TerminalRepository.FindAsync(x => x.Terminal_Name == notificationVM.terminal_Name && x.Area_ID == area.ID && x.IsActive == true);

                    //if (terminal == null)
                    //{
                    //    response.Details.Add(new BatchUploadResponse.UploadDetails { Item = notificationVM, Message = "Row " + i.ToString() + " ---> Terminal " + notificationVM.terminal_Name + " under Campus " + notificationVM.campus_Name + " ---> Area " + notificationVM.area_Name + " does not exist.", isError = true });
                    //    importLog.Logging(_genNotifBatch, fileName, "Row " + i.ToString() + " ---> General Notification " + notification.Message + " From:  " + notification.Date_From.ToString("yyyy-MM-dd") + " To: " + notification.Date_To.ToString("yyyy-MM-dd") + " already exist.");
                    //    response.Failed++;
                    //    continue;
                    //}

                    //notificationEntity notification = await _unitOfWork.NotificationRepository.FindAsync(x => x.Message == notificationVM.Message && x.Date_From == Convert.ToDateTime(notificationVM.DateFrom) && x.Date_To == Convert.ToDateTime(notificationVM.DateTo) && x.tbl_terminal.Terminal_Name == notificationVM.terminal_Name && x.Person_ID == 0);

                    if (notificationVM.Message == "" || notificationVM.Message == null)
                    {
                        importLog.Logging(_genNotifBatch, fileName, "Row: " + i.ToString() + " ---> General Notification Message is a required field.");
                        response.Failed++;
                        continue;
                    }

                    if (notificationVM.Message.Length > 155)
                    {
                        importLog.Logging(_genNotifBatch, fileName, "Row: " + i.ToString() + " ---> General Notification Message accepts 155 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (notificationVM.DateFrom == "" || notificationVM.DateFrom == null)
                    {
                        importLog.Logging(_genNotifBatch, fileName, "Row: " + i.ToString() + " ---> General Notification Date From is a required field.");
                        response.Failed++;
                        continue;
                    }

                    DateTime temp;
                    DateTime dateToday = DateTime.Now.Date;
                    if (!DateTime.TryParse(notificationVM.DateFrom, out temp))
                    {
                        importLog.Logging(_genNotifBatch, fileName, "Row: " + i.ToString() + " ---> General Notification Date From is invalid.");
                        response.Failed++;
                        continue;
                    }
                    else if (temp < dateToday)
                    {
                        importLog.Logging(_genNotifBatch, fileName, "Row: " + i.ToString() + " ---> General Notification Date From does not accept previous dates.");
                        response.Failed++;
                        continue;
                    }

                    if (notificationVM.DateTo == "" || notificationVM.DateTo == null)
                    {
                        importLog.Logging(_genNotifBatch, fileName, "Row: " + i.ToString() + " ---> General Notification Date To is a required field.");
                        response.Failed++;
                        continue;
                    }

                    if (!DateTime.TryParse(notificationVM.DateTo, out temp))
                    {
                        importLog.Logging(_genNotifBatch, fileName, "Row: " + i.ToString() + " ---> General Notification Date To is invalid.");
                        response.Failed++;
                        continue;
                    }
                    else if (temp < dateToday)
                    {
                        importLog.Logging(_genNotifBatch, fileName, "Row: " + i.ToString() + " ---> General Notification Date To does not accept previous dates.");
                        response.Failed++;
                        continue;
                    }

                    if (Convert.ToDateTime(notificationVM.DateFrom) > Convert.ToDateTime(notificationVM.DateTo))
                    {
                        importLog.Logging(_genNotifBatch, fileName, "Row: " + i.ToString() + " ---> General Notification Date From must not be greater than Date To.");
                        response.Failed++;
                        continue;
                    }

                    //notificationEntity notification =
                    //    await _unitOfWork.NotificationRepository
                    //    .GetAll().Where(x => x.Date_From <= Convert.ToDateTime(notificationVM.DateTo)
                    //    && Convert.ToDateTime(notificationVM.DateFrom) <= x.Date_To
                    //    && x.Message == notificationVM.Message
                    //    && x.Person_ID == 0
                    //    && x.IsActive == true).FirstOrDefaultAsync();

                    //if (notification != null)
                    //{
                    //    await UpdateGeneralNotificationFromBatchUpload(notification, user);
                    //    importLog.Logging(_genNotifBatch, fileName, "Row: " + i.ToString() + " ---> General Notification " + notification.Message + " Date From:  " + notification.Date_From.ToString("yyyy-MM-dd") + " Date To: " + notification.Date_To.ToString("yyyy-MM-dd") + " has been successfully updated.");
                    //    response.Success++;
                    //    continue;
                    //}
                    //else
                    //{
                    //    notification = new notificationEntity();
                    //    //notification.Terminal_ID = terminal.ID;
                    //    notification.Message = notificationVM.Message;
                    //    notification.Date_From = Convert.ToDateTime(notificationVM.DateFrom);
                    //    notification.Date_To = Convert.ToDateTime(notificationVM.DateTo);
                    //    notification.Person_ID = 0;
                    //    await AddGeneralNotificationFromBatchUpload(notification, user);
                    //    importLog.Logging(_genNotifBatch, fileName, "Row: " + i.ToString() + " ---> Notification Message " + notificationVM.Message + " has been successfully added.");
                    //    //response.Details.Add(new BatchUploadResponse.UploadDetails { Item = notificationVM, Message = "Row " + i.ToString() + " ---> Notification Message " + notificationVM.Message + " has been successfully added.", isError = false });
                    //    response.Success++;
                    //}
                }

                return response;
            }
            catch (Exception er)
            {
                throw er;
            }
        }

        public async Task UpdatePersonalNotificationFromBatchUpload(notificationEntity notification, int user)
        {
            try
            {
                var notifs = await _unitOfWork.NotificationRepository.GetAll().Where(x => ((x.Notification_Message == notification.Notification_Message)
                    || (x.Date_To_Display_From == notification.Date_To_Display_From && x.Date_To_Display_To == notification.Date_To_Display_To))
                    && x.Person_ID == notification.Person_ID && x.ToDisplay == true).ToListAsync();

                foreach (var t in notifs)
                {
                    notificationEntity _notification = new notificationEntity();
                    _notification.Notification_Message = notification.Notification_Message;
                    _notification.Date_To_Display_From = notification.Date_To_Display_From;
                    _notification.Date_To_Display_To = notification.Date_To_Display_To;
                    _notification.Terminal_ID = t.Terminal_ID;
                    _notification.Notification_ID = t.Notification_ID;
                    _notification.Person_ID = notification.Person_ID;
                    _notification.GUID = t.GUID;
                    _notification.Date_Time_Added = _notification.Date_Time_Added;
                    _notification.Last_Updated = DateTime.Now;
                    _notification.Added_By = _notification.Added_By;
                    _notification.Updated_By = user;

                    _notification.IsActive = true;
                    _notification.ToDisplay = true;

                    personEntity person = await _unitOfWork.PersonRepository.FindAsync(x => x.Person_ID == notification.Person_ID && x.IsActive == true);

                    await _unitOfWork.NotificationRepository.UpdateAsyn(_notification, _notification.Notification_ID);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Notification_Personal, "Update Personal Notification", "UPDATE PERSONAL NOTIFICATION", true, 
                        string.Format("Update: Personal Notification ID Number: {2} Message: {0} Terminal: {1}", notification.Notification_Message, notification.Terminal_ID, person.ID_Number), DateTime.UtcNow.ToLocalTime()));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BatchUploadResponse> BatchPersonalNotificationUpload(ICollection<personalNotificationBatchUploadVM> notifications, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "PersonalNotification_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = notifications.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var notificationVM in notifications)
            {
                i++;

                if (notificationVM.Message == "" || notificationVM.Message == null)
                {
                    importLog.Logging(_personalNotifBatch, fileName, "Row: " + i.ToString() + " ---> Personal Notification Message is a required field.");
                    response.Failed++;
                    continue;
                }

                if (notificationVM.Message.Length > 155)
                {
                    importLog.Logging(_personalNotifBatch, fileName, "Row: " + i.ToString() + " ---> Personal Notification Message accepts 155 characters only.");
                    response.Failed++;
                    continue;
                }

                if (notificationVM.DateFrom == "" || notificationVM.DateFrom == null)
                {
                    importLog.Logging(_personalNotifBatch, fileName, "Row: " + i.ToString() + " ---> Personal Notification Date From is a required field.");
                    response.Failed++;
                    continue;
                }

                DateTime temp;
                DateTime dateToday = DateTime.Now.Date;
                if (!DateTime.TryParse(notificationVM.DateFrom, out temp))
                {
                    importLog.Logging(_personalNotifBatch, fileName, "Row: " + i.ToString() + " ---> Personal Notification Date From is invalid.");
                    response.Failed++;
                    continue;
                }
                else if (temp < dateToday)
                {
                    importLog.Logging(_personalNotifBatch, fileName, "Row: " + i.ToString() + " ---> Personal Notification Date From does not accept previous dates.");
                    response.Failed++;
                    continue;
                }

                if (notificationVM.DateTo == "" || notificationVM.DateTo == null)
                {
                    importLog.Logging(_personalNotifBatch, fileName, "Row: " + i.ToString() + " ---> Personal Notification Date To is a required field.");
                    response.Failed++;
                    continue;
                }

                if (!DateTime.TryParse(notificationVM.DateTo, out temp))
                {
                    importLog.Logging(_personalNotifBatch, fileName, "Row: " + i.ToString() + " ---> Personal Notification Date To is invalid.");
                    response.Failed++;
                    continue;
                }
                else if (temp < dateToday)
                {
                    importLog.Logging(_personalNotifBatch, fileName, "Row: " + i.ToString() + " ---> Personal Notification Date To does not accept previous dates.");
                    response.Failed++;
                    continue;
                }

                if (Convert.ToDateTime(notificationVM.DateFrom) > Convert.ToDateTime(notificationVM.DateTo))
                {
                    importLog.Logging(_personalNotifBatch, fileName, "Row: " + i.ToString() + " ---> Personal Notification Date From must not be greater than Date To.");
                    response.Failed++;
                    continue;
                }

                if (notificationVM.IDNumber == "" || notificationVM.IDNumber == null)
                {
                    importLog.Logging(_personalNotifBatch, fileName, "Row: " + i.ToString() + " ---> Personal Notification ID Number is a required field.");
                    response.Failed++;
                    continue;
                }

                var person = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == notificationVM.IDNumber && x.IsActive == true);

                if (person == null)
                {
                    //response.Details.Add(new BatchUploadResponse.UploadDetails { Item = notificationVM, Message = "Row " + i.ToString() + " ---> ID Number " + notificationVM.IDNumber + " does not exist.", isError = true });
                    importLog.Logging(_personalNotifBatch, fileName, "Row: " + i.ToString() + " ---> ID Number " + notificationVM.IDNumber + " does not exist.");
                    response.Failed++;
                    continue;
                }

                notificationEntity notification = await _unitOfWork.NotificationRepository.GetAll().Where(x => ((x.Notification_Message == notificationVM.Message)
                    || (x.Date_To_Display_From == Convert.ToDateTime(notificationVM.DateFrom) && x.Date_To_Display_To == Convert.ToDateTime(notificationVM.DateTo)))
                    && x.Person_ID == person.Person_ID
                    && x.ToDisplay == true).FirstOrDefaultAsync();

                //notificationEntity notification = await _unitOfWork.NotificationRepository.GetAll().Where(x => x.Date_To_Display_From <= Convert.ToDateTime(notificationVM.DateTo)
                //    && Convert.ToDateTime(notificationVM.DateFrom) <= x.Date_To_Display_To
                //    && x.Person_ID == person.Person_ID
                //    && x.ToDisplay == true).FirstOrDefaultAsync();

                //notificationEntity notification = await _unitOfWork.NotificationRepository.FindAsync(x => x.Message == notificationVM.Message && x.Date_From == Convert.ToDateTime(notificationVM.DateFrom) && x.Date_To == Convert.ToDateTime(notificationVM.DateTo) && x.tbl_terminal.Terminal_Name == notificationVM.terminal_Name && x.tbl_person.ID_Number == notificationVM.IDNumber);

                if (notification != null)
                {
                    notification.Notification_Message = notificationVM.Message;
                    notification.Date_To_Display_From = Convert.ToDateTime(notificationVM.DateFrom);
                    notification.Date_To_Display_To = Convert.ToDateTime(notificationVM.DateTo);
                    notification.Last_Updated = DateTime.Now;
                    notification.Updated_By = user;
                    await UpdatePersonalNotificationFromBatchUpload(notification, user);
                    importLog.Logging(_personalNotifBatch, fileName, "Row: " + i.ToString() + " ---> Personal Notification for " + person.ID_Number + " " + notification.Notification_Message + " Date From: " + notification.Date_To_Display_From.ToString("yyyy-MM-dd") + " Date To: " + notification.Date_To_Display_To.ToString("yyyy-MM-dd") + " has been successfully updated.");
                    response.Success++;
                    continue;
                }
                else
                {
                    notification = new notificationEntity();
                    notification.Notification_Message = notificationVM.Message;
                    notification.Date_To_Display_From = Convert.ToDateTime(notificationVM.DateFrom);
                    notification.Date_To_Display_To = Convert.ToDateTime(notificationVM.DateTo);
                    notification.Person_ID = person.Person_ID;
                    await AddPersonalNotificationFromBatchUpload(notification, user);
                    importLog.Logging(_personalNotifBatch, fileName, "Row: " + i.ToString() + " ---> Personal Notification " + notification.Notification_Message + " Date From: " + notification.Date_To_Display_From.ToString("yyyy-MM-dd") + " Date To: " + notification.Date_To_Display_To.ToString("yyyy-MM-dd") + " has been successfully added.");
                    response.Success++;
                }
            }

            return response;
        }

        public async Task<notificationPagedResult> GetAllGeneralNotifList(int pageNo, int pageSize, string keyword)
        {
            //return await _unitOfWork.NotificationRepository.GetAllGeneralNotifList(pageNo, pageSize, keyword);
            return null;
        }

        public async Task<personalNotificationPagedResult> GetAllPersonalNotifList(int pageNo, int pageSize, string keyword)
        {
            //return await _unitOfWork.NotificationRepository.GetAllPersonalNotifList(pageNo, pageSize, keyword);
            return null;
        }

        public async Task AddNotification(notificationEntity notification, int user)
        {
            //try
            //{
            //    var data = await _unitOfWork.NotificationRepository.AddAsyncWithBase(notification);

            //    if (data == null)
            //    {
            //        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(notification.Added_By, (int)Form.Notification_General, "Add General Notification", "INSERT", false, "Failed: " + notification.Notification_Message, DateTime.UtcNow.ToLocalTime()));
            //        result = new ResultModel();
            //        result.resultCode = "409";
            //        result.isSuccess = false;
            //        return result;
            //    }

            //    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(notification.Added_By, (int)Form.Notification_General, "Add General Notification", "INSERT", true, "Success: " + notification.Notification_Message, DateTime.UtcNow.ToLocalTime()));

            //    result = new ResultModel();
            //    result.resultCode = "200";
            //    result.resultMessage = "Office" + Constants.SuccessMessageAdd;
            //    result.isSuccess = true;
            //    return result;

            //}
            //catch (Exception err)
            //{
            //    result = new ResultModel();
            //    result.resultCode = "500";
            //    result.resultMessage = err.Message;
            //    result.isSuccess = false;
            //    return result;
            //}
        }

        public async Task UpdateNotification(notificationEntity notification, int user)
        {
            try
            {
                notificationEntity oldNotification = await GetNotification(notification.Notification_ID);
                notification.Date_Time_Added = oldNotification.Date_Time_Added;
                notification.Added_By = oldNotification.Added_By;
                notification.Last_Updated = DateTime.Now;
                notification.Updated_By = user;

                await _unitOfWork.NotificationRepository.UpdateAsyn(notification, notification.Notification_ID);
                //await _unitOfWork.AuditTrailRepository.AuditAsync(user, notification.Person_ID == 0 ? (int)Form.Notification_General : (int)Form.Notification_Personal, string.Format("Updated: Notification Message: {0}", notification.Message));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteNotification(long id, int user)
        {
            try
            {
                notificationEntity notification = await GetNotification(id);

                notification.Last_Updated = DateTime.Now;
                notification.Updated_By = user;

                if (notification.IsActive)
                {
                    notification.IsActive = false;
                }
                else
                {
                    notification.IsActive = true;
                }

                await _unitOfWork.NotificationRepository.UpdateAsyn(notification, notification.Notification_ID);
                //await _unitOfWork.AuditTrailRepository.AuditAsync(user, notification.Person_ID == 0 ? (int)Form.Notification_General : (int)Form.Notification_Personal, string.Format("Updated Notification {1} status to {0}", notification.IsActive ? "Active" : "Inactive", notification.Message));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        public async Task<ResultModel> AddGeneralNotification(notificationEntity notification)
        {
            try
            {
                var isExist = await _unitOfWork.NotificationRepository.FindAllAsync(q => q.Notification_Message == notification.Notification_Message
                && q.Date_To_Display_From == Convert.ToDateTime(Convert.ToDateTime(notification.Date_To_Display_From).ToString("yyyy-MM-dd 00:00:00"))
                && q.Date_To_Display_To == Convert.ToDateTime(Convert.ToDateTime(notification.Date_To_Display_To).ToString("yyyy-MM-dd 00:00:00"))
                && q.IsActive == true
                && q.IsDeleted == false);

                if (isExist.Count != 0)
                    return CreateResult("409", GENERAL_NOTIFICATION_EXIST, false);

                //var terminals = await _unitOfWork.TerminalRepository.FindAllAsync(q => q.Terminal_Category == Encryption.Encrypt("PC/Display Terminal") && q.IsActive == true);

                return await _unitOfWork.NotificationRepository.AddNotification(notification,
                    fillEventLogging(notification.Added_By, (int)Form.Notification_General, "Add General Notification", "INSERT", false, "Failed: " + notification.Notification_Message, DateTime.UtcNow.ToLocalTime()));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateGeneralNotification(notificationEntity notification)
        {
            try
            {
                //ICollection<notificationEntity> list = await _unitOfWork.NotificationRepository.FindAllAsync(q => q.GUID == notification.GUID);

                //foreach (notificationEntity data in list)
                //{
                //    data.Updated_By = notification.Updated_By;
                //    data.Last_Updated = notification.Last_Updated;
                //    data.Notification_Message = notification.Notification_Message;
                //    data.Date_To_Display_From = notification.Date_To_Display_From;
                //    data.Date_To_Display_To = notification.Date_To_Display_To;
                //    await _unitOfWork.NotificationRepository.UpdateAsyncWithBase(data, data.Notification_ID);
                //}

                notification.Last_Updated = DateTime.UtcNow.ToLocalTime();
                var data = await _unitOfWork.NotificationRepository.UpdateAsyncWithBase(notification, notification.Notification_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(notification.Added_By, (int)Form.Notification_General, "Update General Notification", "UPDATE", false, "Failed: " + notification.Notification_Message, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "General Notification", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(notification.Updated_By, (int)Form.Notification_General, "Update General Notification", "UPDATE", true, "Success: General Notification Message: " + notification.Notification_Message, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "General Notification" + Constants.SuccessMessageUpdate, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteGeneralNotification(int id, int user)
        {
            try
            {
                /*
                 *  FOR UNIT TESTING / SLOW IN UPDATE
                 */
                //ICollection<notificationEntity> list = await _unitOfWork.NotificationRepository.FindAllAsync(q => q.GUID == notification.GUID);

                //foreach (notificationEntity data in list)
                //{
                //    data.Updated_By = notification.Updated_By;
                //    data.Last_Updated = notification.Last_Updated;
                //    data.IsDeleted = false;
                //    data.IsActive = false;
                //    await _unitOfWork.NotificationRepository.UpdateAsyncWithBase(data, data.Notification_ID);
                //}

                var notification = await _unitOfWork.NotificationRepository.FindAsync(q => q.Notification_ID == id);

                notification.Updated_By = user;
                notification.Last_Updated = DateTime.UtcNow.ToLocalTime();
                notification.IsDeleted = false;
                notification.IsActive = false;

                var data = await _unitOfWork.NotificationRepository.UpdateAsyncWithBase(notification, notification.Notification_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(notification.Updated_By, (int)Form.Notification_General, "Delete General Notification", "DELETE", false, "Failed: " + notification.Notification_Message, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "General Notification", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(notification.Updated_By, (int)Form.Notification_General, "Delete General Notification", "DELETE", true, "Success: " + notification.Notification_Message, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "General Notification" + Constants.SuccessMessageDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }



        public async Task<ResultModel> AddPersonalNotification(notificationEntity notification)
        {
            try
            {
                var terminals = await _unitOfWork.TerminalRepository.FindAllAsync(q => q.Terminal_Category == Encryption.Encrypt("PC/Display Terminal") && q.ToDisplay == true);

                return await _unitOfWork.NotificationRepository.AddPersonalNotification(notification,
                    fillEventLogging(notification.Added_By, (int)Form.Notification_General, "Add Personal Notification", "INSERT", false, "Failed: " + notification.Notification_Message, DateTime.UtcNow.ToLocalTime()),
                    terminals);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdatePersonalNotification(notificationEntity notification)
        {
            try
            {
                ICollection<notificationEntity> list = await _unitOfWork.NotificationRepository.FindAllAsync(q => q.GUID == notification.GUID);

                foreach (notificationEntity data in list)
                {
                    data.Updated_By = notification.Updated_By;
                    data.Last_Updated = notification.Last_Updated;
                    data.Person_ID = notification.Person_ID;
                    data.Notification_Message = notification.Notification_Message;
                    data.Date_To_Display_From = notification.Date_To_Display_From;
                    data.Date_To_Display_To = notification.Date_To_Display_To;
                    await _unitOfWork.NotificationRepository.UpdateAsyncWithBase(data, data.Notification_ID);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(notification.Updated_By, (int)Form.Notification_General, "Update Personal Notification", "UPDATE", true, "Success: General Notification ID: " + notification.Notification_ID + " General Notification Message: " + notification.Notification_Message, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Personal Notification" + Constants.SuccessMessageUpdate, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeletePersonalNotification(notificationEntity notification)
        {
            try
            {
                /*
                 *  FOR UNIT TESTING / SLOW IN UPDATE
                 */
                ICollection<notificationEntity> list = await _unitOfWork.NotificationRepository.FindAllAsync(q => q.GUID == notification.GUID);

                foreach (notificationEntity data in list)
                {
                    data.Updated_By = notification.Updated_By;
                    data.Last_Updated = notification.Last_Updated;
                    data.IsDeleted = true;
                    data.IsActive = false;
                    data.ToDisplay = false;
                    await _unitOfWork.NotificationRepository.UpdateAsyncWithBase(data, data.Notification_ID);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(notification.Updated_By, (int)Form.Notification_General, "Delete Personal Notification", "PERMANENT DELETE", true, "Success: " + notification.Notification_Message, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Personal Notification" + Constants.SuccessMessageDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<notificationPagedResultVM> GetGeneralNotificationsPagination(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.NotificationRepository.GetGeneralNotificationsPagination(pageNo, pageSize, keyword);
        }

        //public async Task<ICollection<notificationEntity>> GetGeneralNotificationsPagination(int pageNo, int pageSize, string keyword)
        //{
        //    return await _unitOfWork.NotificationRepository.GetGeneralNotificationsPagination(pageNo, pageSize, keyword).ToListAsync();
        //}

        public async Task<notificationPagedResultVM> GetPersonalNotificationsPagination(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.NotificationRepository.GetPersonalNotificationsPagination(pageNo, pageSize, keyword);
        }

        //public async Task<notificationPagedResult> GetPersonalNotificationsPagination(int pageNo, int pageSize, string keyword)
        //{
        //    return await _unitOfWork.NotificationRepository.GetPersonalNotificationsPagination(pageNo, pageSize, keyword);
        //}
    }
}
