using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Services.Helpers;
using System.Text.RegularExpressions;
using MyCampusV2.Models.V2.entity;

namespace MyCampusV2.Services
{
    public class CampusService : BaseService, ICampusService
    {
        private string _campusBatch = AppDomain.CurrentDomain.BaseDirectory + @"Campus\";
        private string _formName = "Campus";

        public CampusService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<ResultModel> AddCampus(campusEntity campus, int user)
        {
            try
            {
                var exist = await _unitOfWork.CampusRepository.FindAsync(q => (q.Campus_Name == campus.Campus_Name) || (q.Campus_Code == campus.Campus_Code) && q.IsActive == true);

                if (exist != null)
                    return CreateResult("409", CAMPUS_EXIST, false);

                campus.IsActive = campus.Campus_Status == "Active" ? true : false;
                campus.ToDisplay = true;
                campus.Added_By = user;
                campus.Updated_By = user;
                campus.Date_Time_Added = DateTime.UtcNow;
                campus.Last_Updated = DateTime.UtcNow;

                var data = await _unitOfWork.CampusRepository.AddAsyn(campus);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(campus.Added_By, (int)Form.Campus_Campus, "Add " + _formName, "INSERT", false, "Failed: " + campus.Campus_Code, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Campus", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(campus.Added_By, (int)Form.Campus_Campus, "Add " + _formName, "INSERT", true, "Success: " + campus.Campus_Code, DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", _formName + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateCampus(campusEntity campus, int user)
        {
            try
            {
                var exist = await _unitOfWork.CampusRepository.FindAsync(q => q.Campus_Code == campus.Campus_Code && q.IsActive == true && q.Campus_ID != campus.Campus_ID);

                if (exist != null)
                    return CreateResult("409", CAMPUS_EXIST, false);

                campus.IsActive = campus.Campus_Status == "Active" ? true : false;
                campus.Updated_By = user;

                campusEntity data = null;

                if (campus.IsActive == false)
                {
                    var checkIfCampuslExists = await _unitOfWork.EducationLevelRepository.FindAsync(a => a.Campus_ID == campus.Campus_ID && a.IsActive == true);
                    if (checkIfCampuslExists != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Campus, "Update " + _formName, "UPDATE", false, "Unable to update " + campus.Campus_Name + " due to existing active educational level/s.", DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", "Unable to update Campus Status to inactive. Item is in use.", false);
                    }

                    var checkIfCampus2Exists = await _unitOfWork.DepartmentRepository.FindAsync(a => a.Campus_ID == campus.Campus_ID && a.IsActive == true);
                    if (checkIfCampus2Exists != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Campus, "Update " + _formName, "UPDATE", false, "Unable to update " + campus.Campus_Name + " due to existing active department/s.", DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", "Unable to update Campus Status to inactive. Item is in use.", false);
                    }

                    var checkIfCampus3Exists = await _unitOfWork.AreaRepository.FindAsync(a => a.Campus_ID == campus.Campus_ID && a.IsActive == true);
                    if (checkIfCampus3Exists != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Campus, "Update " + _formName, "UPDATE", false, "Unable to update " + campus.Campus_Name + " due to existing active area/s.", DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", "Unable to update Campus Status to inactive. Item is in use.", false);
                    }

                    campus.Campus_Status = "Inactive";
                    data = await _unitOfWork.CampusRepository.DeleteAsyncTemporary(campus, campus.Campus_ID);
                }
                else
                {
                    campus.Campus_Status = "Active";
                    campus.IsActive = true;
                    data = await _unitOfWork.CampusRepository.UpdateAsyncWithBase(campus, campus.Campus_ID);
                }
                    
                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(campus.Updated_By, (int)Form.Campus_Campus, "Update " + _formName, "UPDATE", false, "Failed: " + campus.Campus_Code, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Campus", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(campus.Updated_By, (int)Form.Campus_Campus, "Update " + _formName, "UPDATE", true, "Success: Campus Code: " + campus.Campus_Code + " Campus Name: " + campus.Campus_Name, DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", _formName + Constants.SuccessMessageUpdate, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteCampusPermanent(int id, int user)
        {
            try
            {
                campusEntity campus = await GetCampusByID(id);

                var checkIfCampuslExists = await _unitOfWork.EducationLevelRepository.FindAsync(a => a.Campus_ID == id && a.ToDisplay == true);
                if (checkIfCampuslExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Campus, "Permanent Delete " + _formName, "PERMANENT DELETE", false, "Unable to permanent delete " + campus.Campus_Name + " due to existing active year level", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Unable to permanent delete.", false);
                }

                campus.Updated_By = user;

                var data = await _unitOfWork.CampusRepository.DeleteAsyncPermanent(campus, campus.Campus_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(campus.Updated_By, (int)Form.Campus_Campus, "Delete Permanently " + _formName, "PERMANENT DELETE", false, "Failed: " + campus.Campus_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(campus.Updated_By, (int)Form.Campus_Campus, "Delete Permanently " + _formName, "PERMANENT DELETE", true, "Success: " + campus.Campus_Name, DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", _formName + Constants.SuccessMessagePermanentDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteCampusTemporary(int id, int user)
        {
            try
            {
                campusEntity campus = await GetCampusByID(id);

                var checkIfCampuslExists = await _unitOfWork.EducationLevelRepository.FindAsync(a => a.Campus_ID == id && a.IsActive == true);
                if (checkIfCampuslExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Campus, "Deactivate " + _formName, "DEACTIVATE", false, "Unable to deactivate " + campus.Campus_Name + " due to existing active educational level/s.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                campus.Updated_By = user;
                campus.Campus_Status = campus.IsActive ? "Inactive" : "Active";

                var data = await _unitOfWork.CampusRepository.DeleteAsyncTemporary(campus, campus.Campus_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(campus.Updated_By, (int)Form.Campus_Campus, "Deactivate " + _formName, "DEACTIVATE", false, "Failed: " + campus.Campus_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(campus.Updated_By, (int)Form.Campus_Campus, "Deactivate " + _formName, "DEACTIVATE", true, "Success: " + campus.Campus_Name, DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", _formName + Constants.SuccessMessageUpdate, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<campusPagedResult> GetAllCampuses(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.CampusRepository.GetAllCampuses(pageNo, pageSize, keyword);
        }

        public async Task<ICollection<regionEntity>> GetRegion()
        {
            return await _unitOfWork.CampusRepository.GetRegion();
        }

        public async Task<ICollection<divisionEntity>> GetDivisionByRegion(int id)
        {
            return await _unitOfWork.CampusRepository.GetDivisionByRegion(id);
        }

        public async Task<campusEntity> GetCampusByID(int id)
        {
            return await _unitOfWork.CampusRepository.GetCampusByID(id);
        }

        public async Task<ICollection<campusEntity>> GetCampuses()
        {
            try
            {
                return await _unitOfWork.CampusRepository.GetCampuses().ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<campusPagedResult> ExportCampus(string keyword)
        {
            return await _unitOfWork.CampusReportRepository.ExportCampus(keyword);
        }

        public async Task<BatchUploadResponse> BatchUpload(ICollection<campusBatchUploadVM> campuses, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();
            bool isSuccess;

            string fileName = "Campus_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = campuses.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var campusVM in campuses)
            {
                i++;

                if (campusVM.CampusCode.Trim() == null || campusVM.CampusCode.Trim() == string.Empty)
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Campus Code is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!Global.ValidateStr(campusVM.CampusCode.Trim()))
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Campus Code does not accept special characters except space, dash, apostrophe and underscore.");
                    response.Failed++;
                    continue;
                }
                else if (campusVM.CampusCode.Trim().Length > 10)
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Campus Code accepts 10 characters only.");
                    response.Failed++;
                    continue;
                }

                if (campusVM.CampusName.Trim() == null || campusVM.CampusName.Trim() == string.Empty)
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!Global.ValidateStr(campusVM.CampusName.Trim()))
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name does not accept special characters except space, dash, apostrophe and underscore.");
                    response.Failed++;
                    continue;
                }
                else if (campusVM.CampusName.Trim().Length > 100)
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name accepts 100 characters only.");
                    response.Failed++;
                    continue;
                }

                if (campusVM.CampusStatus == null || campusVM.CampusStatus == string.Empty)
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Campus Status is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!(campusVM.CampusStatus.ToUpper().Trim() == "ACTIVE" || campusVM.CampusStatus.ToUpper().Trim() == "INACTIVE"))
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Campus Status is invalid.");
                    response.Failed++;
                    continue;
                }


                if (campusVM.CampusContactNo == null || campusVM.CampusContactNo == string.Empty)
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Campus Contact Number is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (campusVM.CampusContactNo.Trim().Length > 20)
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Campus Contact Number accepts 20 characters only.");
                    response.Failed++;
                    continue;
                }

                if (campusVM.CampusAddress == null || campusVM.CampusAddress == string.Empty)
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Campus Address is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (campusVM.CampusAddress.Trim().Length > 300)
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Campus Address accepts 300 characters only.");
                    response.Failed++;
                    continue;
                }

                if (campusVM.Region == null || campusVM.Region == string.Empty)
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Region is a required field.");
                    response.Failed++;
                    continue;
                }

                if (campusVM.Division == null || campusVM.Division == string.Empty)
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Division is a required field.");
                    response.Failed++;
                    continue;
                }

                var region = await _unitOfWork.RegionRepository.FindAsync(x => x.Name == campusVM.Region);

                if (region == null)
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Region " + campusVM.Region + " does not exist.");
                    response.Failed++;
                    continue;
                }

                var division = await _unitOfWork.DivisionRepository.FindAsync(x => x.Name == campusVM.Division && x.RegionEntity.ID == region.ID);

                if (division == null)
                {
                    importLog.Logging(_campusBatch, fileName, "Row: " + i.ToString() + " ---> Division " + campusVM.Division + " does not exist.");
                    response.Failed++;
                    continue;
                }

                campusEntity campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == campusVM.CampusName && x.IsActive == true);

                if (campus != null)
                {
                    campus.Campus_Status = campusVM.CampusStatus;
                    campus.Campus_Address = campusVM.CampusAddress;
                    campus.Campus_ContactNo = campusVM.CampusContactNo;
                    campus.Division_ID = division.ID;

                    isSuccess = await _unitOfWork.CampusRepository.UpdateWithBoolReturn(campus, user);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Campus, "Update " + _formName + " By Batch", "UPDATE", true, "Success: " + campusVM.CampusName, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess)
                    {
                        importLog.Logging(_campusBatch, fileName, _formName + " " + campusVM.CampusName + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_campusBatch, fileName, _formName + " " + campusVM.CampusName + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    campus = new campusEntity();
                    campus.Campus_Code = campusVM.CampusCode;
                    campus.Campus_Name = campusVM.CampusName;
                    campus.Campus_Status = campusVM.CampusStatus;
                    campus.Campus_Address = campusVM.CampusAddress;
                    campus.Campus_ContactNo = campusVM.CampusContactNo;
                    campus.Campus_ID = campus.Campus_ID;
                    campus.Division_ID = division.ID;

                    isSuccess = await _unitOfWork.CampusRepository.AddWithBoolReturn(campus, user);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Campus, "Insert " + _formName + " By Batch", "INSERT", true, "Success: " + campusVM.CampusName, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess)
                    {
                        importLog.Logging(_campusBatch, fileName, _formName + " " + campusVM.CampusName + " successfully added.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_campusBatch, fileName, _formName + " " + campusVM.CampusName + " failed to add.");
                        response.Failed++;
                        continue;
                    }
                }
            }
            return response;
        }

    }
}
