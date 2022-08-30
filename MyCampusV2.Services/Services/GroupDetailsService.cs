using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyCampusV2.Services.Services
{
    public class GroupDetailsService : BaseService, IGroupDetailsService
    {
        private ResultModel result = new ResultModel();

        public GroupDetailsService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<ResultModel> AddStudentToGroup(fetcherGroupDetailsEntity entity)
        {
            try
            {
                var exist = await _unitOfWork.GroupDetailsRepository.FindAsync(q => q.Fetcher_Group_ID == entity.Fetcher_Group_ID && q.Person_ID == entity.Person_ID);

                if (exist != null)
                    return CreateResult("409", FETCHER_GROUP_STUDENT, false);

                var fetchers = await _unitOfWork.FetcherScheduleDetailsRepository.FindAllAsync(q => q.Person_ID == entity.Person_ID && q.IsActive == true);

                if (fetchers.Count >= 5)
                    return CreateResult("409", FETCHER_MAX_LIMIT, false);

                var checkStudentSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == entity.Person_ID && q.IsActive == true);

                if (checkStudentSerial == null || checkStudentSerial.Equals(""))
                    return CreateResult("409", "No card issued.", false);

                var data = await _unitOfWork.GroupDetailsRepository.AddAsyncWithBase(entity);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Added_By, (int)Form.Fetcher_Group, "Add Student to Group", "INSERT", false, "Failed to assign Student: " + entity.Person_ID, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Student to Group", false);
                }

                var fetcherStudentByGroup = await _unitOfWork.ScheduleRepository.GetFetcherScheduleStudentByGroupID(entity.Fetcher_Group_ID).ToListAsync();

                if (fetcherStudentByGroup.Count >= 1)
                {
                    foreach (var groupVM in fetcherStudentByGroup)
                    {
                        //check fetcher if already has an issued card
                        //start
                        var checkFetcherSched = await _unitOfWork.FetcherScheduleRepository.FindAsync(q => q.Fetcher_Sched_ID == groupVM.Fetcher_Sched_ID && q.IsActive == true);
                        var checkFetcherSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == checkFetcherSched.Fetcher_ID && q.IsActive == true);
                        //end

                        if (checkFetcherSerial != null || !checkFetcherSerial.Equals(""))
                        {
                            var terminals = await _unitOfWork.TerminalRepository.FindByAsyn(q => q.Terminal_Category == Encryption.Encrypt("Time And Attendance Terminal") && q.IsActive == true && q.IsForFetcher == false);

                            var fetcherDataSyncTerminals = await _unitOfWork.TerminalRepository.FindByAsyn(q => q.Terminal_Category == Encryption.Encrypt("Time And Attendance Terminal") && q.IsActive == true);

                            fetcherScheduleDetailsEntity fetcherScheduleDetails = new fetcherScheduleDetailsEntity();

                            fetcherScheduleDetails.Fetcher_Sched_ID = groupVM.Fetcher_Sched_ID;
                            fetcherScheduleDetails.Fetcher_Group_ID = groupVM.Fetcher_Group_ID;
                            fetcherScheduleDetails.Person_ID = entity.Person_ID;

                            fetcherScheduleDetails.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                            fetcherScheduleDetails.Last_Updated = DateTime.UtcNow.ToLocalTime();

                            fetcherScheduleDetails.User_ID = groupVM.User_ID;
                            fetcherScheduleDetails.Added_By = groupVM.Added_By;
                            fetcherScheduleDetails.Updated_By = groupVM.Updated_By;

                            fetcherScheduleDetails.IsActive = true;
                            fetcherScheduleDetails.ToDisplay = true;

                            await _unitOfWork.ScheduleRepository.AddFetcherScheduleStudent(fetcherScheduleDetails);

                            var studentSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == entity.Person_ID && q.IsActive == true);

                            var studentInfo = await _unitOfWork.PersonRepository.FindAsync(q => q.Person_ID == entity.Person_ID);

                            var fetcherSched = await _unitOfWork.FetcherScheduleRepository.FindAsync(q => q.Fetcher_Sched_ID == groupVM.Fetcher_Sched_ID && q.IsActive == true);

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
                                newDatasyncFetcher.User_ID = groupVM.User_ID;

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
                                newDatasyncEntity.User_ID = groupVM.User_ID;
                                newDatasyncEntity.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                                newDatasyncEntity.Added_By = groupVM.User_ID;
                                newDatasyncEntity.Last_Updated = DateTime.UtcNow.ToLocalTime();
                                newDatasyncEntity.Updated_By = groupVM.User_ID;
                                newDatasyncEntity.ToDisplay = true;
                                newDatasyncEntity.IsActive = true;

                                await _unitOfWork.DataSyncRepository.AddAsyn(newDatasyncEntity);
                            }
                        }
                    }
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Added_By, (int)Form.Fetcher_Group, "Add Student to Group", "INSERT", true, "Successfully assigned Student: " + entity.Person_ID, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Student to Group" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteGroup(int id, int personId, int user)
        {
            try
            {
                var exist = await _unitOfWork.PersonRepository.FindAsync(q => q.Person_ID == personId);

                var groupdetailscount = await _unitOfWork.ScheduleRepository.GetGroupsByScheduleGroupId(id).ToListAsync();

                var checkFetcherStudentByGroup = await _unitOfWork.ScheduleRepository.GetFetcherScheduleStudentByGroupIDAndPersonID(id, personId).ToListAsync();

                if ((groupdetailscount.Count.Equals(1)) && (!checkFetcherStudentByGroup.Count.Equals(0)))
                {
                    return CreateResult("409", "This group is in use and cannot be empty.", false);
                }
                else
                {
                    fetcherGroupDetailsEntity details = await GetGroupDetailById(id, personId);

                    var data = await _unitOfWork.GroupDetailsRepository.DeleteAsyn(details);

                    if (data == 0)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(details.Updated_By, (int)Form.Fetcher_Group, "Delete Student to Group Permanently ", "PERMANENT DELETE", false, "Failed to remove Student: " + exist.ID_Number, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", "Student to Group", false);
                    }

                    var fetcherStudentByGroup = await _unitOfWork.ScheduleRepository.GetFetcherScheduleStudentByGroupIDAndPersonID(id, personId).ToListAsync();

                    if (fetcherStudentByGroup.Count.Equals(1))
                    {
                        foreach (var groupVM in fetcherStudentByGroup)
                        {
                            //check fetcher if already has an issued card
                            //start
                            var checkFetcherSched = await _unitOfWork.FetcherScheduleRepository.FindAsync(q => q.Fetcher_Sched_ID == groupVM.Fetcher_Sched_ID && q.IsActive == true);
                            var checkFetcherSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == checkFetcherSched.Fetcher_ID && q.IsActive == true);
                            //end

                            if (checkFetcherSerial != null || !checkFetcherSerial.Equals(""))
                            {
                                var terminals = await _unitOfWork.TerminalRepository.FindByAsyn(q => q.Terminal_Category == Encryption.Encrypt("Time And Attendance Terminal") && q.IsActive == true && q.IsForFetcher == false);

                                var fetcherDataSyncTerminals = await _unitOfWork.TerminalRepository.FindByAsyn(q => q.Terminal_Category == Encryption.Encrypt("Time And Attendance Terminal") && q.IsActive == true);

                                await _unitOfWork.ScheduleRepository.DeleteFetcherScheduleStudent(groupVM, user);

                                var studentSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == groupVM.Person_ID && q.IsActive == true);

                                var studentInfo = await _unitOfWork.PersonRepository.FindAsync(q => q.Person_ID == groupVM.Person_ID);

                                var fetcherSched = await _unitOfWork.FetcherScheduleRepository.FindAsync(q => q.Fetcher_Sched_ID == groupVM.Fetcher_Sched_ID && q.IsActive == true);

                                var fetcherSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == fetcherSched.Fetcher_ID && q.IsActive == true);

                                var fetcherInfo = await _unitOfWork.PersonRepository.FindAsync(q => q.Person_ID == fetcherSched.Fetcher_ID);

                                var schedule = await _unitOfWork.ScheduleRepository.FindAsync(q => q.Schedule_ID == fetcherSched.Schedule_ID && q.IsActive == true);

                                foreach (var terminal in fetcherDataSyncTerminals)
                                {
                                    datasyncFetcherEntity newDatasyncFetcher = new datasyncFetcherEntity();
                                    newDatasyncFetcher.DS_Action = "D";
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
                                    newDatasyncFetcher.User_ID = groupVM.User_ID;

                                    await _unitOfWork.DatasyncFetcherRepository.AddAsyn(newDatasyncFetcher);
                                }

                                foreach (var terminal in terminals)
                                {
                                    datasyncEntity newDatasyncEntity = new datasyncEntity();
                                    newDatasyncEntity.Card_Serial = studentSerial.Card_Serial;
                                    newDatasyncEntity.DS_Action = "U";
                                    newDatasyncEntity.Expiry_Date = studentSerial.Expiry_Date;
                                    newDatasyncEntity.Person_Type = null;
                                    newDatasyncEntity.Terminal_ID = terminal.Terminal_ID;
                                    newDatasyncEntity.User_ID = groupVM.User_ID;
                                    newDatasyncEntity.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                                    newDatasyncEntity.Added_By = groupVM.User_ID;
                                    newDatasyncEntity.Last_Updated = DateTime.UtcNow.ToLocalTime();
                                    newDatasyncEntity.Updated_By = groupVM.User_ID;
                                    newDatasyncEntity.ToDisplay = true;
                                    newDatasyncEntity.IsActive = true;

                                    await _unitOfWork.DataSyncRepository.AddAsyn(newDatasyncEntity);
                                }
                            }
                        }
                    }
                    else if (fetcherStudentByGroup.Count > 1)
                    {
                        foreach (var groupVM in fetcherStudentByGroup)
                        {
                            //check fetcher if already has an issued card
                            //start
                            var checkFetcherSched = await _unitOfWork.FetcherScheduleRepository.FindAsync(q => q.Fetcher_Sched_ID == groupVM.Fetcher_Sched_ID && q.IsActive == true);
                            var checkFetcherSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == checkFetcherSched.Fetcher_ID && q.IsActive == true);
                            //end

                            if (checkFetcherSerial != null || !checkFetcherSerial.Equals(""))
                            {
                                var terminals = await _unitOfWork.TerminalRepository.FindByAsyn(q => q.Terminal_Category == Encryption.Encrypt("Time And Attendance Terminal") && q.IsActive == true && q.IsForFetcher == false);

                                var fetcherDataSyncTerminals = await _unitOfWork.TerminalRepository.FindByAsyn(q => q.Terminal_Category == Encryption.Encrypt("Time And Attendance Terminal") && q.IsActive == true);

                                await _unitOfWork.ScheduleRepository.DeleteFetcherScheduleStudent(groupVM, user);

                                var studentSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == groupVM.Person_ID && q.IsActive == true);

                                var studentInfo = await _unitOfWork.PersonRepository.FindAsync(q => q.Person_ID == groupVM.Person_ID);

                                var fetcherSched = await _unitOfWork.FetcherScheduleRepository.FindAsync(q => q.Fetcher_Sched_ID == groupVM.Fetcher_Sched_ID && q.IsActive == true);

                                var fetcherSerial = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == fetcherSched.Fetcher_ID && q.IsActive == true);

                                var fetcherInfo = await _unitOfWork.PersonRepository.FindAsync(q => q.Person_ID == fetcherSched.Fetcher_ID);

                                var schedule = await _unitOfWork.ScheduleRepository.FindAsync(q => q.Schedule_ID == fetcherSched.Schedule_ID && q.IsActive == true);

                                foreach (var terminal in fetcherDataSyncTerminals)
                                {
                                    datasyncFetcherEntity newDatasyncFetcher = new datasyncFetcherEntity();
                                    newDatasyncFetcher.DS_Action = "D";
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
                                    newDatasyncFetcher.User_ID = groupVM.User_ID;

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
                                    newDatasyncEntity.User_ID = groupVM.User_ID;
                                    newDatasyncEntity.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                                    newDatasyncEntity.Added_By = groupVM.User_ID;
                                    newDatasyncEntity.Last_Updated = DateTime.UtcNow.ToLocalTime();
                                    newDatasyncEntity.Updated_By = groupVM.User_ID;
                                    newDatasyncEntity.ToDisplay = true;
                                    newDatasyncEntity.IsActive = true;

                                    await _unitOfWork.DataSyncRepository.AddAsyn(newDatasyncEntity);
                                }
                            }
                        }
                    }

                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(details.Updated_By, (int)Form.Fetcher_Group, "Delete Student to Group Permanently ", "PERMANENT DELETE", true, "Successfully removed Student: " + exist.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("200", "Student to Group" + Constants.SuccessMessagePermanentDelete, true);
                }
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<fetcherGroupDetailsPagedResult> GetAllStudentAssignedToGroup(int pageNo, int pageSize, string keyword, int groupId)
        {
            return await _unitOfWork.GroupDetailsRepository.GetAllStudentAssignedToGroup(pageNo, pageSize, keyword, groupId);
        }

        public async Task<fetcherGroupDetailsEntity> GetGroupDetailById(int id, int personId)
        {
            try
            {
                return await _unitOfWork.GroupDetailsRepository.GetGroupDetailById(id, personId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
