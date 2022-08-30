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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Services
{
    public class AreaService : BaseService, IAreaService
    {
        private string _areaBatch = AppDomain.CurrentDomain.BaseDirectory + @"Area\";
        private ResultModel result = new ResultModel();

        public AreaService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) 
            : base(unitOfWork, audit, user) 
        {

        }

        public async Task<IList<areaEntity>> GetAreasUsingCampusId(int id)
        {
            return await _unitOfWork.AreaRepository.GetAreasUsingCampusId(id);
        }

        public async Task<IList<areaEntity>> GetAreasUsingCampusName(string campus)
        {
            return await _unitOfWork.AreaRepository.GetAreasUsingCampusName(campus);
        }

        public async Task<areaEntity> GetAreaById(int id)
        {
           return await _unitOfWork.AreaRepository.GetAreaById(id);
        }

        public async Task<ResultModel> AddArea(areaEntity area)
        {
            try
            {
                area.Area_Status = "Active";

                var exist = await _unitOfWork.AreaRepository.FindAsync(q => q.Area_Name == area.Area_Name && q.IsActive == true && q.Campus_ID == area.Campus_ID);

                if (exist != null)
                    return CreateResult("409", AREA_EXIST, false);

                var data = await _unitOfWork.AreaRepository.AddAsyncWithBase(area);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(area.Added_By, (int)Form.Campus_Area, "Add Area", "INSERT", false, "Failed: " + area.Area_Name, DateTime.UtcNow.ToLocalTime()));

                    return CreateResult("409", "Area", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(area.Added_By, (int)Form.Campus_Area, "Add Area", "INSERT", true, "Success: " + area.Area_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Area" + Constants.SuccessMessageAdd, true);

            } catch(Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateArea(areaEntity area)
        {
            try
            {
                var exist = await _unitOfWork.AreaRepository.FindAsync(q => q.Area_Name == area.Area_Name && q.IsActive == true && q.Campus_ID == area.Campus_ID && q.Area_ID != area.Area_ID);

                if (exist != null)
                    return CreateResult("409", AREA_EXIST, false);

                var updateCheck = await _unitOfWork.AreaRepository.FindAsync(q => q.Area_ID == area.Area_ID && q.Campus_ID == area.Campus_ID);

                if (updateCheck == null)
                {
                    var recordCountTerminal = await _unitOfWork.TerminalRepository.FindAsync(q => q.Area_ID == area.Area_ID);

                    if (recordCountTerminal != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(area.Updated_By, (int)Form.Campus_Area, "Update Area", "UPDATE", false, "Failed due to active record: " + area.Area_Name, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }
                }

                area.Area_Status = "Active";

                var data = await _unitOfWork.AreaRepository.UpdateAsyncWithBase(area, area.Area_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(area.Updated_By, (int)Form.Campus_Area, "Update Area", "UPDATE", false, "Failed: " + area.Area_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Area", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(area.Updated_By, (int)Form.Campus_Area, "Update Area", "UPDATE", true, "Success: Area ID: " + area.Area_ID + " Area Name: " + area.Area_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Area" + Constants.SuccessMessageUpdate, true);

            } catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteAreaPermanent(int id, int user)
        {
            try
            {
                areaEntity area = await GetAreaById(id);

                area.Updated_By = user;

                var data = await _unitOfWork.AreaRepository.DeleteAsyncPermanent(area, area.Area_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(area.Updated_By, (int)Form.Campus_Area, "Delete Permanently Area", "PERMANENT DELETE", false, "Failed: " + area.Area_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Area", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(area.Updated_By, (int)Form.Campus_Area, "Delete Permanently Area", "PERMANENT DELETE", true, "Success: " + area.Area_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Area" + Constants.SuccessMessagePermanentDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteAreaTemporary(int id, int user)
        {
            try
            {
                areaEntity area = await GetAreaById(id);

                var checkIfTerminalExists = await _unitOfWork.TerminalRepository.FindAsync(a => a.Area_ID == id && a.IsActive == true);
                if (checkIfTerminalExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(area.Updated_By, (int)Form.Campus_Area, "Deactivate Area", "DEACTIVATE", false, "Unable to deactivate " + area.Area_Name + " due to existing active terminal.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                area.Updated_By = user;
                area.Area_Status = "Inactive";

                var data = await _unitOfWork.AreaRepository.DeleteAsyncTemporary(area, area.Area_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(area.Updated_By, (int)Form.Campus_Area, "Deactivate Area", "DEACTIVATE", false, "Failed: " + area.Area_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Area", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(area.Updated_By, (int)Form.Campus_Area, "Deactivate Area", "DEACTIVATE", true, "Success: " + area.Area_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Area" + Constants.SuccessMessageTemporaryDelete, true);

            } catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> RetrieveArea(areaEntity area)
        {
            try
            {
                var newEntity = await _unitOfWork.AreaRepository.GetAsync(area.Area_ID);

                var checkIfCampusIsActive = await _unitOfWork.CampusRepository.FindAsync(a => a.Campus_ID == newEntity.Campus_ID);
                if (!checkIfCampusIsActive.IsActive)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(area.Updated_By, (int)Form.Campus_Area, "Activate Area", "ACTIVATE AREA", false, "Unable to activate " + area.Area_Name + " due to inactive Campus.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", UNABLE_ACTIVATE("Campus", checkIfCampusIsActive.Campus_Name), false);
                }

                newEntity.Area_Status = "Active";

                var data = await _unitOfWork.AreaRepository.RetrieveAsync(newEntity, newEntity.Area_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Area, "Activate Area", "ACTIVATE AREA", false, "Failed: " + newEntity.Area_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Area, "Activate Area", "ACTIVATE AREA", true, "Success: " + newEntity.Area_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Area" + Constants.SuccessMessageRetrieve, true);

            } catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ICollection<areaEntity>> GetAreaByUsingCampusId(int id)
        {
            try
            {
                return await _unitOfWork.AreaRepository.FindByAsyn(q => q.Campus_ID == id && q.IsActive == true && q.ToDisplay == true);
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<areaPagedResult> GetAllArea(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.AreaRepository.GetAllArea(pageNo, pageSize, keyword);
        }

        public async Task<areaPagedResult> ExportAllAreas(string keyword)
        {
            return await _unitOfWork.AreaRepository.ExportAllAreas(keyword);
        }

        public async Task<BatchUploadResponse> BatchUpload(ICollection<areaBatchUploadVM> areas, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "Area_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = areas.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var areaVM in areas)
            {
                i++;

                if (areaVM.CampusName == null || areaVM.CampusName == string.Empty)
                {
                    importLog.Logging(_areaBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (areaVM.AreaName == null || areaVM.AreaName == string.Empty)
                {
                    importLog.Logging(_areaBatch, fileName, "Row: " + i.ToString() + " ---> Area Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!Global.ValidateStr(areaVM.AreaName.Trim()))
                {
                    importLog.Logging(_areaBatch, fileName, "Row: " + i.ToString() + " ---> Area Name does not accept special characters except space, dash, apostrophe and underscore.");
                    response.Failed++;
                    continue;
                }
                else if (areaVM.AreaName.Trim().Length > 100)
                {
                    importLog.Logging(_areaBatch, fileName, "Row: " + i.ToString() + " ---> Area Name accepts 100 characters only.");
                    response.Failed++;
                    continue;
                }

                if (areaVM.AreaDescription == null || areaVM.AreaDescription == string.Empty)
                {
                    importLog.Logging(_areaBatch, fileName, "Row: " + i.ToString() + " ---> Area Description is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!Global.ValidateStr(areaVM.AreaDescription.Trim()))
                {
                    importLog.Logging(_areaBatch, fileName, "Row: " + i.ToString() + " ---> Area Description does not accept special characters except space, dash, apostrophe and underscore.");
                    response.Failed++;
                    continue;
                }
                else if (areaVM.AreaDescription.Trim().Length > 300)
                {
                    importLog.Logging(_areaBatch, fileName, "Row: " + i.ToString() + " ---> Area Description accepts 300 characters only.");
                    response.Failed++;
                    continue;
                }

                var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == areaVM.CampusName && x.IsActive == true);

                if (campus == null)
                {
                    importLog.Logging(_areaBatch, fileName, "Row: " + i.ToString() + " ---> Campus " + areaVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                areaEntity area = await _unitOfWork.AreaRepository.FindAsync(x => x.Area_Name == areaVM.AreaName && x.IsActive == true && x.Campus_ID == campus.Campus_ID);

                if (area != null)
                {
                    var updateCheck = await _unitOfWork.AreaRepository.FindAsync(q => q.Area_ID == area.Area_ID && q.Campus_ID == area.Campus_ID);

                    if (updateCheck == null)
                    {
                        var recordCountTerminal = await _unitOfWork.TerminalRepository.FindAsync(q => q.Area_ID == area.Area_ID);

                        if (recordCountTerminal != null)
                        {
                            importLog.Logging(_areaBatch, fileName, "Row: " + i.ToString() + " ---> Area " + areaVM.AreaName + ": " + UNABLE_EDIT);
                            response.Failed++;
                            continue;
                        }
                    }

                    area.Area_Name = areaVM.AreaName;
                    area.Area_Description = areaVM.AreaDescription;
                    area.Campus_ID = campus.Campus_ID;
                    area.Updated_By = user;

                    var isSuccess = await _unitOfWork.AreaRepository.UpdateAsyncWithBase(area, area.Area_ID);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Area, "Update Area By Batch", "UPDATE", true, "Success: " + area.Area_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_areaBatch, fileName, "Area " + area.Area_Name + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_areaBatch, fileName, "Area " + area.Area_Name + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    area = new areaEntity();
                    area.Area_Name = areaVM.AreaName;
                    area.Area_Description = areaVM.AreaDescription;
                    area.Campus_ID = campus.Campus_ID;
                    area.Updated_By = user;
                    area.Added_By = user;
                    area.Area_Status = "Active";

                    var isSuccess = await _unitOfWork.AreaRepository.AddAsyncWithBase(area);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Area, "Insert Area By Batch", "INSERT", true, "Success: " + area.Area_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_areaBatch, fileName, "Area " + area.Area_Name + " successfully added.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_areaBatch, fileName, "Area " + area.Area_Name + " failed to add.");
                        response.Failed++;
                        continue;
                    }
                }
            }

            return response;
        }
    }
}
