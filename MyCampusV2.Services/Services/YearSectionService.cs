using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MyCampusV2.Services.Services
{
    public class YearSectionService : BaseService, IYearSectionService
    {
        private string _yearSectionBatch = AppDomain.CurrentDomain.BaseDirectory + @"YearSection\";
        private ResultModel result = new ResultModel();

        public YearSectionService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<ICollection<yearSectionEntity>> GetYearSections()
        {
            try
            {
                return await _unitOfWork.YearSectionRepository.GetYearSections().ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<IList<yearSectionEntity>> GetYearSectionsUsingEducationalLevelId(int id)
        {
            return await _unitOfWork.YearSectionRepository.GetYearSectionsUsingEducationalLevelId(id);
        }

        public async Task<yearSectionEntity> GetYearSectionById(int id)
        {
            return await _unitOfWork.YearSectionRepository.GetYearSectionById(id);
        }

        public async Task<ResultModel> AddYearSection(yearSectionEntity yearSection)
        {
            try
            {
                var exist = await _unitOfWork.YearSectionRepository.FindAsync(q => q.YearSec_Name == yearSection.YearSec_Name && q.IsActive == true && q.Level_ID == yearSection.Level_ID);

                if (exist != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(yearSection.Added_By, (int)Form.Campus_Year, "Add Year Level", "INSERT", false, "Failed: " + yearSection.YearSec_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", YEAR_SECTION_EXIST, false);
                }

                var data = await _unitOfWork.YearSectionRepository.AddAsyncWithBase(yearSection);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(yearSection.Added_By, (int)Form.Campus_Year, "Add Year Level", "INSERT", false, "Failed: " + yearSection.YearSec_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Year Level", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(yearSection.Added_By, (int)Form.Campus_Year, "Add Year Level", "INSERT", true, "Success: " + yearSection.YearSec_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Year Level" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateYearSection(yearSectionEntity yearSection)
        {
            try
            {
                var exist = await _unitOfWork.YearSectionRepository.FindAsync(q => q.YearSec_Name == yearSection.YearSec_Name && q.IsActive == true && q.Level_ID == yearSection.Level_ID && q.YearSec_ID != yearSection.YearSec_ID);

                if (exist != null)
                    return CreateResult("409", YEAR_SECTION_EXIST, false);

                var updateCheck = await _unitOfWork.YearSectionRepository.FindAsync(q => q.YearSec_ID == yearSection.YearSec_ID && q.Level_ID == yearSection.Level_ID);

                if (updateCheck == null)
                {
                    var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.Year_Section_ID == yearSection.YearSec_ID);

                    if (recordCountPerson != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(yearSection.Updated_By, (int)Form.Campus_Year, "Update Year Level", "UPDATE", false, "Failed due to active record: " + yearSection.YearSec_Name, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }

                    var recordCountSection = await _unitOfWork.StudentSectionRepository.FindAsync(q => q.YearSec_ID == yearSection.YearSec_ID && q.IsActive == true);

                    if (recordCountSection != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(yearSection.Updated_By, (int)Form.Campus_Year, "Update Year Level", "UPDATE", false, "Failed due to active record: " + yearSection.YearSec_Name, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }
                }

                var data = await _unitOfWork.YearSectionRepository.UpdateAsyncWithBase(yearSection, yearSection.YearSec_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(yearSection.Updated_By, (int)Form.Campus_Year, "Update Year Level", "UPDATE", false, "Failed: " + yearSection.YearSec_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Year Level", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(yearSection.Updated_By, (int)Form.Campus_Year, "Update Year Level", "UPDATE", true, "Success: Year Level ID: " + yearSection.YearSec_ID + " Year Level Name: " + yearSection.YearSec_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Year Level" + Constants.SuccessMessageUpdate, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteYearSectionPermanent(int id, int user)
        {
            try
            {
                yearSectionEntity yearSection = await GetYearSectionById(id);

                var checkIfStudentSectionExists = await _unitOfWork.StudentSectionRepository.FindAsync(a => a.YearSec_ID == id && a.ToDisplay == true);
                if (checkIfStudentSectionExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Year, "Permanent Delete Year Level", "PERMANENT DELETE", false, "Unable to permanent delete " + yearSection.YearSec_Name + " due to existing active section", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Unable to permanent delete.", false);
                }

                yearSection.Updated_By = user;

                var data = await _unitOfWork.YearSectionRepository.DeleteAsyncPermanent(yearSection, yearSection.YearSec_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(yearSection.Updated_By, (int)Form.Campus_Year, "Delete Permanently Year Level", "PERMANENT DELETE", false, "Failed: " + yearSection.YearSec_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Year Level", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(yearSection.Updated_By, (int)Form.Campus_Year, "Delete Permanently Year Level", "PERMANENT DELETE", true, "Success: " + yearSection.YearSec_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Year Level" + Constants.SuccessMessagePermanentDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteYearSectionTemporary(int id, int user)
        {
            try
            {
                yearSectionEntity yearSection = await GetYearSectionById(id);

                var checkIfStudentSectionExists = await _unitOfWork.StudentSectionRepository.FindAsync(a => a.YearSec_ID == id && a.IsActive == true);
                if (checkIfStudentSectionExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Year, "Deactivate Year Level", "DEACTIVATE", false, "Unable to deactivate " + yearSection.YearSec_Name + " due to existing active section/s.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                yearSection.Updated_By = user;

                var data = await _unitOfWork.YearSectionRepository.DeleteAsyncTemporary(yearSection, yearSection.YearSec_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(yearSection.Updated_By, (int)Form.Campus_Year, "Deactivate Year Level", "DEACTIVATE", false, "Failed: " + yearSection.YearSec_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Year Level", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(yearSection.Updated_By, (int)Form.Campus_Year, "Deactivate Year Level", "DEACTIVATE", true, "Success: " + yearSection.YearSec_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Year Level" + Constants.SuccessMessageTemporaryDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> RetrieveYearSection(yearSectionEntity yearSection)
        {
            try
            {
                var newEntity = await _unitOfWork.YearSectionRepository.GetAsync(yearSection.YearSec_ID);

                var checkIfEducLevelIsActive = await _unitOfWork.EducationLevelRepository.FindAsync(a => a.Level_ID == newEntity.Level_ID);
                if (!checkIfEducLevelIsActive.IsActive)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(yearSection.Updated_By, (int)Form.Campus_Year, "Activate Year Level", "ACTIVATE YEAR LEVEL", false, "Unable to activate " + newEntity.YearSec_Name + " due to inactive Educational Level.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", UNABLE_ACTIVATE("Educational Level", checkIfEducLevelIsActive.Level_Name), false);
                }

                newEntity.Updated_By = yearSection.Updated_By;

                var data = await _unitOfWork.YearSectionRepository.RetrieveAsync(newEntity, newEntity.YearSec_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Year, "Activate Year Level", "ACTIVATE YEAR LEVEL", false, "Failed: " + newEntity.YearSec_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Year Level", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Year, "Activate Year Level", "ACTIVATE YEAR LEVEL", true, "Success: " + newEntity.YearSec_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Year Level" + Constants.SuccessMessageRetrieve, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<yearSectionPagedResult> GetAllYearSection(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.YearSectionRepository.GetAllYearSection(pageNo, pageSize, keyword);
        }

        public async Task<yearSectionPagedResult> ExportAllYearSections(string keyword)
        {
            return await _unitOfWork.YearSectionRepository.ExportAllYearSections(keyword);
        }

        public async Task<BatchUploadResponse> BatchUpload(ICollection<yearSectionBatchUploadVM> yearsections, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "YearLevel_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = yearsections.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var yearsectionVM in yearsections)
            {
                i++;

                if (yearsectionVM.CampusName == null || yearsectionVM.CampusName == string.Empty)
                {
                    importLog.Logging(_yearSectionBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (yearsectionVM.EducationalLevelName == null || yearsectionVM.EducationalLevelName == string.Empty)
                {
                    importLog.Logging(_yearSectionBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (yearsectionVM.YearSecName == null || yearsectionVM.YearSecName == string.Empty)
                {
                    importLog.Logging(_yearSectionBatch, fileName, "Row: " + i.ToString() + " ---> Year Level Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!Global.ValidateStr(yearsectionVM.YearSecName.Trim()))
                {
                    importLog.Logging(_yearSectionBatch, fileName, "Row: " + i.ToString() + " ---> Year Level Name does not accept special characters except space, dash, apostrophe and underscore.");
                    response.Failed++;
                    continue;
                }
                else if (yearsectionVM.YearSecName.Trim().Length > 125)
                {
                    importLog.Logging(_yearSectionBatch, fileName, "Row: " + i.ToString() + " ---> Year Level Name accepts 125 characters only.");
                    response.Failed++;
                    continue;
                }

                var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == yearsectionVM.CampusName && x.IsActive == true);

                if (campus == null)
                {
                    importLog.Logging(_yearSectionBatch, fileName, "Row: " + i.ToString() + " ---> Campus " + yearsectionVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                var educlevel = await _unitOfWork.EducationLevelRepository.FindAsync(x => x.Level_Name == yearsectionVM.EducationalLevelName && x.Campus_ID == campus.Campus_ID && x.IsActive == true);

                if (educlevel == null)
                {
                    importLog.Logging(_yearSectionBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level " + yearsectionVM.EducationalLevelName + " under Campus " + yearsectionVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                yearSectionEntity yearsection = await _unitOfWork.YearSectionRepository.FindAsync(x => x.YearSec_Name == yearsectionVM.YearSecName && x.IsActive == true && x.Level_ID == educlevel.Level_ID);

                if (yearsection != null)
                {
                    yearsection.YearSec_Name = yearsectionVM.YearSecName;
                    yearsection.Level_ID = educlevel.Level_ID; ;
                    yearsection.Updated_By = user;

                    //Boolean isSuccess = await _unitOfWork.YearSectionRepository.UpdateYearSectionWithBoolReturn(yearsec, user);
                    var isSuccess = await _unitOfWork.YearSectionRepository.UpdateAsyncWithBase(yearsection, yearsection.YearSec_ID);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Year, "Update Year Level By Batch", "UPDATE", true, "Success: " + yearsection.YearSec_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_yearSectionBatch, fileName, "Year Level " + yearsection.YearSec_Name + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_yearSectionBatch, fileName, "Year Level " + yearsection.YearSec_Name + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    yearsection = new yearSectionEntity();
                    yearsection.YearSec_Name = yearsectionVM.YearSecName;
                    yearsection.Level_ID = educlevel.Level_ID;
                    yearsection.IsActive = true;
                    yearsection.ToDisplay = true;
                    yearsection.Added_By = user;
                    yearsection.Updated_By = user;

                    var isSuccess = await _unitOfWork.YearSectionRepository.AddAsyncWithBase(yearsection);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Year, "Insert Year Level By Batch", "INSERT", true, "Success: " + yearsection.YearSec_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_yearSectionBatch, fileName, "Year Level " + yearsection.YearSec_Name + " successfully added.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_yearSectionBatch, fileName, "Year Level " + yearsection.YearSec_Name + " failed to add.");
                        response.Failed++;
                        continue;
                    }
                }
            }

            return response;
        }
    }
}
