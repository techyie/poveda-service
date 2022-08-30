using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;

namespace MyCampusV2.Services
{
    public class EducationLevelService : BaseService, IEducationLevelService
    {
        private string _educLevelBatch = AppDomain.CurrentDomain.BaseDirectory + @"EducationalLevel\";
        private string _formName = "Educational Level";

        public EducationLevelService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<IList<educationalLevelEntity>> GetEducationalLevelsUsingCampusId(int id)
        {
            try
            {
                return await _unitOfWork.EducationLevelRepository.GetEducationalLevelsUsingCampusId(id);
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<IList<educationalLevelEntity>> GetEducationalLevelsCollegeOnlyUsingCampusId(int id)
        {
            try
            {
                return await _unitOfWork.EducationLevelRepository.GetEducationalLevelsCollegeOnlyUsingCampusId(id);
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<educationalLevelEntity> GetEducationalLevelById(int id)
        {
            try
            {
                return await _unitOfWork.EducationLevelRepository.GetEducationalLevelById(id);
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<ResultModel> AddEducationalLevel(educationalLevelEntity educLevel)
        {
            try
            {
                //educLevel.IsActive = educLevel.Level_Status == "Active" ? true : false;

                var exist = await _unitOfWork.EducationLevelRepository.FindAsync(q => q.Level_Name == educLevel.Level_Name && q.IsActive == true && q.Campus_ID == educLevel.Campus_ID);

                if (exist != null)
                    return CreateResult("409", EDUCATIONAL_LEVEL_EXIST, false);

                educLevel.Level_Status = "Active";
                educLevel.IsActive = true;

                var data = await _unitOfWork.EducationLevelRepository.AddAsyncWithBase(educLevel);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(educLevel.Added_By, (int)Form.Campus_EducationalLevel, "Add " + _formName, "INSERT", false, "Failed: " + educLevel.Level_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(educLevel.Added_By, (int)Form.Campus_EducationalLevel, "Add " + _formName, "INSERT", true, "Success: " + educLevel.Level_Name, DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", _formName + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateEducationalLevel(educationalLevelEntity educLevel)
        {
            try
            {
                //educLevel.IsActive = educLevel.Level_Status == "Active" ? true : false;
                var updateCheck = await _unitOfWork.EducationLevelRepository.FindAsync(q => q.Level_ID == educLevel.Level_ID && q.Campus_ID == educLevel.Campus_ID && q.hasCourse == educLevel.hasCourse);

                if (updateCheck == null)
                {
                    var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.Educ_Level_ID == educLevel.Level_ID);

                    if (recordCountPerson != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(educLevel.Updated_By, (int)Form.Campus_EducationalLevel, "Update " + _formName, "UPDATE", false, "Failed due to active record: " + educLevel.Level_Name, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }

                    var recordCountYearLevel = await _unitOfWork.YearSectionRepository.FindAsync(q => q.Level_ID == educLevel.Level_ID && q.IsActive == true);

                    if (recordCountYearLevel != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(educLevel.Updated_By, (int)Form.Campus_EducationalLevel, "Update " + _formName, "UPDATE", false, "Failed due to active record: " + educLevel.Level_Name, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }

                    var recordCountCollege = await _unitOfWork.CollegeRepository.FindAsync(q => q.Level_ID == educLevel.Level_ID && q.IsActive == true);

                    if (recordCountCollege != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(educLevel.Updated_By, (int)Form.Campus_EducationalLevel, "Update " + _formName, "UPDATE", false, "Failed due to active record: " + educLevel.Level_Name, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }
                }

                var data = await _unitOfWork.EducationLevelRepository.UpdateAsyncWithBase(educLevel, educLevel.Level_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(educLevel.Updated_By, (int)Form.Campus_EducationalLevel, "Update " + _formName, "UPDATE", false, "Failed: " + educLevel.Level_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(educLevel.Updated_By, (int)Form.Campus_EducationalLevel, "Update " + _formName, "UPDATE", true, "Success:  Educational Level Name: " + educLevel.Level_Name, DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", _formName + Constants.SuccessMessageUpdate, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteEducationalLevelPermanent(int id, int user)
        {
            try
            {
                educationalLevelEntity educationalLevel = await GetEducationalLevelById(id);

                var checkIfYearLevelExists = await _unitOfWork.YearSectionRepository.FindAsync(a => a.Level_ID == id && a.ToDisplay == true);
                if (checkIfYearLevelExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_EducationalLevel, "Permanent Delete " + _formName, "PERMANENT DELETE", false, "Unable to permanent delete " + educationalLevel.Level_Name + " due to existing active year level", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Unable to permanent delete.", false);
                }

                educationalLevel.Updated_By = user;

                var data = await _unitOfWork.EducationLevelRepository.DeleteAsyncPermanent(educationalLevel, educationalLevel.Level_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(educationalLevel.Updated_By, (int)Form.Campus_EducationalLevel, "Delete Permanently " + _formName, "PERMANENT DELETE", false, "Failed: " + educationalLevel.Level_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(educationalLevel.Updated_By, (int)Form.Campus_EducationalLevel, "Delete Permanently " + _formName, "PERMANENT DELETE", true, "Success: " + educationalLevel.Level_Name, DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", _formName + Constants.SuccessMessagePermanentDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteEducationalLevelTemporary(int id, int user)
        {
            try
            {
                educationalLevelEntity educationalLevel = await GetEducationalLevelById(id);

                var checkIfYearLevelExists = await _unitOfWork.YearSectionRepository.FindAsync(a => a.Level_ID == id && a.IsActive == true);
                if (checkIfYearLevelExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_EducationalLevel, "Deactivate " + _formName, "DEACTIVATE", false, "Unable to deactivate " + educationalLevel.Level_Name + " due to existing active year level/s.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                var checkIfCollegeExists = await _unitOfWork.CollegeRepository.FindAsync(a => a.Level_ID == id && a.IsActive == true);
                if (checkIfCollegeExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_EducationalLevel, "Deactivate " + _formName, "DEACTIVATE", false, "Unable to deactivate " + educationalLevel.Level_Name + " due to existing active college/s.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                educationalLevel.Updated_By = user;
                educationalLevel.Level_Status = "Inactive";

                var data = await _unitOfWork.EducationLevelRepository.DeleteAsyncTemporary(educationalLevel, educationalLevel.Level_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(educationalLevel.Updated_By, (int)Form.Campus_EducationalLevel, "Deactivate " + _formName, "DEACTIVATE", false, "Failed: " + educationalLevel.Level_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(educationalLevel.Updated_By, (int)Form.Campus_EducationalLevel, "Deactivate " + _formName, "DEACTIVATE", true, "Success: " + educationalLevel.Level_Name, DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", _formName + Constants.SuccessMessageTemporaryDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> RetrieveEducationalLevel(educationalLevelEntity educationalLevel)
        { 
            try
            {
                educationalLevelEntity newEntity = await _unitOfWork.EducationLevelRepository.FindAsync(q => q.Level_ID == educationalLevel.Level_ID);

                var checkIfCampusIsActive = await _unitOfWork.CampusRepository.FindAsync(a => a.Campus_ID == newEntity.Campus_ID);
                if (!checkIfCampusIsActive.IsActive)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(educationalLevel.Updated_By, (int)Form.Campus_EducationalLevel, "Activate " + _formName, "ACTIVATE EDUCATIONAL LEVEL", false, "Unable to activate " + educationalLevel.Level_Name + " due to inactive Campus.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", UNABLE_ACTIVATE("Campus", checkIfCampusIsActive.Campus_Name), false);
                }

                newEntity.Updated_By = educationalLevel.Updated_By;
                newEntity.Level_Status = "Active";

                var data = await _unitOfWork.EducationLevelRepository.RetrieveAsync(newEntity, newEntity.Level_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_EducationalLevel, "Activate Educational Level", "ACTIVATE EDUCATIONAL LEVEL", false, "Failed: " + newEntity.Level_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_EducationalLevel, "Activate Educational Level", "ACTIVATE EDUCATIONAL LEVEL", true, "Success: " + newEntity.Level_Name, DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", _formName + Constants.SuccessMessageRetrieve, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<educlevelPagedResult> GetAllEduclevel(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.EducationLevelRepository.GetAllEduclevel(pageNo, pageSize, keyword);
        }

        public async Task<educlevelPagedResult> ExportEducationalLevel(string keyword)
        {
            return await _unitOfWork.EducationLevelReportRepository.ExportEducationalLevels(keyword);
        }
        
        public async Task<BatchUploadResponse> BatchUpload(ICollection<educLevelBatchUploadVM> educLevels, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "EducationalLevel_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = educLevels.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var educLevelVM in educLevels)
            {
                i++;

                if (educLevelVM.CampusName == null || educLevelVM.CampusName == string.Empty)
                {
                    importLog.Logging(_educLevelBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (educLevelVM.EducationalLevelName.Trim() == null || educLevelVM.EducationalLevelName.Trim() == string.Empty)
                {
                    importLog.Logging(_educLevelBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!Global.ValidateStr(educLevelVM.EducationalLevelName.Trim()))
                {
                    importLog.Logging(_educLevelBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level Name does not accept special characters except space, dash, apostrophe and underscore.");
                    response.Failed++;
                    continue;
                }
                else if (educLevelVM.EducationalLevelName.Trim().Length > 100)
                {
                    importLog.Logging(_educLevelBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level Name accepts 100 characters only.");
                    response.Failed++;
                    continue;
                }

                //if (educLevelVM.EducationalLevelStatus == null || educLevelVM.EducationalLevelStatus == string.Empty)
                //{
                //    importLog.Logging(_educLevelBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level Status is a required field.");
                //    response.Failed++;
                //    continue;
                //}
                //else if (!(educLevelVM.EducationalLevelStatus.ToUpper().Trim() == "ACTIVE" || educLevelVM.EducationalLevelStatus.ToUpper().Trim() == "INACTIVE"))
                //{
                //    importLog.Logging(_educLevelBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level Status is invalid.");
                //    response.Failed++;
                //    continue;
                //}

                if (educLevelVM.College == null || educLevelVM.College == string.Empty)
                {
                    importLog.Logging(_educLevelBatch, fileName, "Row: " + i.ToString() + " ---> College is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!(educLevelVM.College.ToUpper() == "YES" || educLevelVM.College.ToUpper() == "NO"))
                {
                    importLog.Logging(_educLevelBatch, fileName, "Row: " + i.ToString() + " ---> College is invalid.");
                    response.Failed++;
                    continue;
                }

                var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == educLevelVM.CampusName && x.IsActive == true);

                if (campus == null)
                {
                    importLog.Logging(_educLevelBatch, fileName, "Row: " + i.ToString() + " ---> Campus " + educLevelVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                educationalLevelEntity educLevel = await _unitOfWork.EducationLevelRepository.FindAsync(x => x.Level_Name == educLevelVM.EducationalLevelName && x.Campus_ID == campus.Campus_ID);
                
                if (educLevel != null)
                {
                    educLevel.hasCourse = (educLevelVM.College.ToLower().Equals("yes") ? true : false);
                    educLevel.Campus_ID = campus.Campus_ID;
                    educLevel.Updated_By = user;

                    var isSuccess = await _unitOfWork.EducationLevelRepository.UpdateAsyncWithBase(educLevel, educLevel.Level_ID);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_EducationalLevel, "Update " + _formName +" By Batch", "UPDATE", true, "Success: " + educLevel.Level_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_educLevelBatch, fileName, _formName + " " + educLevel.Level_Name + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_educLevelBatch, fileName, _formName + " " + educLevel.Level_Name + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    educLevel = new educationalLevelEntity();
                    educLevel.Level_Name = educLevelVM.EducationalLevelName;
                    educLevel.Level_Status = "Active";
                    educLevel.hasCourse = (educLevelVM.College.ToLower().Equals("yes") ? true : false);
                    educLevel.Campus_ID = campus.Campus_ID;
                    educLevel.Updated_By = user;
                    educLevel.Added_By = user;

                    var isSuccess = await _unitOfWork.EducationLevelRepository.AddAsyncWithBase(educLevel);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_EducationalLevel, "Insert " + _formName + " By Batch", "INSERT", true, "Success: " + educLevel.Level_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_educLevelBatch, fileName, _formName + " " + educLevel.Level_Name + " successfully added.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_educLevelBatch, fileName, _formName + " " + educLevel.Level_Name + " failed to add.");
                        response.Failed++;
                        continue;
                    }
                }
            }

            return response;
        }
    }
}
