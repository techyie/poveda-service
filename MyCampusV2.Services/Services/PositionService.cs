using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services
{
    public class PositionService : BaseService, IPositionService
    {
        private string _posBatch = AppDomain.CurrentDomain.BaseDirectory + @"Position\";
        private ResultModel result = new ResultModel();

        public PositionService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<IList<positionEntity>> GetPositionsUsingDepartmentId(int id)
        {
            return await _unitOfWork.PositionRepository.GetPositionsUsingDepartmentId(id);
        }

        public async Task<positionEntity> GetPositionById(int id)
        {
            return await _unitOfWork.PositionRepository.GetPositionById(id);
        }

        public async Task<ResultModel> AddPosition(positionEntity position)
        {
            try
            {
                position.Position_Status = "Active";

                var exist = await _unitOfWork.PositionRepository.FindAsync(q => q.Position_Name == position.Position_Name && q.IsActive == true && q.Department_ID == position.Department_ID);

                if (exist != null)
                    return CreateResult("409", POSITION_EXIST, false);

                var data = await _unitOfWork.PositionRepository.AddAsyncWithBase(position);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(position.Added_By, (int)Form.Campus_Position, "Add Position", "INSERT", false, "Failed: " + position.Position_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Position", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(position.Added_By, (int)Form.Campus_Position, "Add Position", "INSERT", true, "Success: " + position.Position_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Position" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdatePosition(positionEntity position)
        {
            try
            {
                var exist = await _unitOfWork.PositionRepository.FindAsync(q => q.Position_Name == position.Position_Name && q.IsActive == true && q.Department_ID == position.Department_ID && q.Position_ID != position.Position_ID);

                if (exist != null)
                    return CreateResult("409", POSITION_EXIST, false);

                var updateCheck = await _unitOfWork.PositionRepository.FindAsync(q => q.Position_ID == position.Position_ID && q.Department_ID == position.Department_ID);

                if (updateCheck == null)
                {
                    var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.Position_ID == position.Position_ID);

                    if (recordCountPerson != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(position.Added_By, (int)Form.Campus_Position, "Update Position", "UPDATE", false, "Failed due to active record: " + position.Position_Name, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }
                }

                position.Position_Status = "Active";

                var data = await _unitOfWork.PositionRepository.UpdateAsyncWithBase(position, position.Position_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(position.Added_By, (int)Form.Campus_Position, "Update Position", "UPDATE", false, "Failed: " + position.Position_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Position", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(position.Updated_By, (int)Form.Campus_Position, "Update Position", "UPDATE", true, "Success: Position ID: " + position.Position_ID + " Position Name: " + position.Position_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Position" + Constants.SuccessMessageUpdate, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeletePositionPermanent(int id, int user)
        {
            try
            {
                positionEntity position = await GetPositionById(id);

                var checkIfEmployeeExists = await _unitOfWork.PersonRepository.FindAsync(a => a.Position_ID == id && a.Person_Type == "E" && a.ToDisplay == true);
                if (checkIfEmployeeExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Position, "Permanent Delete Position", "PERMANENT DELETE", false, "Unable to permanent delete " + position.Position_Name + " due to existing active employee", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Unable to permanent delete.", false);
                }

                position.Updated_By = user;

                var data = await _unitOfWork.PositionRepository.DeleteAsyncPermanent(position, position.Position_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(position.Updated_By, (int)Form.Campus_Position, "Delete Permanently Position", "PERMANENT DELETE", false, "Failed: " + position.Position_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Position", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(position.Updated_By, (int)Form.Campus_Position, "Delete Permanently Position", "PERMANENT DELETE", true, "Success: " + position.Position_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Position" + Constants.SuccessMessagePermanentDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeletePositionTemporary(int id, int user)
        {
            try
            {
                positionEntity position = await GetPositionById(id);

                var checkIfEmployeeExists = await _unitOfWork.PersonRepository.FindAsync(a => a.Position_ID == id && a.Person_Type == "E" && a.IsActive == true);
                if (checkIfEmployeeExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Position, "Deactivate Position", "DEACTIVATE", false, "Unable to deactivate " + position.Position_Name + " due to existing active employee/s.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                position.Updated_By = user;
                position.Position_Status = "Inactive";

                var data = await _unitOfWork.PositionRepository.DeleteAsyncTemporary(position, position.Position_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(position.Updated_By, (int)Form.Campus_Position, "Deactivate Position", "DEACTIVATE", false, "Failed: " + position.Position_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Position", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(position.Updated_By, (int)Form.Campus_Position, "Deactivate Position", "DEACTIVATE", true, "Success: " + position.Position_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Position" + Constants.SuccessMessageTemporaryDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> RetrievePosition(positionEntity position)
        {
            try
            {
                var newEntity = await _unitOfWork.PositionRepository.GetAsync(position.Position_ID);

                var checkIfDepartmentIsActive = await _unitOfWork.DepartmentRepository.FindAsync(a => a.Department_ID == newEntity.Department_ID);
                if (!checkIfDepartmentIsActive.IsActive)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(position.Updated_By, (int)Form.Campus_Position, "Activate Position", "ACTIVATE POSITION", false, "Unable to activate " + position.Position_Name + " due to inactive Department.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", UNABLE_ACTIVATE("Department", checkIfDepartmentIsActive.Department_Name), false);
                }

                newEntity.Position_Status = "Active";

                var data = await _unitOfWork.PositionRepository.RetrieveAsync(newEntity, newEntity.Position_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(position.Updated_By, (int)Form.Campus_Position, "Activate Position", "ACTIVATE POSITION", false, "Failed: " + position.Position_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Position", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(position.Updated_By, (int)Form.Campus_Position, "Activate Position", "ACTIVATE POSITION", true, "Success: " + position.Position_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Position" + Constants.SuccessMessageRetrieve, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<positionPagedResult> GetAllPosition(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.PositionRepository.GetAllPosition(pageNo, pageSize, keyword);
        }

        public async Task<positionPagedResult> ExportAllPositions(string keyword)
        {
            return await _unitOfWork.PositionRepository.ExportAllPositions(keyword);
        }

        public async Task<BatchUploadResponse> BatchUpload(ICollection<positionBatchUploadVM> positions, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "Position_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = positions.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var positionVM in positions)
            {
                i++;

                if (positionVM.CampusName == null || positionVM.CampusName == string.Empty)
                {
                    importLog.Logging(_posBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (positionVM.DepartmentName == null || positionVM.DepartmentName == string.Empty)
                {
                    importLog.Logging(_posBatch, fileName, "Row: " + i.ToString() + " ---> Department Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (positionVM.PositionName == null || positionVM.PositionName == string.Empty)
                {
                    importLog.Logging(_posBatch, fileName, "Row: " + i.ToString() + " ---> Position Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!Global.ValidateStr(positionVM.PositionName.Trim()))
                {
                    importLog.Logging(_posBatch, fileName, "Row: " + i.ToString() + " ---> Position Name does not accept special characters except space, dash, apostrophe and underscore.");
                    response.Failed++;
                    continue;
                }
                else if (positionVM.PositionName.Trim().Length > 125)
                {
                    importLog.Logging(_posBatch, fileName, "Row: " + i.ToString() + " ---> Position Name accepts 125 characters only.");
                    response.Failed++;
                    continue;
                }

                var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == positionVM.CampusName && x.IsActive == true);

                if (campus == null)
                {
                    importLog.Logging(_posBatch, fileName, "Row: " + i.ToString() + " ---> Campus " + positionVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                var department = await _unitOfWork.DepartmentRepository.FindAsync(x => x.Department_Name == positionVM.DepartmentName && x.Campus_ID == campus.Campus_ID && x.IsActive == true);

                if (department == null)
                {
                    importLog.Logging(_posBatch, fileName, "Row: " + i.ToString() + " ---> Department " + positionVM.DepartmentName + " under Campus " + positionVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                positionEntity position = await _unitOfWork.PositionRepository.FindAsync(x => x.Position_Name == positionVM.PositionName && x.IsActive == true && x.Department_ID == department.Department_ID);

                if (position != null)
                {
                    var updateCheck = await _unitOfWork.PositionRepository.FindAsync(q => q.Position_ID == position.Position_ID && q.Department_ID == position.Department_ID);

                    if (updateCheck == null)
                    {
                        var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.Position_ID == position.Position_ID);

                        if (recordCountPerson != null)
                        {
                            importLog.Logging(_posBatch, fileName, "Row: " + i.ToString() + " ---> Position " + positionVM.PositionName + ": " + UNABLE_EDIT);
                            response.Failed++;
                            continue;
                        }
                    }

                    position.Position_Name = positionVM.PositionName;
                    position.Department_ID = department.Department_ID;
                    position.Updated_By = user;

                    var isSuccess = await _unitOfWork.PositionRepository.UpdateAsyncWithBase(position, position.Position_ID);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Position, "Update Position By Batch", "UPDATE", true, "Success: " + position.Position_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_posBatch, fileName, "Position " + position.Position_Name + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_posBatch, fileName, "Position " + position.Position_Name + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    position = new positionEntity();
                    position.Position_Name = positionVM.PositionName;
                    position.Department_ID = department.Department_ID;
                    position.Updated_By = user;
                    position.Position_Status = "Active";
                    position.Added_By = user;

                    var isSuccess = await _unitOfWork.PositionRepository.AddAsyncWithBase(position);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Position, "Insert Position By Batch", "INSERT", true, "Success: " + position.Position_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_posBatch, fileName, "Position " + position.Position_Name + " successfully added.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_posBatch, fileName, "Position " + position.Position_Name + " failed to add.");
                        response.Failed++;
                        continue;
                    }
                }
            }

            return response;
        }
    }

}
