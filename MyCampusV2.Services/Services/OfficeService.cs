using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Common.ViewModels.OfficeViewModel;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.Services
{
    public class OfficeService : BaseService, IOfficeService
    {
        private string _officeBatch = AppDomain.CurrentDomain.BaseDirectory + @"Office\";
        private string _formName = "Office";

        public OfficeService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<IList<officeEntity>> GetOfficesUsingCampusId(int id)
        {
            return await _unitOfWork.OfficeRepository.GetOfficesUsingCampusId(id);
        }

        public async Task<officeEntity> GetOfficeById(int id)
        {
            return await _unitOfWork.OfficeRepository.GetOfficeById(id);
        }

        public async Task<ResultModel> AddOffice(officeEntity office)
        {
            try
            {
                office.Office_Status = "Active";
                office.IsActive = true;

                var exist = await _unitOfWork.OfficeRepository.FindAsync(q => q.Office_Name == office.Office_Name && q.IsActive == true && q.Campus_ID == office.Campus_ID);

                if (exist != null)
                    return CreateResult("409", OFFICE_EXIST, false);

                var data = await _unitOfWork.OfficeRepository.AddAsyncWithBase(office);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(office.Added_By, (int)Form.Campus_Office, "Add " + _formName, "INSERT", false, "Failed: " + office.Office_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(office.Added_By, (int)Form.Campus_Office, "Add " + _formName, "INSERT", true, "Success: " + office.Office_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateOffice(officeEntity office)
        {
            try
            {
                var updateCheck = await _unitOfWork.OfficeRepository.FindAsync(q => q.Office_ID == office.Office_ID && q.Campus_ID == office.Campus_ID);

                if (updateCheck == null)
                {
                    var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.Office_ID == office.Office_ID);

                    if (recordCountPerson != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(office.Added_By, (int)Form.Campus_Office, "Update " + _formName, "UPDATE", false, "Failed due to active record: " + office.Office_Name, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }
                }

                office.IsActive = office.Office_Status == "Active" ? true : false;

                var data = await _unitOfWork.OfficeRepository.UpdateAsyncWithBase(office, office.Office_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(office.Added_By, (int)Form.Campus_Office, "Update " + _formName, "UPDATE", false, "Failed: " + office.Office_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(office.Updated_By, (int)Form.Campus_Office, "Update " + _formName, "UPDATE", true, "Success: Office ID: " + office.Office_ID + " Office Name: " + office.Office_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageUpdate, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteOfficePermanent(int id, int user)
        {
            try
            {
                officeEntity office = await GetOfficeById(id);
                
                office.Updated_By = user;

                var data = await _unitOfWork.OfficeRepository.DeleteAsyncPermanent(office, office.Office_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(office.Updated_By, (int)Form.Campus_Office, "Delete Permanently " + _formName, "PERMANENT DELETE", false, "Failed: " + office.Office_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(office.Updated_By, (int)Form.Campus_Office, "Delete Permanently " + _formName, "PERMANENT DELETE", true, "Success: " + office.Office_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessagePermanentDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteOfficeTemporary(int id, int user)
        {
            try
            {
                officeEntity office = await GetOfficeById(id);

                var checkIfOtherAccessExists = await _unitOfWork.PersonRepository.FindAsync(a => a.Office_ID == id && a.Person_Type == "O" && a.IsActive == true);
                if (checkIfOtherAccessExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(office.Updated_By, (int)Form.Campus_Employee_Sub_Type, "Deactivate " + _formName, "DEACTIVATE", false, "Unable to deactivate " + office.Office_Name + " due to existing active other access.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                office.Office_Status = "Inactive";
                office.IsActive = true;
                office.Updated_By = user;

                var data = await _unitOfWork.OfficeRepository.DeleteAsyncTemporary(office, office.Office_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(office.Updated_By, (int)Form.Campus_Office, "Deactivate " + _formName, "DEACTIVATE", false, "Failed: " + office.Office_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(office.Updated_By, (int)Form.Campus_Office, "Deactivate " + _formName, "DEACTIVATE", true, "Success: " + office.Office_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageTemporaryDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> RetrieveOffice(officeEntity office)
        {
            try
            {
                var newEntity = await _unitOfWork.OfficeRepository.GetAsync(office.Office_ID);

                var checkIfCampusIsActive = await _unitOfWork.CampusRepository.FindAsync(a => a.Campus_ID == newEntity.Campus_ID);
                if (!checkIfCampusIsActive.IsActive)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(office.Updated_By, (int)Form.Campus_Office, "Activate Office", "ACTIVATE OFFICE", false, "Unable to activate " + newEntity.Office_Name + " due to inactive Campus.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", UNABLE_ACTIVATE("Campus", checkIfCampusIsActive.Campus_Name), false);
                }

                newEntity.Office_Status = "Active";
                newEntity.Updated_By = office.Updated_By;

                var data = await _unitOfWork.OfficeRepository.RetrieveAsync(newEntity, newEntity.Office_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Office, "Activate Office", "ACTIVATE OFFICE", false, "Failed: " + newEntity.Office_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Office", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Office, "Activate Office", "ACTIVATE OFFICE", true, "Success: " + newEntity.Office_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Office" + Constants.SuccessMessageRetrieve, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<officePagedResult> GetAllOffice(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.OfficeRepository.GetAllOffice(pageNo, pageSize, keyword);
        }

        public async Task<officePagedResult> ExportOffice(string keyword)
        {
            return await _unitOfWork.OfficeReportRepository.ExportOffice(keyword);
        }

        public async Task<BatchUploadResponse> BatchUpload(ICollection<officeBatchUploadVM> offices, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();
            bool isSuccess;

            string fileName = "Office_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = offices.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var officeVM in offices)
            {
                i++;

                if (officeVM.CampusName == null || officeVM.CampusName == string.Empty)
                {
                    importLog.Logging(_officeBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (officeVM.OfficeName.Trim() == null || officeVM.OfficeName.Trim() == string.Empty)
                {
                    importLog.Logging(_officeBatch, fileName, "Row: " + i.ToString() + " ---> Office Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!Global.ValidateStr(officeVM.OfficeName.Trim()))
                {
                    importLog.Logging(_officeBatch, fileName, "Row: " + i.ToString() + " ---> Office Name does not accept special characters except space, dash, apostrophe and underscore.");
                    response.Failed++;
                    continue;
                }
                else if (officeVM.OfficeName.Trim().Length > 100)
                {
                    importLog.Logging(_officeBatch, fileName, "Row: " + i.ToString() + " ---> Office Name accepts 100 characters only.");
                    response.Failed++;
                    continue;
                }

                //if (officeVM.OfficeStatus == null || officeVM.OfficeStatus == string.Empty)
                //{
                //    importLog.Logging(_officeBatch, fileName, "Row: " + i.ToString() + " ---> Office Status is a required field.");
                //    response.Failed++;
                //    continue;
                //}
                //else if (!(officeVM.OfficeStatus.ToUpper().Trim() == "ACTIVE" || officeVM.OfficeStatus.ToUpper().Trim() == "INACTIVE"))
                //{
                //    importLog.Logging(_officeBatch, fileName, "Row: " + i.ToString() + " ---> Office Status is invalid.");
                //    response.Failed++;
                //    continue;
                //}

                var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == officeVM.CampusName && x.IsActive == true);

                if (campus == null)
                {
                    importLog.Logging(_officeBatch, fileName, "Row: " + i.ToString() + " ---> Campus " + officeVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                officeEntity office = await _unitOfWork.OfficeRepository.FindAsync(x => x.Office_Name == officeVM.OfficeName && x.Campus_ID == campus.Campus_ID);

                if (office != null)
                {
                    var updateCheck = await _unitOfWork.OfficeRepository.FindAsync(q => q.Office_ID == office.Office_ID && q.Campus_ID == office.Campus_ID);

                    if (updateCheck == null)
                    {
                        var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.Office_ID == office.Office_ID);

                        if (recordCountPerson != null)
                        {
                            importLog.Logging(_officeBatch, fileName, "Row: " + i.ToString() + " ---> Office " + officeVM.OfficeName + ": " + UNABLE_EDIT);
                            response.Failed++;
                            continue;
                        }
                    }

                    office.Office_Status = "Active";
                    office.Campus_ID = campus.Campus_ID;

                    isSuccess = await _unitOfWork.OfficeRepository.UpdateWithBoolReturn(office, user);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Office, "Update " + _formName + " By Batch", "UPDATE", true, "Success: " + officeVM.OfficeName, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess)
                    {
                        importLog.Logging(_officeBatch, fileName, _formName + " " + officeVM.OfficeName + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_officeBatch, fileName, _formName + " " + officeVM.OfficeName + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    office = new officeEntity();
                    office.Office_Name = officeVM.OfficeName;
                    office.Office_Status = "Active";
                    office.Campus_ID = campus.Campus_ID;

                    isSuccess = await _unitOfWork.OfficeRepository.AddWithBoolReturn(office, user);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Office, "Insert " + _formName + " By Batch", "INSERT", true, "Success: " + officeVM.OfficeName, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess)
                    {
                        importLog.Logging(_officeBatch, fileName, _formName + " " + officeVM.OfficeName + " successfully added.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_officeBatch, fileName, _formName + " " + officeVM.OfficeName + " failed to add.");
                        response.Failed++;
                        continue;
                    }
                }
            }

            return response;
        }

    }
}
