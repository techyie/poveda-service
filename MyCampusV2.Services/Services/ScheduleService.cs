using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Services.Helpers;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyCampusV2.Models.V2.entity;

namespace MyCampusV2.Services.Services
{
    public class ScheduleService : BaseService, IScheduleService
    {
        private ResultModel result = new ResultModel();

        public ScheduleService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) 
            : base(unitOfWork, audit, user) 
        { 
        }

        public async Task<ResultModel> AddSchedule(scheduleEntity scheduleEntity)
        {
            try
            {
                var exist = await _unitOfWork.ScheduleRepository.FindAsync(q => q.Schedule_Name == scheduleEntity.Schedule_Name && q.IsActive == true && q.ToDisplay == true);

                if (exist != null)
                    return CreateResult("409", SCHEDULE_EXIST, false);

                var data = await _unitOfWork.ScheduleRepository.AddAsyncWithBase(scheduleEntity);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(scheduleEntity.Added_By, (int)Form.Fetcher_Schedule, "Add Schedule", "INSERT", false, "Failed: " + scheduleEntity.Schedule_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Schedule", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(scheduleEntity.Added_By, (int)Form.Fetcher_Schedule, "Add Schedule", "INSERT", true, "Success: " + scheduleEntity.Schedule_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Schedule" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateSchedule(scheduleEntity scheduleEntity)
        {
            try
            {
                var data = await _unitOfWork.ScheduleRepository.UpdateAsyncWithBase(scheduleEntity, scheduleEntity.Schedule_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(scheduleEntity.Added_By, (int)Form.Fetcher_Schedule, "Update Schedule", "INSERT", false, "Failed: " + scheduleEntity.Schedule_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Schedule", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(scheduleEntity.Added_By, (int)Form.Fetcher_Schedule, "Update Schedule", "UPDATE", true, "Success: Schedule ID: " + scheduleEntity.Schedule_ID + " Schedule Name: " + scheduleEntity.Schedule_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Schedule" + Constants.SuccessMessageUpdate, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteSchedulePermanent(int id, int user)
        {
            try
            {
                scheduleEntity schedule = await GetScheduleByID(id);

                schedule.Updated_By = user;

                var data = await _unitOfWork.ScheduleRepository.DeleteAsyncPermanent(schedule, schedule.Schedule_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(schedule.Updated_By, (int)Form.Fetcher_Schedule, "Delete Permanently Fetcher Schedule", "PERMANENT DELETE", false, "Failed: " + schedule.Schedule_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Fetcher Schedule", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(schedule.Updated_By, (int)Form.Fetcher_Schedule, "Delete Permanently Fetcher Schedule", "PERMANENT DELETE", true, "Success: " + schedule.Schedule_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Fetcher Schedule" + Constants.SuccessMessagePermanentDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteScheduleTemporary(int id, int user)
        {
            try
            {
                scheduleEntity schedule = await GetScheduleByID(id);

                if (schedule.Schedule_Status == "Active")
                    schedule.Schedule_Status = "Inactive";
                else
                    schedule.Schedule_Status = "Active";

                schedule.Updated_By = user;

                var data = await _unitOfWork.ScheduleRepository.DeleteAsyncTemporary(schedule, schedule.Schedule_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(schedule.Updated_By, (int)Form.Fetcher_Schedule, "Deactivate Fetcher Schedule", "DEACTIVATE", false, "Failed: " + schedule.Schedule_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Fetcher Schedule", false);
                }

                if (schedule.Schedule_Status == "Active")
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(schedule.Updated_By, (int)Form.Fetcher_Schedule, "Activate Fetcher Schedule", "ACTIVATE", true, "Success: " + schedule.Schedule_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("200", "Fetcher Schedule" + Constants.SuccessMessageRetrieve, true);
                }
                else
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(schedule.Updated_By, (int)Form.Fetcher_Schedule, "Deactivate Fetcher Schedule", "DEACTIVATE", true, "Success: " + schedule.Schedule_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("200", "Fetcher Schedule" + Constants.SuccessMessageTemporaryDelete, true);
                }
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> RetrieveSchedule(scheduleEntity scheduleEntity)
        {
            try
            {
                var data = await _unitOfWork.ScheduleRepository.RetrieveAsync(scheduleEntity, scheduleEntity.Schedule_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(scheduleEntity.Updated_By, (int)Form.Fetcher_Schedule, "Activate Schedule", "ACTIVATE SCHEDULE", false, "Failed: " + scheduleEntity.Schedule_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Schedule", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(scheduleEntity.Updated_By, (int)Form.Fetcher_Schedule, "Activate Schedule", "ACTIVATE SCHEDULE", true, "Success: " + scheduleEntity.Schedule_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Schedule" + Constants.SuccessMessageRetrieve, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<schedulePagedResult> GetAllSchedule(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.ScheduleRepository.GetAllSchedule(pageNo, pageSize, keyword);
        }

        public async Task<ICollection<scheduleEntity>> GetSchedules()
        {
            try
            {
                return await _unitOfWork.ScheduleRepository.GetSchedules();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<scheduleEntity> GetScheduleByID(int id)
        {
            return await _unitOfWork.ScheduleRepository.GetScheduleByID(id);
        }

        public async Task<fetcherSchedulePagedResult> GetScheduleByFetcherID(string id, int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.ScheduleRepository.GetScheduleByFetcherID(id, pageNo, pageSize, keyword);
        }

        public async Task<ResultModel> AddFetcherSchedule(fetcherScheduleEntity entity)
        {
            try
            {
                var exist = await _unitOfWork.ScheduleRepository.GetFetcherSchedule(entity.Fetcher_ID, entity.Schedule_ID);

                if (exist != null)
                    return CreateResult("409", FETCHER_SCHEDULE_STUDENT, false);

                await _unitOfWork.ScheduleRepository.AddFetcherSchedule(entity);

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Added_By, (int)Form.Fetcher_Assign, "Add Fetcher Schedule", "INSERT", true, "Success: " + entity.Fetcher_ID, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Fetcher Schedule" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteFetcherSchedule(int id, int user)
        {
            try
            {
                fetcherScheduleEntity entity = await _unitOfWork.ScheduleRepository.GetFetcherScheduleByID(id);

                await _unitOfWork.ScheduleRepository.DeleteFetcherSchedule(entity, user);

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Updated_By, (int)Form.Fetcher_Assign, "Delete Fetcher Schedule", "DELETE FETCHER SCHEDULE", true, "Success: " + entity.Fetcher_Sched_ID, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Fetcher Schedule " + Constants.SuccessMessageDelete;
                result.isSuccess = true;

                return result;
            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }

        public async Task<fetcherScheduleDetailsPagedResult> GetStudentByFetcherScheduleID(string id, int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.ScheduleRepository.GetStudentByFetcherScheduleID(id, pageNo, pageSize, keyword);
        }
        
        public async Task<fetcherScheduleDetailsPagedResultVM> GetGroupByFetcherScheduleID(string id, int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.ScheduleRepository.GetGroupByFetcherScheduleID(id, pageNo, pageSize, keyword);
        }
        
        public async Task<ResultModel> AddFetcherScheduleStudent(fetcherScheduleDetailsEntity entity)
        {
            try
            {
                var exist = await _unitOfWork.ScheduleRepository.GetFetcherScheduleStudent(entity.Fetcher_Sched_ID, entity.Fetcher_Group_ID, entity.Person_ID);

                if (exist != null)
                    return CreateResult("409", FETCHER_SCHEDULE_STUDENT, false);

                var group = await _unitOfWork.ScheduleRepository.GetGroupsByScheduleGroupId(entity.Fetcher_Group_ID).ToListAsync();

                if (entity.Fetcher_Group_ID == 0)
                {
                    var fetchers = await _unitOfWork.FetcherScheduleDetailsRepository.FindAllAsync(q => q.Person_ID == entity.Person_ID && q.IsActive == true);

                    if (fetchers.Count >= 5)
                        return CreateResult("409", FETCHER_MAX_LIMIT, false);
                }
                else
                {
                    var students = string.Empty;
                    int counter = 0;

                    foreach (var groupVM in group)
                    {
                        var fetchers = await _unitOfWork.FetcherScheduleDetailsRepository.FindAllAsync(q => q.Person_ID == groupVM.Person_ID && q.IsActive == true);

                        if (fetchers.Count >= 5)
                        {
                            var studentInfo = await _unitOfWork.PersonRepository.FindAsync(q => q.Person_ID == groupVM.Person_ID);
                            students += ", " + studentInfo.ID_Number;
                            counter++;
                        }
                    }

                    if (students != string.Empty)
                    {
                        students = students.Remove(0, 2);

                        if (counter > 1)
                            return CreateResult("409", "Students (" + students + ") has reached the maximum number of allowed fetchers!", false);
                        else
                            return CreateResult("409", "Student " + students + " has reached the maximum number of allowed fetchers!", false);
                    }
                }


                //check fetcher if already has an issued card
                //start
                var checkFetcherSched = await _unitOfWork.FetcherScheduleRepository.FindAsync(q => q.Fetcher_Sched_ID == entity.Fetcher_Sched_ID && q.IsActive == true);
                var checkFetcherSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == checkFetcherSched.Fetcher_ID && q.IsActive == true);
                //end

                if (checkFetcherSerial == null || checkFetcherSerial.Equals(""))
                {
                    return CreateResult("409", "Unable to proceed. Fetcher has no issued card.", false);
                } 
                else 
                {
                    if (!group.Count.Equals(0))
                    {
                        var terminals = await _unitOfWork.TerminalRepository.FindByAsyn(q => q.Terminal_Category == Encryption.Encrypt("Time And Attendance Terminal") && q.IsActive == true && q.IsForFetcher == false);

                        var fetcherDataSyncTerminals = await _unitOfWork.TerminalRepository.FindByAsyn(q => q.Terminal_Category == Encryption.Encrypt("Time And Attendance Terminal") && q.IsActive == true);

                        //var fetcherTerminals = await _unitOfWork.TerminalRepository.FindByAsyn(q => q.Terminal_Category == Encryption.Encrypt("Time And Attendance Terminal") && q.IsActive == true && q.IsForFetcher == true);

                        foreach (var groupVM in group)
                        {
                            fetcherScheduleDetailsEntity fetcherScheduleDetails = new fetcherScheduleDetailsEntity();

                            fetcherScheduleDetails.Fetcher_Sched_ID = entity.Fetcher_Sched_ID;
                            fetcherScheduleDetails.Fetcher_Group_ID = groupVM.Fetcher_Group_ID;
                            fetcherScheduleDetails.Person_ID = groupVM.Person_ID;

                            fetcherScheduleDetails.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                            fetcherScheduleDetails.Last_Updated = DateTime.UtcNow.ToLocalTime();

                            fetcherScheduleDetails.User_ID = entity.User_ID;
                            fetcherScheduleDetails.Added_By = entity.Added_By;
                            fetcherScheduleDetails.Updated_By = entity.Updated_By;

                            fetcherScheduleDetails.IsActive = true;
                            fetcherScheduleDetails.ToDisplay = true;

                            await _unitOfWork.ScheduleRepository.AddFetcherScheduleStudent(fetcherScheduleDetails);

                            var studentSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == groupVM.Person_ID && q.IsActive == true);

                            var studentInfo = await _unitOfWork.PersonRepository.FindAsync(q => q.Person_ID == groupVM.Person_ID);

                            var fetcherSched = await _unitOfWork.FetcherScheduleRepository.FindAsync(q => q.Fetcher_Sched_ID == entity.Fetcher_Sched_ID && q.IsActive == true);

                            var fetcherSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == fetcherSched.Fetcher_ID && q.IsActive == true);

                            var fetcherInfo = await _unitOfWork.PersonRepository.FindAsync(q => q.Person_ID == fetcherSched.Fetcher_ID);

                            var schedule = await _unitOfWork.ScheduleRepository.FindAsync(q => q.Schedule_ID == fetcherSched.Schedule_ID && q.IsActive == true);

                            foreach (var terminal in fetcherDataSyncTerminals)
                            {
                                datasyncFetcherEntity newDatasyncFetcher = new datasyncFetcherEntity();
                                newDatasyncFetcher.DS_Action = "A";
                                newDatasyncFetcher.FetcherID = fetcherInfo.ID_Number;
                                newDatasyncFetcher.FetcherSerial = fetcherSerial.Card_Serial;
                                newDatasyncFetcher.StudID = studentInfo.ID_Number;
                                newDatasyncFetcher.StudentSerial = studentSerial.Card_Serial;

                                string schedDays = schedule.Schedule_Days;

                                if (schedDays.Contains("MON"))
                                    schedDays = schedDays.Replace("MON", "1");
                                if (schedDays.Contains("TUE"))
                                    schedDays = schedDays.Replace("TUE", "2");
                                if (schedDays.Contains("WED"))
                                    schedDays = schedDays.Replace("WED", "3");
                                if (schedDays.Contains("THU"))
                                    schedDays = schedDays.Replace("THU", "4");
                                if (schedDays.Contains("FRI"))
                                    schedDays = schedDays.Replace("FRI", "5");
                                if (schedDays.Contains("SAT"))
                                    schedDays = schedDays.Replace("SAT", "6");
                                if (schedDays.Contains("SUN"))
                                    schedDays = schedDays.Replace("SUN", "0");

                                newDatasyncFetcher.SchedDays = schedDays;
                                newDatasyncFetcher.Terminal_ID = terminal.Terminal_ID;
                                newDatasyncFetcher.TimeFrom = schedule.Schedule_Time_From;
                                newDatasyncFetcher.TimeTo = schedule.Schedule_Time_To;
                                newDatasyncFetcher.User_ID = entity.User_ID;

                                await _unitOfWork.DatasyncFetcherRepository.AddAsyn(newDatasyncFetcher);
                            }

                            foreach (var terminal in terminals)
                            {
                                datasyncEntity newDatasyncEntity = new datasyncEntity();
                                newDatasyncEntity.Card_Serial = studentSerial.Card_Serial;
                                newDatasyncEntity.DS_Action = "U";
                                newDatasyncEntity.Expiry_Date = studentSerial.Expiry_Date;
                                newDatasyncEntity.Person_Type = "S";
                                newDatasyncEntity.Terminal_ID = terminal.Terminal_ID;
                                newDatasyncEntity.User_ID = entity.User_ID;
                                newDatasyncEntity.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                                newDatasyncEntity.Added_By = entity.User_ID;
                                newDatasyncEntity.Last_Updated = DateTime.UtcNow.ToLocalTime();
                                newDatasyncEntity.Updated_By = entity.User_ID;
                                newDatasyncEntity.ToDisplay = true;
                                newDatasyncEntity.IsActive = true;

                                await _unitOfWork.DataSyncRepository.AddAsyn(newDatasyncEntity);
                            }

                            //foreach(var terminal in fetcherTerminals)
                            //{
                            //    datasyncEntity newDatasyncEntity = new datasyncEntity();
                            //    newDatasyncEntity.Card_Serial = fetcherSerial.Card_Serial;
                            //    newDatasyncEntity.DS_Action = "U";
                            //    newDatasyncEntity.Expiry_Date = fetcherSerial.Expiry_Date;
                            //    newDatasyncEntity.Person_Type = "F";
                            //    newDatasyncEntity.Terminal_ID = terminal.Terminal_ID;
                            //    newDatasyncEntity.User_ID = entity.User_ID;
                            //    newDatasyncEntity.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                            //    newDatasyncEntity.Added_By = entity.User_ID;
                            //    newDatasyncEntity.Last_Updated = DateTime.UtcNow.ToLocalTime();
                            //    newDatasyncEntity.Updated_By = entity.User_ID;
                            //    newDatasyncEntity.ToDisplay = true;
                            //    newDatasyncEntity.IsActive = true;

                            //    await _unitOfWork.DataSyncRepository.AddAsyn(newDatasyncEntity);
                            //}
                        }
                    }
                    else
                    {
                        if (entity.Person_ID == 0)
                        {
                            return CreateResult("409", "Unable to proceed. No student in this group.", false);
                        } 
                        else 
                        {
                            var checkStudentSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == entity.Person_ID && q.IsActive == true);

                            if (checkStudentSerial == null || checkStudentSerial.Equals(""))
                                return CreateResult("409", "No card issued.", false);

                            entity.Date_Time_Added = DateTime.Now;
                            entity.Last_Updated = DateTime.Now;

                            entity.IsActive = true;
                            entity.ToDisplay = true;

                            await _unitOfWork.ScheduleRepository.AddFetcherScheduleStudent(entity);

                            var terminals = await _unitOfWork.TerminalRepository.FindByAsyn(q => q.Terminal_Category == Encryption.Encrypt("Time And Attendance Terminal") && q.IsActive == true && q.IsForFetcher == false);

                            var fetcherTerminals = await _unitOfWork.TerminalRepository.FindByAsyn(q => q.Terminal_Category == Encryption.Encrypt("Time And Attendance Terminal") && q.IsActive == true);

                            var studentSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == entity.Person_ID && q.IsActive == true);

                            var studentInfo = await _unitOfWork.PersonRepository.FindAsync(q => q.Person_ID == entity.Person_ID);

                            var fetcherScheds = await _unitOfWork.FetcherScheduleRepository.FindAsync(q => q.Fetcher_Sched_ID == entity.Fetcher_Sched_ID && q.IsActive == true);

                            var fetcherSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == fetcherScheds.Fetcher_ID && q.IsActive == true);

                            var fetcherInfo = await _unitOfWork.PersonRepository.FindAsync(q => q.Person_ID == fetcherScheds.Fetcher_ID);

                            var schedule = await _unitOfWork.ScheduleRepository.FindAsync(q => q.Schedule_ID == fetcherScheds.Schedule_ID && q.IsActive == true);

                            foreach (var terminal in fetcherTerminals)
                            {
                                datasyncFetcherEntity newDatasyncFetcher = new datasyncFetcherEntity();
                                newDatasyncFetcher.DS_Action = "A";
                                newDatasyncFetcher.FetcherID = fetcherInfo.ID_Number;
                                newDatasyncFetcher.FetcherSerial = fetcherSerial.Card_Serial;
                                newDatasyncFetcher.StudID = studentInfo.ID_Number;
                                newDatasyncFetcher.StudentSerial = studentSerial.Card_Serial;

                                string schedDays = schedule.Schedule_Days;

                                if (schedDays.Contains("MON"))
                                    schedDays = schedDays.Replace("MON", "1");
                                if (schedDays.Contains("TUE"))
                                    schedDays = schedDays.Replace("TUE", "2");
                                if (schedDays.Contains("WED"))
                                    schedDays = schedDays.Replace("WED", "3");
                                if (schedDays.Contains("THU"))
                                    schedDays = schedDays.Replace("THU", "4");
                                if (schedDays.Contains("FRI"))
                                    schedDays = schedDays.Replace("FRI", "5");
                                if (schedDays.Contains("SAT"))
                                    schedDays = schedDays.Replace("SAT", "6");
                                if (schedDays.Contains("SUN"))
                                    schedDays = schedDays.Replace("SUN", "0");

                                newDatasyncFetcher.SchedDays = schedDays;
                                newDatasyncFetcher.Terminal_ID = terminal.Terminal_ID;
                                newDatasyncFetcher.TimeFrom = schedule.Schedule_Time_From;
                                newDatasyncFetcher.TimeTo = schedule.Schedule_Time_To;
                                newDatasyncFetcher.User_ID = entity.User_ID;

                                await _unitOfWork.DatasyncFetcherRepository.AddAsyn(newDatasyncFetcher);
                            }

                            foreach (var terminal in terminals)
                            {
                                datasyncEntity newDatasyncEntity = new datasyncEntity();
                                newDatasyncEntity.Card_Serial = studentSerial.Card_Serial;
                                newDatasyncEntity.DS_Action = "U";
                                newDatasyncEntity.Expiry_Date = studentSerial.Expiry_Date;
                                newDatasyncEntity.Person_Type = "S";
                                newDatasyncEntity.Terminal_ID = terminal.Terminal_ID;
                                newDatasyncEntity.User_ID = entity.User_ID;
                                newDatasyncEntity.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                                newDatasyncEntity.Added_By = entity.User_ID;
                                newDatasyncEntity.Last_Updated = DateTime.UtcNow.ToLocalTime();
                                newDatasyncEntity.Updated_By = entity.User_ID;
                                newDatasyncEntity.ToDisplay = true;
                                newDatasyncEntity.IsActive = true;

                                await _unitOfWork.DataSyncRepository.AddAsyn(newDatasyncEntity);
                            }
                        }
                    }

                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Added_By, (int)Form.Fetcher_Assign, "Add Fetcher Schedule", "INSERT", true, "Success: " + entity.Fetcher_Sched_Dtl_ID, DateTime.UtcNow.ToLocalTime()));

                    if (entity.Fetcher_Group_ID == 0)
                        return CreateResult("200", "Fetcher Schedule Student" + Constants.SuccessMessageAdd, true);
                    else
                        return CreateResult("200", "Fetcher Schedule Group" + Constants.SuccessMessageAdd, true);
                }
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteFetcherScheduleStudent(int id, int user)
        {
            try
            {
                fetcherScheduleDetailsEntity entity = await _unitOfWork.ScheduleRepository.GetFetcherScheduleStudentByID(id);

                var group = await _unitOfWork.ScheduleRepository.GetGroupsByScheduleGroupId(entity.Fetcher_Group_ID).ToListAsync();

                if (!group.Count.Equals(0))
                {
                    foreach (var groupVM in group)
                    {
                        await _unitOfWork.ScheduleRepository.DeleteFetcherScheduleStudent(entity, user);
                    }
                }
                else
                {
                    await _unitOfWork.ScheduleRepository.DeleteFetcherScheduleStudent(entity, user);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Updated_By, (int)Form.Fetcher_Assign, "Delete Fetcher Schedule Details", "DELETE FETCHER SCHEDULE DETAILS", true, "Success: " + entity.Fetcher_Sched_Dtl_ID, DateTime.UtcNow.ToLocalTime()));
                
                return CreateResult("200", "Fetcher Schedule Details" + Constants.SuccessMessageDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteFetcherScheduleGroup(int id, int user)
        {
            try
            {
                List<fetcherScheduleDetailsEntity> entity = await _unitOfWork.ScheduleRepository.GetFetcherScheduleGroup(id);

                await _unitOfWork.ScheduleRepository.DeleteFetcherScheduleGroup(entity, user);

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Fetcher_Assign, "Delete Fetcher Schedule Details", "DELETE FETCHER SCHEDULE DETAILS", true, "Success: " + entity[0].Fetcher_Group_ID, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Fetcher Schedule Details" + Constants.SuccessMessageDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }
    }
}
