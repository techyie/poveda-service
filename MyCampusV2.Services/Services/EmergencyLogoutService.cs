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

namespace MyCampusV2.Services.Services
{
    public class EmergencyLogoutService : BaseService, IEmergencyLogoutService
    {
        private string _emergencylogoutBatch = AppDomain.CurrentDomain.BaseDirectory + @"EmergencyLogout\";
        private string _formName = "Emergency Logout";
        private ResultModel result = new ResultModel();

        public EmergencyLogoutService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<emergencyLogoutEntity> GetEmergencyLogoutById(int id)
        {
            return await _unitOfWork.EmergencyLogoutRepository.GetEmergencyLogoutById(id);
        }

        public async Task<emergencyLogoutPagedResult> GetAll(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.EmergencyLogoutRepository.GetAll(pageNo, pageSize, keyword);
        }

        public async Task<studentListPagedResult> GetStudentList(int campusId, int educLevelId, int yearSecId, int studSecId)
        {
            return await _unitOfWork.EmergencyLogoutRepository.GetStudentList(campusId, educLevelId, yearSecId, studSecId);
        }

        public async Task<ResultModel> AddEmergencyLogout(emergencyLogoutEntity entity)
        {
            try
            {
                var exist = await _unitOfWork.EmergencyLogoutRepository.FindAsync(q => q.Person_ID == entity.Person_ID && q.Remarks == entity.Remarks && q.EffectivityDate.ToString() == entity.EffectivityDate.ToString());

                if (exist != null)
                    return CreateResult("409", EMERGENCY_LOGOUT_EXIST, false);

                var data = await _unitOfWork.EmergencyLogoutRepository.AddAsyncWithBase(entity);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Added_By, (int)Form.Fetcher_Emergency_Logout, "Add Emergency Logout", "INSERT", false, "Failed: " + entity.Emergency_logout_ID + " - " + entity.Remarks  + " - " + entity.EffectivityDate.ToString(), DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Fetcher Emergency Logout", false);
                }
                else
                {
                    var cardDetails = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == entity.Person_ID && q.IsActive == true);
                    var terminals = await _unitOfWork.TerminalRepository.FindByAsyn(q => q.Terminal_Category == Encryption.Encrypt("Time And Attendance Terminal"));

                    foreach(var terminal in terminals)
                    {
                        datasyncEmergencyEntity datasyncEmergencyEntity = new datasyncEmergencyEntity();
                        datasyncEmergencyEntity.DS_Action = "A";
                        datasyncEmergencyEntity.EffectivityDate = entity.EffectivityDate;
                        datasyncEmergencyEntity.StudentSerial = cardDetails.Card_Serial;
                        datasyncEmergencyEntity.Terminal_ID = terminal.Terminal_ID;
                        await _unitOfWork.DatasyncEmergencyLogoutRepository.AddAsyn(datasyncEmergencyEntity);
                    }
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Added_By, (int)Form.Fetcher_Emergency_Logout, "Add Emergency Logout", "INSERT", true, "Success: " + entity.Emergency_logout_ID + " - " + entity.Remarks + " - " + entity.EffectivityDate.ToString(), DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Emergency Logout" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateEmergencyLogout(emergencyLogoutEntity entity)
        {
            try
            {
                var exist = await _unitOfWork.EmergencyLogoutRepository.FindAsync(q => q.Person_ID == entity.Person_ID && q.Remarks == entity.Remarks && q.EffectivityDate.ToString() == entity.EffectivityDate.ToString());

                if (exist != null)
                    return CreateResult("409", EMERGENCY_LOGOUT_EXIST, false);

                var data = await _unitOfWork.EmergencyLogoutRepository.UpdateAsyncWithBase(entity, entity.Emergency_logout_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Updated_By, (int)Form.Fetcher_Emergency_Logout, "Update Emergency Logout", "UPDATE", false, "Failed: " + entity.Emergency_logout_ID + " - " + entity.Remarks + " - " + entity.EffectivityDate.ToString(), DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Emergency Logout", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Updated_By, (int)Form.Fetcher_Emergency_Logout, "Update Emergency Logout", "UPDATE", true, "Success: Emergency Logout ID: " + entity.Emergency_logout_ID +  " - " + entity.Remarks + " - " + entity.EffectivityDate.ToString(), DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", "Emergency Logout" + Constants.SuccessMessageUpdate, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> CreateByBatch(string[] studentList, string studentRemarks, string studentEffectiveDate, int user)
        {
            try
            {
                return await _unitOfWork.EmergencyLogoutRepository.EmergencyLogout(studentList, studentRemarks, studentEffectiveDate, user);

                //for (int i=0; i<studentList.Length; i++)
                //{ 
                //    var student = await _unitOfWork.PersonRepository.FindAsync(q => q.Person_ID == Convert.ToInt32(studentList[i]));

                //    emergencyLogoutEntity exist = await _unitOfWork.EmergencyLogoutRepository.FindAsync(q => q.Person_ID == Convert.ToInt32(studentList[i]) && q.EffectivityDate.ToString() == studentEffectiveDate);

                //    if (exist != null)
                //    {
                //        exist.Person_ID = Convert.ToInt32(studentList[i]);
                //        exist.Remarks = studentRemarks;
                //        exist.EffectivityDate = DateTime.Parse(studentEffectiveDate.ToString());
                //        exist.IsCancelled = false;
                //        exist.User_ID = user;
                //        exist.Added_By = user;
                //        exist.Updated_By = user;

                //        await _unitOfWork.EmergencyLogoutRepository.UpdateAsyncWithBase(exist, exist.Emergency_logout_ID);
                //        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(exist.Updated_By, (int)Form.Fetcher_Emergency_Logout, "Update Emergency Logout By Batch", "UPDATE", true, "Success: Emergency Logout ID: " + student.ID_Number + " - " + exist.Remarks + " - " + exist.EffectivityDate.ToString(), DateTime.UtcNow.ToLocalTime()));
                //    }
                //    else
                //    {
                //        emergencyLogoutEntity entity = new emergencyLogoutEntity();
                //        entity.Person_ID = Convert.ToInt32(studentList[i]);
                //        entity.Remarks = studentRemarks;
                //        entity.EffectivityDate = DateTime.Parse(studentEffectiveDate.ToString());
                //        entity.IsCancelled = false;
                //        entity.User_ID = user;
                //        entity.Added_By = user;
                //        entity.Updated_By = user;

                //        await _unitOfWork.EmergencyLogoutRepository.AddAsyncWithBase(entity);
                //        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Added_By, (int)Form.Fetcher_Emergency_Logout, "Add Emergency Logout By Batch", "INSERT", true, "Success: " + student.ID_Number + " - " + entity.Remarks + " - " + entity.EffectivityDate.ToString(), DateTime.UtcNow.ToLocalTime()));
                //    } 
                //}
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        } 

        public async Task<ResultModel> DeleteEmergencyLogoutTemporary(int id, int user)
        {
            try
            {
                emergencyLogoutEntity entity = await GetEmergencyLogoutById(id);

                if (entity.IsCancelled == true)
                    entity.IsCancelled = false;
                else
                    entity.IsCancelled = true;

                entity.Updated_By = user;

                var data = await _unitOfWork.EmergencyLogoutRepository.DeleteAsyncTemporary(entity, entity.Emergency_logout_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Updated_By, (int)Form.Fetcher_Emergency_Logout, "Cancel Emergency Logout", "CANCEL", false, "Failed: " + entity.PersonEntity.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Emergency Logout", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Updated_By, (int)Form.Fetcher_Emergency_Logout, "Cancel Emergency Logout", "CANCEL", true, "Success: " + entity.PersonEntity.ID_Number, DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", "Emergency Logout has been cancelled!", true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteEmergencyLogoutPermanent(int id, int user)
        {
            try
            {
                emergencyLogoutEntity entity = await GetEmergencyLogoutById(id);

                entity.Updated_By = user;

                var data = await _unitOfWork.EmergencyLogoutRepository.DeleteAsyncPermanent(entity, entity.Emergency_logout_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Updated_By, (int)Form.Fetcher_Emergency_Logout, "Permanent Delete Emergency Logout", "PERMANENT DELETE", false, "Failed: " + entity.PersonEntity.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Emergency Logout", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Updated_By, (int)Form.Fetcher_Emergency_Logout, "Permanent Delete Emergency Logout", "PERMANENT DELETE", true, "Success: " + entity.PersonEntity.ID_Number, DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", "Emergency Logout" + Constants.SuccessMessagePermanentDelete, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> RetrieveEmergencyLogout(int id, int user)
        {
            try
            {
                var newEntity = await _unitOfWork.EmergencyLogoutRepository.GetAsync(id);
                newEntity.Updated_By = user;

                emergencyLogoutEntity entity = await GetEmergencyLogoutById(id);

                var data = await _unitOfWork.EmergencyLogoutRepository.RetrieveAsync(newEntity, newEntity.Emergency_logout_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Fetcher_Emergency_Logout, "Uncancel Emergency Logout", "UNCANCEL", false, "Failed: " + entity.PersonEntity.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Emergency Logout", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Fetcher_Emergency_Logout, "Uncancel Emergency Logout", "UNCANCEL", true, "Success: " + entity.PersonEntity.ID_Number, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Emergency Logout has been uncancelled!", true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<emergencyLogoutPagedResult> ExportEmergencyLogoutStudentsExcelFile(
            int campusId,
            int educLevelId,
            int yearSecId,
            int studSecId)
        {
            return await _unitOfWork.EmergencyLogoutRepository.ExportEmergencyLogoutStudentsExcelFile(campusId, educLevelId, yearSecId, studSecId);
        }

        public async Task<BatchUploadResponse> BatchUpload(ICollection<emergencyLogoutBatchUploadVM> emergencyLogouts, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "EmergencyLogout_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = emergencyLogouts.Count;
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var emergencyLogoutsVM in emergencyLogouts)
            {
                i++;

                if (emergencyLogoutsVM.Remarks == null || emergencyLogoutsVM.Remarks == string.Empty)
                {
                    importLog.Logging(_emergencylogoutBatch, fileName, "Row: " + i.ToString() + " ---> Remarks is a required field.");
                    response.Failed++;
                    continue;
                }

                if (emergencyLogoutsVM.Date == "" || emergencyLogoutsVM.Date == null)
                {
                    importLog.Logging(_emergencylogoutBatch, fileName, "Row: " + i.ToString() + " ---> Date is a required field.");
                    response.Failed++;
                    continue;
                }

                DateTime temp;
                if (!DateTime.TryParse(emergencyLogoutsVM.Date, out temp))
                {
                    importLog.Logging(_emergencylogoutBatch, fileName, "Row: " + i.ToString() + " ---> Date is invalid.");
                    response.Failed++;
                    continue;
                }

                var person = await _unitOfWork.PersonRepository.FindAsync(x => (x.ID_Number + " - " + x.Last_Name + ", " + x.First_Name) == emergencyLogoutsVM.Student && x.ToDisplay == true);

                if (person == null)
                {
                    importLog.Logging(_emergencylogoutBatch, fileName, "Row: " + i.ToString() + " ---> Student " + (person.Last_Name + ", " + person.First_Name) + " does not exist.");
                    response.Failed++;
                    continue;
                }

                emergencyLogoutEntity emergencyLogout = await _unitOfWork.EmergencyLogoutRepository.FindAsync(x => x.Person_ID == person.Person_ID && x.Remarks == emergencyLogoutsVM.Remarks && x.EffectivityDate.ToString("YYYY-mm-dd") == emergencyLogoutsVM.Date);

                if (emergencyLogout != null)
                {
                    emergencyLogout.Updated_By = user;

                    var isSuccess = await _unitOfWork.EmergencyLogoutRepository.UpdateAsyncWithBase(emergencyLogout, emergencyLogout.Emergency_logout_ID);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Fetcher_Emergency_Logout, "Update " + _formName + " By Batch", "UPDATE", true, "Success: " + (person.Last_Name + ", " + person.First_Name), DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_emergencylogoutBatch, fileName, _formName + " " + (person.Last_Name + ", " + person.First_Name) + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_emergencylogoutBatch, fileName, _formName + " " + (person.Last_Name + ", " + person.First_Name) + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    emergencyLogout = new emergencyLogoutEntity();
                    emergencyLogout.Person_ID = person.Person_ID;
                    emergencyLogout.IsCancelled = true;
                    emergencyLogout.User_ID = user;
                    emergencyLogout.Remarks = emergencyLogoutsVM.Remarks;
                    emergencyLogout.EffectivityDate = (emergencyLogoutsVM.Date == null ? DateTime.Now : Convert.ToDateTime(emergencyLogoutsVM.Date));
                    emergencyLogout.Updated_By = user;
                    emergencyLogout.Added_By = user;

                    var isSuccess = await _unitOfWork.EmergencyLogoutRepository.AddAsyncWithBase(emergencyLogout);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Fetcher_Emergency_Logout, "Insert " + _formName + " By Batch", "INSERT", true, "Success: " + (person.Last_Name + ", " + person.First_Name), DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_emergencylogoutBatch, fileName, _formName + " " + (person.Last_Name + ", " + person.First_Name) + " successfully added.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_emergencylogoutBatch, fileName, _formName + " " + (person.Last_Name + ", " + person.First_Name) + " failed to add.");
                        response.Failed++;
                        continue;
                    }
                }
            }

            return response;
        }
    }
}
