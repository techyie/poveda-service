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
    public class EmployeeSubTypeService : BaseService, IEmployeeSubTypeService
    {
        private ResultModel result = new ResultModel();

        public EmployeeSubTypeService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user)
            : base(unitOfWork, audit, user)
        {
        }

        public async Task<IList<employeeSubTypeEntity>> GetEmployeeSubTypesUsingEmployeeTypeId(int id) 
        {
            return await _unitOfWork.EmployeeSubTypeRepository.GetEmployeeSubTypesUsingEmployeeTypeId(id);
        }

        public async Task<employeeSubTypeEntity> GetEmployeeSubTypeById(int id)
        {
            return await _unitOfWork.EmployeeSubTypeRepository.GetEmployeeSubTypeById(id);
        }

        public async Task<IList<employeeSubTypeEntity>> GetEmployeeSubTypes()
        {
            return await _unitOfWork.EmployeeSubTypeRepository.GetEmployeeSubTypes();
        }

        public async Task<ResultModel> AddEmployeeSubType(employeeSubTypeEntity employeeSubType)
        {
            try
            {
                var exist = await _unitOfWork.EmployeeSubTypeRepository.FindAsync(q => q.EmpSubTypeDesc == employeeSubType.EmpSubTypeDesc && q.IsActive == true && q.EmpType_ID == employeeSubType.EmpType_ID);

                if (exist != null)
                    return CreateResult("409", EMPLOYEE_SUB_TYPE_EXIST, false);

                var data = await _unitOfWork.EmployeeSubTypeRepository.AddAsyncWithBase(employeeSubType);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeSubType.Added_By, (int)Form.Campus_Employee_Sub_Type, "Add Employee Sub Type", "INSERT", false, "Failed: " + employeeSubType.EmpSubTypeDesc, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Employee Sub Type", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeSubType.Added_By, (int)Form.Campus_Employee_Sub_Type, "Add Employee Sub Type", "INSERT", true, "Success: " + employeeSubType.EmpSubTypeDesc, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Employee Sub Type" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateEmployeeSubType(employeeSubTypeEntity employeeSubType)
        {
            try
            {
                var exist = await _unitOfWork.EmployeeSubTypeRepository.FindAsync(q => q.EmpSubTypeDesc == employeeSubType.EmpSubTypeDesc && q.IsActive == true && q.EmpType_ID == employeeSubType.EmpType_ID && q.EmpSubtype_ID != employeeSubType.EmpSubtype_ID);

                if (exist != null)
                    return CreateResult("409", EMPLOYEE_SUB_TYPE_EXIST, false);

                var updateCheck = await _unitOfWork.EmployeeSubTypeRepository.FindAsync(q => q.EmpSubtype_ID == employeeSubType.EmpSubtype_ID && q.EmpType_ID == employeeSubType.EmpType_ID);

                if (updateCheck == null)
                {
                    var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.EmpSubtype_ID == employeeSubType.EmpSubtype_ID);

                    if (recordCountPerson != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeSubType.Added_By, (int)Form.Campus_Employee_Sub_Type, "Update Employee Type", "UPDATE", false, "Failed due to active record: " + employeeSubType.EmpSubTypeDesc, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }
                }

                var data = await _unitOfWork.EmployeeSubTypeRepository.UpdateAsyncWithBase(employeeSubType, employeeSubType.EmpSubtype_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeSubType.Added_By, (int)Form.Campus_Employee_Sub_Type, "Update Employee Type", "UPDATE", false, "Failed: " + employeeSubType.EmpSubTypeDesc, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Employee Sub Type", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeSubType.Updated_By, (int)Form.Campus_Employee_Sub_Type, "Update Employee Type", "UPDATE", true, "Success: Employee Sub Type ID: " + employeeSubType.EmpSubtype_ID + " Employee Sub Type Name: " + employeeSubType.EmpSubTypeDesc, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Employee Sub Type" + Constants.SuccessMessageUpdate, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteEmployeeSubTypePermanent(employeeSubTypeEntity employeeSubType)
        {
            try
            {
                employeeSubTypeEntity newEntity = await _unitOfWork.EmployeeSubTypeRepository.FindAsync(q => q.EmpSubtype_ID == employeeSubType.EmpSubtype_ID);
                newEntity.Updated_By = employeeSubType.Updated_By;

                var checkIfEmployeeExists = await _unitOfWork.PersonRepository.FindAsync(a => a.EmpSubtype_ID == employeeSubType.EmpSubtype_ID && a.Person_Type == "E" && a.ToDisplay == true);
                if (checkIfEmployeeExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Employee_Sub_Type, "Delete Employee Sub Type", "DELETE", false, "Unable to delete " + newEntity.EmpSubTypeDesc + " due to existing active employee/s.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", UNABLE_DELETE, false);
                }

                var data = await _unitOfWork.EmployeeSubTypeRepository.DeleteAsyncPermanent(newEntity, newEntity.EmpSubtype_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Employee_Sub_Type, "Delete Employee Sub Type", "DELETE", false, "Failed: " + newEntity.EmpSubTypeDesc, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Employee Sub Type", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Employee_Sub_Type, "Delete Employee Sub Type", "DELETE", true, "Success: " + newEntity.EmpSubTypeDesc, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Employee Sub Type" + Constants.SuccessMessageDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteEmployeeSubTypeTemporary(employeeSubTypeEntity employeeSubType)
        {
            try
            {
                employeeSubTypeEntity newEntity = await _unitOfWork.EmployeeSubTypeRepository.FindAsync(q => q.EmpSubtype_ID == employeeSubType.EmpSubtype_ID);

                var checkIfEmployeeExists = await _unitOfWork.PersonRepository.FindAsync(a => a.EmpSubtype_ID == newEntity.EmpSubtype_ID && a.Person_Type == "E" && a.IsActive == true);
                if (checkIfEmployeeExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeSubType.Updated_By, (int)Form.Campus_Employee_Sub_Type, "Deactivate Employee Sub Type", "DEACTIVATE", false, "Failed: " + employeeSubType.EmpSubTypeDesc + " due to existing active employee/s.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                newEntity.Updated_By = employeeSubType.Updated_By;

                var data = await _unitOfWork.EmployeeSubTypeRepository.DeleteAsyncTemporary(newEntity, newEntity.EmpSubtype_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeSubType.Updated_By, (int)Form.Campus_Employee_Sub_Type, "Deactivate Employee Sub Type", "DEACTIVATE", false, "Failed: " + employeeSubType.EmpSubTypeDesc, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Employee Sub Type", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeSubType.Updated_By, (int)Form.Campus_Employee_Sub_Type, "Deactivate Employee Sub Type", "DEACTIVATE", true, "Success: " + employeeSubType.EmpSubTypeDesc, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Employee Sub Type" + Constants.SuccessMessageTemporaryDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> RetrieveEmployeeSubType(employeeSubTypeEntity employeeSubType)
        {
            try
            {
                employeeSubTypeEntity newEntity = await _unitOfWork.EmployeeSubTypeRepository.FindAsync(q => q.EmpSubtype_ID == employeeSubType.EmpSubtype_ID);

                var checkIfEmployeeTypeIsActive = await _unitOfWork.EmployeeTypeRepository.FindAsync(a => a.EmpType_ID == newEntity.EmpType_ID);
                if (!checkIfEmployeeTypeIsActive.IsActive)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(checkIfEmployeeTypeIsActive.Updated_By, (int)Form.Campus_Employee_Sub_Type, "Activate Employee Sub Type", "ACTIVATE EMPLOYEE SUB TYPE", false, "Unable to activate " + employeeSubType.EmpSubTypeDesc + " due to inactive Employee Type.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", UNABLE_ACTIVATE("Employee Type", checkIfEmployeeTypeIsActive.EmpTypeDesc), false);
                }

                newEntity.Updated_By = employeeSubType.Updated_By;

                var data = await _unitOfWork.EmployeeSubTypeRepository.RetrieveAsync(newEntity, newEntity.EmpSubtype_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeSubType.Updated_By, (int)Form.Campus_Employee_Sub_Type, "Activate Employee Sub Type", "ACTIVATE EMPLOYEE SUB TYPE", false, "Failed: " + employeeSubType.EmpSubTypeDesc, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Employee Sub Type", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employeeSubType.Updated_By, (int)Form.Campus_Employee_Sub_Type, "Activate Employee Sub Type", "ACTIVATE EMPLOYEE SUB TYPE", true, "Success: " + employeeSubType.EmpSubTypeDesc, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Employee Sub Type" + Constants.SuccessMessageRetrieve, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<employeeSubTypePagedResult> GetAllEmployeeSubType(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.EmployeeSubTypeRepository.GetAllEmployeeSubType(pageNo, pageSize, keyword);
        }
    }
}
