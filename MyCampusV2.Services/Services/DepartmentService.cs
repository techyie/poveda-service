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
    public class DepartmentService : BaseService, IDepartmentService
    {
        private string _deptBatch = AppDomain.CurrentDomain.BaseDirectory + @"Department\";
        private ResultModel result = new ResultModel();

        public DepartmentService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<IList<departmentEntity>> GetDepartmentsUsingCampusId(int id)
        {
            return await _unitOfWork.DepartmentRepository.GetDepartmentsUsingCampusId(id);
        }

        public async Task<departmentEntity> GetDepartmentById(int id)
        {
            return await _unitOfWork.DepartmentRepository.GetDepartmentById(id);
        }

        public async Task<ResultModel> AddDepartment(departmentEntity department)
        {
            try
            {
                department.Department_Status = "Active";

                var exist = await _unitOfWork.DepartmentRepository.FindAsync(q => q.Department_Name == department.Department_Name && q.IsActive == true && q.Campus_ID == department.Campus_ID);

                if (exist != null)
                    return CreateResult("409", DEPARTMENT_EXIST, false);

                var data = await _unitOfWork.DepartmentRepository.AddAsyncWithBase(department);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(department.Added_By, (int)Form.Campus_Area, "Add Department", "INSERT", false, "Failed: " + department.Department_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Department", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(department.Added_By, (int)Form.Campus_Area, "Add Department", "INSERT", true, "Success: " + department.Department_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Department" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateDepartment(departmentEntity department)
        {
            try
            {
                var exist = await _unitOfWork.DepartmentRepository.FindAsync(q => q.Department_Name == department.Department_Name && q.IsActive == true && q.Campus_ID == department.Campus_ID && q.Department_ID != department.Department_ID);

                if (exist != null)
                    return CreateResult("409", DEPARTMENT_EXIST, false);

                var updateCheck = await _unitOfWork.DepartmentRepository.FindAsync(q => q.Department_ID == department.Department_ID && q.Campus_ID == department.Campus_ID);

                if (updateCheck == null)
                {
                    var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.Department_ID == department.Department_ID);

                    if (recordCountPerson != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(department.Added_By, (int)Form.Campus_Department, "Update Department", "UPDATE", false, "Failed due to active record: " + department.Department_Name, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }

                    var recordCountPosition = await _unitOfWork.PositionRepository.FindAsync(q => q.Department_ID == department.Department_ID && q.IsActive == true);

                    if (recordCountPosition != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(department.Added_By, (int)Form.Campus_Department, "Update Department", "UPDATE", false, "Failed due to active record: " + department.Department_Name, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }
                }

                department.Department_Status = "Active";

                var data = await _unitOfWork.DepartmentRepository.UpdateAsyncWithBase(department, department.Department_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(department.Added_By, (int)Form.Campus_Department, "Update Department", "UPDATE", false, "Failed: " + department.Department_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Department", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(department.Updated_By, (int)Form.Campus_Department, "Update Department", "UPDATE", true, "Success: Department ID: " + department.Department_ID + " Department Name: " + department.Department_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Department" + Constants.SuccessMessageUpdate, true);
            } 
            catch(Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteDepartmentTemporary(int id, int user)
        {
            try
            {
                departmentEntity department = await GetDepartmentById(id);

                var checkIfPositionExists = await _unitOfWork.PositionRepository.FindAsync(a => a.Department_ID == id && a.IsActive == true);
                if (checkIfPositionExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Department, "Deactivate Department", "DEACTIVATE", false, "Unable to deactivate " + department.Department_Name + " due to existing active position/s.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                department.Updated_By = user;
                department.Department_Status = "Inactive";

                var data = await _unitOfWork.DepartmentRepository.DeleteAsyncTemporary(department, department.Department_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(department.Updated_By, (int)Form.Campus_Department, "Deactivate Department", "DEACTIVATE", false, "Failed: " + department.Department_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Department", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(department.Updated_By, (int)Form.Campus_Department, "Deactivate Department", "DEACTIVATE", true, "Success: " + department.Department_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Department" + Constants.SuccessMessageTemporaryDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteDepartmentPermanent(int id, int user)
        {
            try
            {
                departmentEntity department = await GetDepartmentById(id);

                var checkIfPositionExists = await _unitOfWork.PositionRepository.FindAsync(a => a.Department_ID == id && a.IsActive == true);
                if (checkIfPositionExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Department, "Permanent Delete Department", "PERMANENT DELETE", false, "Unable to permanent delete " + department.Department_Name + " due to existing active position", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Unable to permanent delete.", false);
                }

                department.Updated_By = user;

                var data = await _unitOfWork.DepartmentRepository.DeleteAsyncPermanent(department, department.Department_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(department.Updated_By, (int)Form.Campus_Area, "Deactivate Department", "DEACTIVATE", false, "Failed: " + department.Department_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Department", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(department.Updated_By, (int)Form.Campus_Area, "Deactivate Department", "DEACTIVATE", true, "Success: " + department.Department_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Department" + Constants.SuccessMessageTemporaryDelete, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> RetrieveDepartment(departmentEntity department)
        {
            try
            {
                var newEntity = await _unitOfWork.DepartmentRepository.GetAsync(department.Department_ID);

                var checkIfCampusIsActive = await _unitOfWork.CampusRepository.FindAsync(a => a.Campus_ID == newEntity.Campus_ID);
                if (!checkIfCampusIsActive.IsActive)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(department.Updated_By, (int)Form.Campus_Department, "Activate Department", "ACTIVATE DEPARTMENT", false, "Unable to activate " + department.Department_Name + " due to inactive Campus.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", UNABLE_ACTIVATE("Campus", checkIfCampusIsActive.Campus_Name), false);
                }

                newEntity.Department_Status = "Active";

                var data = await _unitOfWork.DepartmentRepository.RetrieveAsync(newEntity, newEntity.Department_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Department, "Activate Department", "ACTIVATE DEPARTMENT", false, "Failed: " + newEntity.Department_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Department", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Department, "Activate Department", "ACTIVATE DEPARTMENT", true, "Success: " + newEntity.Department_Name, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Department" + Constants.SuccessMessageRetrieve;
                result.isSuccess = true;

                return CreateResult("200", "Department" + Constants.SuccessMessageRetrieve, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<departmentPagedResult> GetAllDepartment(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.DepartmentRepository.GetAllDepartment(pageNo, pageSize, keyword);
        }

        public async Task<departmentPagedResult> ExportAllDepartments(string keyword)
        {
            return await _unitOfWork.DepartmentRepository.ExportAllDepartments(keyword);
        }

        public async Task<BatchUploadResponse> BatchUpload(ICollection<departmentBatchUploadVM> departments, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "Department_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = departments.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var departmentVM in departments)
            {
                i++;

                if (departmentVM.CampusName == null || departmentVM.CampusName == string.Empty)
                {
                    importLog.Logging(_deptBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (departmentVM.DepartmentName == null || departmentVM.DepartmentName == string.Empty)
                {
                    importLog.Logging(_deptBatch, fileName, "Row: " + i.ToString() + " ---> Department Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!Global.ValidateStr(departmentVM.DepartmentName.Trim()))
                {
                    importLog.Logging(_deptBatch, fileName, "Row: " + i.ToString() + " ---> Department Name does not accept special characters except space, dash, apostrophe and underscore.");
                    response.Failed++;
                    continue;
                }
                else if (departmentVM.DepartmentName.Trim().Length > 125)
                {
                    importLog.Logging(_deptBatch, fileName, "Row: " + i.ToString() + " ---> Department Name accepts 150 characters only.");
                    response.Failed++;
                    continue;
                }

                var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == departmentVM.CampusName && x.IsActive == true);

                if (campus == null)
                {
                    importLog.Logging(_deptBatch, fileName, "Row: " + i.ToString() + " ---> Campus " + departmentVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                departmentEntity department = await _unitOfWork.DepartmentRepository.FindAsync(x => x.Department_Name == departmentVM.DepartmentName && x.IsActive == true && x.Campus_ID == campus.Campus_ID);

                if (department != null)
                {
                    var updateCheck = await _unitOfWork.DepartmentRepository.FindAsync(q => q.Department_ID == department.Department_ID && q.Campus_ID == department.Campus_ID);

                    if (updateCheck == null)
                    {
                        var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.Department_ID == department.Department_ID);

                        if (recordCountPerson != null)
                        {
                            importLog.Logging(_deptBatch, fileName, "Row: " + i.ToString() + " ---> Department " + departmentVM.DepartmentName + ": " + UNABLE_EDIT);
                            response.Failed++;
                            continue;
                        }

                        var recordCountPosition = await _unitOfWork.PositionRepository.FindAsync(q => q.Department_ID == department.Department_ID && q.IsActive == true);

                        if (recordCountPosition != null)
                        {
                            importLog.Logging(_deptBatch, fileName, "Row: " + i.ToString() + " ---> Department " + departmentVM.DepartmentName + ": " + UNABLE_EDIT);
                            response.Failed++;
                            continue;
                        }
                    }

                    department.Department_Name = departmentVM.DepartmentName;
                    department.Campus_ID = campus.Campus_ID;
                    department.Updated_By = user;

                    var isSuccess = await _unitOfWork.DepartmentRepository.UpdateAsyncWithBase(department, department.Department_ID);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Department, "Update Department By Batch", "UPDATE", true, "Success: " + department.Department_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_deptBatch, fileName, "Department " + department.Department_Name + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_deptBatch, fileName, "Department " + department.Department_Name + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    department = new departmentEntity();
                    department.Department_Name = departmentVM.DepartmentName;
                    department.Campus_ID = campus.Campus_ID;
                    department.Department_Status = "Active";
                    department.IsActive = true;
                    department.Updated_By = user;
                    department.Added_By = user;

                    var isSuccess = await _unitOfWork.DepartmentRepository.AddAsyncWithBase(department);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Department, "Insert Department By Batch", "INSERT", true, "Success: " + department.Department_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_deptBatch, fileName, "Department " + department.Department_Name + " successfully added.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_deptBatch, fileName, "Department " + department.Department_Name + " failed to add.");
                        response.Failed++;
                        continue;
                    }
                }
            }

            return response;
        }
    }
}
