using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyCampusV2.Services.Services
{
    public class SchoolCalendarService : BaseService, ISchoolCalendarService
    {
        private string _schoolCalendarBatch = AppDomain.CurrentDomain.BaseDirectory + @"SchoolCalendar\";
        private string _formName = "School Calendar";

        public SchoolCalendarService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<schoolCalendarResult> GetCalendarDates(string schoolyear)
        {
            return await _unitOfWork.SchoolCalendarRepository.GetCalendarDates(schoolyear);
        }
        public async Task<List<schoolCalendarDatesVM>> GetCalendarList(string schoolyear)
        {
            return await _unitOfWork.SchoolCalendarRepository.GetCalendarList(schoolyear);
        }


        public async Task<BatchUploadResponse> BatchUpload(ICollection<schoolCalendarBatchUploadVM> calendarlist, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();
            bool isSuccess;
            int loopOnce = 1;

            string fileName = "School_Calendar_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = calendarlist.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var calendarVM in calendarlist)
            {
                i++;
                
                if (calendarVM.Year == 0)
                {
                    importLog.Logging(_schoolCalendarBatch, fileName, "Row: " + i.ToString() + " ---> Year is a required field.");
                    response.Failed++;
                    continue;
                }

                if (calendarVM.Month == 0)
                {
                    importLog.Logging(_schoolCalendarBatch, fileName, "Row: " + i.ToString() + " ---> Month is a required field.");
                    response.Failed++;
                    continue;
                }
                
                schoolCalendarEntity calendar = await _unitOfWork.SchoolCalendarRepository.FindAsync(x => x.School_Year == calendarVM.SchoolYear
                                                        && x.Year == calendarVM.Year && x.Month == calendarVM.Month);

                if (calendar != null)
                {
                    calendar.Days = calendarVM.Days;

                    isSuccess = await _unitOfWork.SchoolCalendarRepository.UpdateWithBoolReturn(calendar, user);

                    if (loopOnce == 1)
                    {
                        var exist = await _unitOfWork.SchoolYearRepository.FindAsync(q => q.School_Year == calendarVM.SchoolYear);

                        if (exist == null)
                        {
                            var schoolYear = new schoolYearEntity
                            {
                                School_Year = calendarVM.SchoolYear,
                                Start_Date = null,
                                End_Date = null,
                                IsActive = true,
                                ToDisplay = true,
                                Added_By = user,
                                Updated_By = user,
                                Date_Time_Added = DateTime.UtcNow.ToLocalTime(),
                                Last_Updated = DateTime.UtcNow.ToLocalTime()
                            };
                            var data = await _unitOfWork.SchoolYearRepository.AddAsyncWithBase(schoolYear);

                            if (data == null)
                            {
                                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_School_Calendar, "Add School Year", "INSERT", false, "Failed: School Year " + calendarVM.SchoolYear, DateTime.UtcNow.ToLocalTime()));

                                importLog.Logging(_schoolCalendarBatch, fileName, "School Year " + calendarVM.SchoolYear + " has not been added.");
                                response.Failed++;
                                continue;
                            }
                        }
                        loopOnce++;
                    }

                    if (isSuccess)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_School_Calendar, "Update " + _formName, "UPDATE", true, "Success: " + calendarVM.SchoolYear + " for " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(calendarVM.Month), DateTime.UtcNow.ToLocalTime()));

                        importLog.Logging(_schoolCalendarBatch, fileName, _formName + " " + calendarVM.SchoolYear + " for " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(calendarVM.Month) + " has been successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_School_Calendar, "Update " + _formName, "UPDATE", true, "Failed: " + calendarVM.SchoolYear + " for " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(calendarVM.Month), DateTime.UtcNow.ToLocalTime()));

                        importLog.Logging(_schoolCalendarBatch, fileName, _formName + " " + calendarVM.SchoolYear + " for " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(calendarVM.Month) + " has not been updated.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    calendar = new schoolCalendarEntity();
                    calendar.School_Year = calendarVM.SchoolYear;
                    calendar.Year = calendarVM.Year;
                    calendar.Month = calendarVM.Month;
                    calendar.Days = calendarVM.Days;

                    isSuccess = await _unitOfWork.SchoolCalendarRepository.AddWithBoolReturn(calendar, user);

                    if (loopOnce == 1)
                    {
                        var exist = await _unitOfWork.SchoolYearRepository.FindAsync(q => q.School_Year == calendarVM.SchoolYear);

                        if (exist == null)
                        {
                            var schoolYear = new schoolYearEntity
                            {
                                School_Year = calendarVM.SchoolYear,
                                Start_Date = null,
                                End_Date = null,
                                IsActive = true,
                                ToDisplay = true,
                                Added_By = user,
                                Updated_By = user,
                                Date_Time_Added = DateTime.UtcNow.ToLocalTime(),
                                Last_Updated = DateTime.UtcNow.ToLocalTime()
                            };
                            var data = await _unitOfWork.SchoolYearRepository.AddAsyncWithBase(schoolYear);

                            if (data == null)
                            {
                                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_School_Calendar, "Insert School Year", "INSERT", false, "Failed: School Year " + calendarVM.SchoolYear, DateTime.UtcNow.ToLocalTime()));

                                importLog.Logging(_schoolCalendarBatch, fileName, "School Year " + calendarVM.SchoolYear + " has not been added.");
                                response.Failed++;
                                continue;
                            }
                        }
                        loopOnce++;
                    }

                    if (isSuccess)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_School_Calendar, "Insert " + _formName, "INSERT", true, "Success: " + calendarVM.SchoolYear + " for " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(calendarVM.Month), DateTime.UtcNow.ToLocalTime()));
                        
                        importLog.Logging(_schoolCalendarBatch, fileName, _formName + " " + calendarVM.SchoolYear + " for " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(calendarVM.Month) + " has been successfully added.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_School_Calendar, "Insert " + _formName, "INSERT", true, "Failed: " + calendarVM.SchoolYear + " for " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(calendarVM.Month), DateTime.UtcNow.ToLocalTime()));
                        
                        importLog.Logging(_schoolCalendarBatch, fileName, _formName + " " + calendarVM.SchoolYear + " for " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(calendarVM.Month) + " has not been added.");
                        response.Failed++;
                        continue;
                    }
                }
            }

            return response;
        }

        public async Task<schoolYearPagedResult> GetAll(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.SchoolYearRepository.GetAll(pageNo, pageSize, keyword);
        }

        public async Task<schoolYearPagedResult> GetFiltered()
        {
            return await _unitOfWork.SchoolYearRepository.GetFiltered();
        }

        public async Task<ResultModel> AddSchoolYear(schoolYearEntity schoolYear)
        {
            try
            {
                var exist = await _unitOfWork.SchoolYearRepository.FindAsync(q => q.School_Year == schoolYear.School_Year);

                if (exist != null)
                    return CreateResult("409", SCHOOL_YEAR_EXIST, false);

                var data = await _unitOfWork.SchoolYearRepository.AddAsyncWithBase(schoolYear);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(schoolYear.Added_By, (int)Form.Campus_School_Calendar, "Add " + _formName, "INSERT", false, "Failed: School Year " + schoolYear.School_Year, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(schoolYear.Added_By, (int)Form.Campus_School_Calendar, "Add " + _formName, "INSERT", true, "Success: School Year " + schoolYear.School_Year, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateSchoolYear(schoolYearEntity schoolYear)
        {
            try
            {
                var data = await _unitOfWork.SchoolYearRepository.UpdateAsyncWithBase(schoolYear, schoolYear.School_Year_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(schoolYear.Updated_By, (int)Form.Campus_School_Calendar, "Update " + _formName, "UPDATE", false, "Failed: School Year " + schoolYear.School_Year, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(schoolYear.Updated_By, (int)Form.Campus_School_Calendar, "Update " + _formName, "UPDATE", true, "Success: School Year " + schoolYear.School_Year, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageUpdate, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteSchoolYear(int id, int user)
        {
            try
            {
                schoolYearEntity schoolYear = await GetById(id);
                List<schoolCalendarEntity> calendar = await GetCalendarBySchoolYear(schoolYear.School_Year);

                schoolYear.Updated_By = user;

                await _unitOfWork.SchoolYearRepository.DeleteAsyn(schoolYear);

                foreach (var record in calendar)
                {
                    await _unitOfWork.SchoolCalendarRepository.DeleteAsyn(record);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(schoolYear.Updated_By, (int)Form.Campus_School_Calendar, "Delete Permanently " + _formName, "PERMANENT DELETE", true, "Success: School Year" + schoolYear.School_Year, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<schoolYearEntity> GetById(int id)
        {
            return await _unitOfWork.SchoolYearRepository.GetById(id);
        }

        public async Task<List<schoolCalendarEntity>> GetCalendarBySchoolYear(string schoolyear)
        {
            return await _unitOfWork.SchoolCalendarRepository.GetBySchoolYear(schoolyear);
        }
    }
}
