using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyCampusV2.Services.Services
{
    public class StudentSectionService : BaseService, IStudentSectionService
    {
        private string _studentSectionBatch = AppDomain.CurrentDomain.BaseDirectory + @"Section\";
        private string _formName = "Section";

        public StudentSectionService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<IList<studentSectionEntity>> GetStudentSectionsUsingYearSectionId(int id)
        {
            return await _unitOfWork.StudentSectionRepository.GetStudentSectionsUsingYearSectionId(id);
        }

        public async Task<studentSectionEntity> GetStudentSectionById(int id)
        {
            return await _unitOfWork.StudentSectionRepository.GetStudentSectionById(id);
        }

        public async Task<ResultModel> AddStudentSection(studentSectionEntity studentSection)
        {
            try
            {
                var exist = await _unitOfWork.StudentSectionRepository.FindAsync(q => q.Description == studentSection.Description && q.ToDisplay == true && q.YearSec_ID == studentSection.YearSec_ID);

                if (exist != null)
                    return CreateResult("409", STUDENT_SECTION_EXIST, false);

                var data = await _unitOfWork.StudentSectionRepository.AddAsyncWithBase(studentSection);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(studentSection.Added_By, (int)Form.Campus_Section, "Add " + _formName, "INSERT", false, "Failed: " + studentSection.Description, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(studentSection.Added_By, (int)Form.Campus_Section, "Add " + _formName, "INSERT", true, "Success: " + studentSection.Description, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateStudentSection(studentSectionEntity studentSection)
        {
            try
            {
                var exist = await _unitOfWork.StudentSectionRepository.FindAsync(q => q.Description == studentSection.Description && q.IsActive == true && q.YearSec_ID == studentSection.YearSec_ID && q.StudSec_ID != studentSection.StudSec_ID);

                if (exist != null)
                    return CreateResult("409", STUDENT_SECTION_EXIST, false);

                var updateCheck = await _unitOfWork.StudentSectionRepository.FindAsync(q => q.StudSec_ID == studentSection.StudSec_ID && q.YearSec_ID == studentSection.YearSec_ID);

                if (updateCheck == null)
                {
                    var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.StudSec_ID == studentSection.StudSec_ID);

                    if (recordCountPerson != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(studentSection.Updated_By, (int)Form.Campus_Section, "Update " + _formName, "UPDATE", false, "Failed due to active record: " + studentSection.Description, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }
                }

                var data = await _unitOfWork.StudentSectionRepository.UpdateAsyncWithBase(studentSection, studentSection.StudSec_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(studentSection.Updated_By, (int)Form.Campus_Section, "Update " + _formName, "UPDATE", false, "Failed: " + studentSection.Description, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(studentSection.Updated_By, (int)Form.Campus_Section, "Update " +_formName, "UPDATE", true, "Success: " + _formName + " ID: " + studentSection.StudSec_ID + " " + _formName + " Name: " + studentSection.Description, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageUpdate, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteStudentSectionPermanent(int id, int user)
        {
            try
            {
                studentSectionEntity section = await GetStudentSectionById(id);

                section.Updated_By = user;

                var data = await _unitOfWork.StudentSectionRepository.DeleteAsyncPermanent(section, section.StudSec_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(section.Updated_By, (int)Form.Campus_Section, "Delete Permanently " + _formName, "PERMANENT DELETE", false, "Failed: " + section.Description, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(section.Updated_By, (int)Form.Campus_Section, "Delete Permanently " + _formName, "PERMANENT DELETE", true, "Success: " + section.Description, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessagePermanentDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteStudentSectionTemporary(int id, int user)
        {
            try
            {
                studentSectionEntity section = await GetStudentSectionById(id);
                section.Updated_By = user;
                
                var checkIfStudentExists = await _unitOfWork.PersonRepository.FindAsync(a => a.StudSec_ID == id && a.Person_Type == "S" && a.IsActive == true);
                if (checkIfStudentExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Section, "Deactivate " + _formName, "DEACTIVATE", false, "Unable to deactivate " + section.Description + " due to existing active student/s.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                var data = await _unitOfWork.StudentSectionRepository.DeleteAsyncTemporary(section, section.StudSec_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(section.Updated_By, (int)Form.Campus_Section, "Deactivate " + _formName, "DEACTIVATE", false, "Failed: " + section.Description, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(section.Updated_By, (int)Form.Campus_Section, "Deactivate " + _formName, "DEACTIVATE", true, "Success: " + section.Description, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageTemporaryDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> RetrieveStudentSection(studentSectionEntity studentSection)
        {
            try
            {
                var newEntity = await _unitOfWork.StudentSectionRepository.GetAsync(studentSection.StudSec_ID);

                var checkIfYearLevelIsActive = await _unitOfWork.YearSectionRepository.FindAsync(a => a.YearSec_ID == newEntity.YearSec_ID);
                if (!checkIfYearLevelIsActive.IsActive)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(studentSection.Updated_By, (int)Form.Campus_Section, "Activate Student Section", "ACTIVATE STUDENT SECTION", false, "Unable to activate " + newEntity.Description + " due to inactive Year Level.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", UNABLE_ACTIVATE("Year Level", checkIfYearLevelIsActive.YearSec_Name), false);
                }

                newEntity.Updated_By = studentSection.Updated_By;

                var data = await _unitOfWork.StudentSectionRepository.RetrieveAsync(newEntity, newEntity.StudSec_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Section, "Activate Student Section", "ACTIVATE STUDENT SECTION", false, "Failed: " + newEntity.Description, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Student Section", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Section, "Activate Student Section", "ACTIVATE STUDENT SECTION", true, "Success: " + newEntity.Description, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Section" + Constants.SuccessMessageRetrieve, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<studentSecPagedResult> GetAllStudentSection(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.StudentSectionRepository.GetAllStudentSection(pageNo, pageSize, keyword);
        }

        public async Task<studentSecPagedResult> ExportSection(string keyword)
        {
            return await _unitOfWork.SectionReportRepository.ExportSection(keyword);
        }

        public bool IsValidTime(string thetime)
        {
            Regex checktime = new Regex(@"^(20|21|22|23|[01]d|d)(([:][0-5]d){1,2})$");
            return checktime.IsMatch(thetime);
        }

        public async Task<BatchUploadResponse> BatchUpload(ICollection<studentSectionBatchUploadVM> sections, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "Section_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = sections.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var sectionVM in sections)
            {
                i++;

                TimeSpan tsStartTime, tsEndTime, tsHalfDay;

                if (sectionVM.CampusName == null || sectionVM.CampusName == string.Empty)
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (sectionVM.EducationalLevelName.Trim() == null || sectionVM.EducationalLevelName.Trim() == string.Empty)
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (sectionVM.YearLevelName.Trim() == null || sectionVM.YearLevelName.Trim() == string.Empty)
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Year Level Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (sectionVM.SectionName.Trim() == null || sectionVM.SectionName.Trim() == string.Empty)
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Section Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (sectionVM.SectionName.Trim().Length > 200)
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Section Name accepts 200 characters only.");
                    response.Failed++;
                    continue;
                }

                if (sectionVM.StartTime == null || sectionVM.StartTime == string.Empty)
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Start Time is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (IsValidTime(sectionVM.StartTime))
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Start Time is invalid.");
                    response.Failed++;
                    continue;
                }
                else if (!TimeSpan.TryParse(sectionVM.StartTime, out tsStartTime))
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Start Time is invalid.");
                    response.Failed++;
                    continue;
                }
                else if (sectionVM.StartTime == "00:00")
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Start Time is required.");
                    response.Failed++;
                    continue;
                }

                if (sectionVM.EndTime == null || sectionVM.EndTime == string.Empty)
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> End Time is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (IsValidTime(sectionVM.EndTime))
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> End Time is invalid.");
                    response.Failed++;
                    continue;
                }
                else if (!TimeSpan.TryParse(sectionVM.EndTime, out tsEndTime))
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> End Time is invalid.");
                    response.Failed++;
                    continue;
                }
                else if (sectionVM.EndTime == "00:00")
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> End Time is required.");
                    response.Failed++;
                    continue;
                }

                if (sectionVM.HalfDay == null || sectionVM.HalfDay == string.Empty)
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Half Day is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (IsValidTime(sectionVM.HalfDay))
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Half Day is invalid.");
                    response.Failed++;
                    continue;
                }
                else if (!TimeSpan.TryParse(sectionVM.HalfDay, out tsHalfDay))
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Half Day is invalid.");
                    response.Failed++;
                    continue;
                }
                else if (sectionVM.HalfDay == "00:00")
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Half Day is required.");
                    response.Failed++;
                    continue;
                }

                    int checkGracePeriod;
                if (sectionVM.GracePeriod.Trim() == null || sectionVM.GracePeriod.Trim() == string.Empty)
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Grace Period is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (sectionVM.GracePeriod.Trim().Length > 3 || !int.TryParse(sectionVM.GracePeriod.Trim(), out checkGracePeriod))
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Grace Period is invalid.");
                    response.Failed++;
                    continue;
                }

                var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == sectionVM.CampusName && x.ToDisplay == true);

                if (campus == null)
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Campus " + sectionVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                var educlevel = await _unitOfWork.EducationLevelRepository.FindAsync(x => x.CampusEntity.Campus_Name == sectionVM.CampusName 
                                    && x.Level_Name == sectionVM.EducationalLevelName && x.IsActive == true);

                if (educlevel == null)
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level " + sectionVM.EducationalLevelName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                var yearlevel = await _unitOfWork.YearSectionRepository.FindAsync(x => x.EducationalLevelEntity.CampusEntity.Campus_Name == sectionVM.CampusName
                                    && x.EducationalLevelEntity.Level_Name == sectionVM.EducationalLevelName && x.YearSec_Name == sectionVM.YearLevelName && x.IsActive == true);

                if (yearlevel == null)
                {
                    importLog.Logging(_studentSectionBatch, fileName, "Row: " + i.ToString() + " ---> Year Level " + sectionVM.YearLevelName + " does not exist.");
                    response.Failed++;
                    continue;
                }
                
                studentSectionEntity section = await _unitOfWork.StudentSectionRepository.FindAsync(x => x.Description == sectionVM.SectionName 
                                                        && x.YearSec_ID == yearlevel.YearSec_ID);
                
                if (section != null)
                {
                    section.Start_Time = tsStartTime;
                    section.End_Time = tsEndTime;
                    section.Half_Day = tsHalfDay;
                    section.YearSec_ID = yearlevel.YearSec_ID;
                    section.Updated_By = user;
                    section.IsActive = true;

                    var isSuccess = await _unitOfWork.StudentSectionRepository.UpdateAsyncWithBase(section, section.StudSec_ID);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Section, "Update " + _formName + " By Batch", "UPDATE", true, "Success: " + sectionVM.SectionName, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_studentSectionBatch, fileName, _formName + " " + sectionVM.SectionName + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_studentSectionBatch, fileName, _formName + " " + sectionVM.SectionName + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    section = new studentSectionEntity();
                    section.Description = sectionVM.SectionName;
                    section.Start_Time = tsStartTime;
                    section.End_Time = tsEndTime;
                    section.Half_Day = tsHalfDay;
                    section.Grace_Period = sectionVM.GracePeriod;
                    section.YearSec_ID = yearlevel.YearSec_ID;
                    section.IsActive = true;
                    section.Added_By = user;
                    section.Updated_By = user;

                    var isSuccess = await _unitOfWork.StudentSectionRepository.AddAsyncWithBase(section);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Section, "Insert " + _formName + " By Batch", "INSERT", true, "Success: " + sectionVM.SectionName, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_studentSectionBatch, fileName, _formName + " " + sectionVM.SectionName + " successfully added.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_studentSectionBatch, fileName, _formName + " " + sectionVM.SectionName + " failed to add.");
                        response.Failed++;
                        continue;
                    }
                }
            }

            return response;
        }

        public async Task<sectionSchedulePagedResult> GetAllSchedule(int sectionId, int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.SectionScheduleRepository.GetAllSchedule(sectionId, pageNo, pageSize, keyword);
        }

        public async Task<ResultModel> AddSectionSchedule(sectionScheduleEntity sectionSchedule)
        {
            try
            {
                _formName = "Section Schedule";

                DateTime currDate = sectionSchedule.Schedule_Start_Date;

                while (sectionSchedule.Schedule_End_Date >= currDate)
                {
                    var exist = await _unitOfWork.SectionScheduleRepository.FindAsync(q => q.Schedule_Date == currDate && q.StudSec_ID == sectionSchedule.StudSec_ID && q.IsActive == true);

                    if (exist != null)
                        return CreateResult("409", SECTION_RANGE_SCHEDULE_EXIST, false);

                    currDate = currDate.AddDays(1);
                }

                sectionSchedule.Grace_Period = sectionSchedule.Grace_Period == null || sectionSchedule.Grace_Period == string.Empty ? "0" : sectionSchedule.Grace_Period;
                currDate = sectionSchedule.Schedule_Start_Date;

                var data = new sectionScheduleEntity();
                while (sectionSchedule.Schedule_End_Date >= currDate)
                {
                    var newSchedule = new sectionScheduleEntity();
                    newSchedule.StudSec_ID = sectionSchedule.StudSec_ID;
                    newSchedule.Schedule_Date = currDate;
                    newSchedule.Start_Time = sectionSchedule.Start_Time;
                    newSchedule.End_Time = sectionSchedule.End_Time;
                    newSchedule.Half_Day = sectionSchedule.Half_Day;
                    newSchedule.Grace_Period = sectionSchedule.Grace_Period;
                    newSchedule.IsExcused = sectionSchedule.IsExcused;
                    newSchedule.Remarks = sectionSchedule.Remarks;
                    newSchedule.Added_By = sectionSchedule.Added_By;
                    newSchedule.Updated_By = sectionSchedule.Updated_By;

                    data = await _unitOfWork.SectionScheduleRepository.AddAsyncWithBase(newSchedule);
                    currDate = currDate.AddDays(1);
                }

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(sectionSchedule.Added_By, (int)Form.Campus_Section, "Add " + _formName, "INSERT", false, "Failed: Section " + sectionSchedule.StudSec_ID + " Schedule: " + sectionSchedule.Schedule_Start_Date + "-" + sectionSchedule.Schedule_End_Date, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(sectionSchedule.Added_By, (int)Form.Campus_Section, "Add " + _formName, "INSERT", true, "Success: Section " + sectionSchedule.StudSec_ID + " Schedule: " + sectionSchedule.Schedule_Start_Date + "-" + sectionSchedule.Schedule_End_Date, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateSectionSchedule(sectionScheduleEntity sectionSchedule)
        {
            try
            {
                _formName = "Section Schedule";
                sectionSchedule.IsActive = true;
                
                var data = await _unitOfWork.SectionScheduleRepository.UpdateAsyncWithBase(sectionSchedule, sectionSchedule.ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(sectionSchedule.Updated_By, (int)Form.Campus_Section, "Update " + _formName, "UPDATE", false, "Failed: Section " + sectionSchedule.StudSec_ID + " Schedule: " + sectionSchedule.Schedule_Date, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(sectionSchedule.Updated_By, (int)Form.Campus_Section, "Update " + _formName, "UPDATE", true, "Success: Section " + sectionSchedule.StudSec_ID + " Schedule: " + sectionSchedule.Schedule_Date, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageUpdate, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteSectionSchedule(int id, int user)
        {
            try
            {
                _formName = "Section Schedule";
                sectionScheduleEntity schedule = await GetSectionScheduleById(id);

                schedule.Updated_By = user;

                await _unitOfWork.SectionScheduleRepository.DeleteAsyn(schedule);
                
                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(schedule.Updated_By, (int)Form.Campus_Section, "Delete Permanently " + _formName, "PERMANENT DELETE", true, "Success: Section" + schedule.StudentSectionEntity.Description + " Schedule: " + schedule.Schedule_Date, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<sectionScheduleEntity> GetSectionScheduleById(int id)
        {
            return await _unitOfWork.SectionScheduleRepository.GetSectionScheduleById(id);
        }
    }
}
