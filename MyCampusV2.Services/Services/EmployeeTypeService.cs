using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace MyCampusV2.Services.Services
{
    public class EmployeeTypeService : BaseService, IEmployeeTypeService
    {
        private ResultModel result = new ResultModel();

        public EmployeeTypeService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user)
            : base(unitOfWork, audit, user)
        {
        }

        public async Task<IList<empTypeEntity>> GetEmployeeTypesUsingCampusId(int id)
        {
            return await _unitOfWork.EmployeeTypeRepository.GetEmployeeTypesUsingCampusId(id);
        }

        public async Task<IList<empTypeEntity>> GetEmployeeTypes()
        {
            return await _unitOfWork.EmployeeTypeRepository.GetEmployeeTypes();
        }

        public async Task<empTypeEntity> GetEmployeeTypeById(int id)
        {
            return await _unitOfWork.EmployeeTypeRepository.GetEmployeeTypeById(id);
        }

        public async Task<ResultModel> AddEmployeeType(empTypeEntity employeeType)
        {
            try
            {
                var exist = await _unitOfWork.EmployeeTypeRepository.FindAsync(q => q.EmpTypeDesc == employeeType.EmpTypeDesc && q.IsActive == true && q.Campus_ID == employeeType.Campus_ID);

                if (exist != null)
                    return CreateResult("409", EMPLOYEE_TYPE_EXIST, false);

                var data = await _unitOfWork.EmployeeTypeRepository.AddAsyncWithBase(employeeType);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeType.Added_By, (int)Form.Campus_Employee_Type, "Add Employee Type", "INSERT", false, "Failed: " + employeeType.EmpTypeDesc, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Employee Type", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeType.Added_By, (int)Form.Campus_Employee_Type, "Add Employee Type", "INSERT", true, "Success: " + employeeType.EmpTypeDesc, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Employee Type" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateEmployeeType(empTypeEntity employeeType)
        {
            try
            {
                var exist = await _unitOfWork.EmployeeTypeRepository.FindAsync(q => q.EmpTypeDesc == employeeType.EmpTypeDesc && q.IsActive == true && q.Campus_ID == employeeType.Campus_ID && q.EmpType_ID != employeeType.EmpType_ID);

                if (exist != null)
                    return CreateResult("409", EMPLOYEE_TYPE_EXIST, false);

                var updateCheck = await _unitOfWork.EmployeeTypeRepository.FindAsync(q => q.EmpType_ID == employeeType.EmpType_ID && q.Campus_ID == employeeType.Campus_ID);

                if (updateCheck == null)
                {
                    var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.EmpType_ID == employeeType.EmpType_ID);

                    if (recordCountPerson != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeType.Added_By, (int)Form.Campus_Employee_Type, "Update Employee Type", "UPDATE", false, "Failed due to active record: " + employeeType.EmpTypeDesc, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }

                    var recordCountEmployeeSubType = await _unitOfWork.EmployeeSubTypeRepository.FindAsync(q => q.EmpType_ID == employeeType.EmpType_ID && q.IsActive == true);

                    if (recordCountEmployeeSubType != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeType.Added_By, (int)Form.Campus_Employee_Type, "Update Employee Type", "UPDATE", false, "Failed due to active record: " + employeeType.EmpTypeDesc, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }
                }

                var data = await _unitOfWork.EmployeeTypeRepository.UpdateAsyncWithBase(employeeType, employeeType.EmpType_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeType.Added_By, (int)Form.Campus_Employee_Type, "Update Employee Type", "UPDATE", false, "Failed: " + employeeType.EmpTypeDesc, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Employee Type", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeType.Updated_By, (int)Form.Campus_Employee_Type, "Update Employee Type", "UPDATE", true, "Success: Employee Type ID: " + employeeType.EmpType_ID + " Employee Type Name: " + employeeType.EmpTypeDesc, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Employee Type" + Constants.SuccessMessageUpdate, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteEmployeeTypePermanent(empTypeEntity employeeType)
        {
            try
            {
                var data = await _unitOfWork.EmployeeTypeRepository.DeleteAsyncPermanent(employeeType, employeeType.EmpType_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeType.Updated_By, (int)Form.Campus_Employee_Type, "Delete Permanently Employee Type", "PERMANENT DELETE", false, "Failed: " + employeeType.EmpTypeDesc, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Employee Type", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeType.Updated_By, (int)Form.Campus_Employee_Type, "Delete Permanently Employee Type", "PERMANENT DELETE", true, "Success: " + employeeType.EmpTypeDesc, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Employee Type" + Constants.SuccessMessagePermanentDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteEmployeeTypeTemporary(empTypeEntity employeeType)
        {
            try
            {

                empTypeEntity newEntity = await _unitOfWork.EmployeeTypeRepository.FindAsync(q => q.EmpType_ID == employeeType.EmpType_ID);

                var checkIfEmployeeExists = await _unitOfWork.PersonRepository.FindAsync(a => a.EmpType_ID == newEntity.EmpType_ID && a.Person_Type == "E" && a.IsActive == true);
                if (checkIfEmployeeExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeType.Updated_By, (int)Form.Campus_Employee_Type, "Deactivate Employee Type", "DEACTIVATE", false, "Unable to deactivate " + employeeType.EmpTypeDesc + " due to existing active employee/s.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                var checkIfEmployeeTypeExists = await _unitOfWork.EmployeeSubTypeRepository.FindAsync(a => a.EmpType_ID == newEntity.EmpType_ID && a.IsActive == true);
                if (checkIfEmployeeTypeExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeType.Updated_By, (int)Form.Campus_Employee_Type, "Deactivate Employee Type", "DEACTIVATE", false, "Unable to deactivate " + employeeType.EmpTypeDesc + " due to existing active employee sub type/s.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                newEntity.Updated_By = employeeType.Updated_By;

                var data = await _unitOfWork.EmployeeTypeRepository.DeleteAsyncTemporary(newEntity, newEntity.EmpType_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeType.Updated_By, (int)Form.Campus_Employee_Type, "Deactivate Employee Type", "DEACTIVATE", false, "Failed: " + employeeType.EmpTypeDesc, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Employee Type", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeType.Updated_By, (int)Form.Campus_Employee_Type, "Deactivate Employee Type", "DEACTIVATE", true, "Success: " + employeeType.EmpTypeDesc, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Employee Type" + Constants.SuccessMessageTemporaryDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> RetrieveEmployeeType(empTypeEntity employeeType)
        {
            try
            {
                empTypeEntity newEntity = await _unitOfWork.EmployeeTypeRepository.FindAsync(q => q.EmpType_ID == employeeType.EmpType_ID);

                var checkIfCampusIsActive = await _unitOfWork.CampusRepository.FindAsync(a => a.Campus_ID == newEntity.Campus_ID);
                if (!checkIfCampusIsActive.IsActive)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeType.Updated_By, (int)Form.Campus_Employee_Type, "Activate Employee Type", "ACTIVATE EMPLOYEE TYPE", false, "Unable to activate " + newEntity.EmpTypeDesc + " due to inactive Campus.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", UNABLE_ACTIVATE("Campus", checkIfCampusIsActive.Campus_Name), false);
                }

                newEntity.Updated_By = employeeType.Updated_By;

                var data = await _unitOfWork.EmployeeTypeRepository.RetrieveAsync(newEntity, newEntity.EmpType_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Employee_Type, "Activate Employee Type", "ACTIVATE EMPLOYEE TYPE", false, "Failed: " + newEntity.EmpTypeDesc, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Employee Type", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Employee_Type, "Activate Employee Type", "ACTIVATE EMPLOYEE TYPE", true, "Success: " + newEntity.EmpTypeDesc, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Employee Type" + Constants.SuccessMessageRetrieve, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<employeeTypePagedResult> GetAllEmployeeType(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.EmployeeTypeRepository.GetAllEmployeeType(pageNo, pageSize, keyword);
        }
    }
}