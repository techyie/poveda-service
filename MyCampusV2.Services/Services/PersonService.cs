using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Services.Helpers;
using System.ComponentModel.DataAnnotations;

namespace MyCampusV2.Services
{
    public class PersonService : BaseService, IPersonService
    {
        private string _personstudentBatch = AppDomain.CurrentDomain.BaseDirectory + @"PersonStudent\";
        private string _personemployeeBatch = AppDomain.CurrentDomain.BaseDirectory + @"PersonEmployee\";
        private string _personvisitorBatch = AppDomain.CurrentDomain.BaseDirectory + @"PersonVisitor\";
        private string _personfetcherBatch = AppDomain.CurrentDomain.BaseDirectory + @"PersonFetcher\";
        private string _personotheraccessBatch = AppDomain.CurrentDomain.BaseDirectory + @"PersonOtherAccess\";
        private ResultModel result = new ResultModel();
        private ImportLog errorLogging = new ImportLog();
        
        public PersonService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<ICollection<personEntity>> GetEmployeeList()
        {
            return await _unitOfWork.PersonRepository.GetAllEmployee().ToListAsync();
        }

        public async Task<ICollection<personEntity>> GetStudentList()
        {
            return await _unitOfWork.PersonRepository.GetAllStudent().ToListAsync();
        }

        public async Task<ICollection<personEntity>> GetFetcherList()
        {
            try
            {
                return await _unitOfWork.PersonRepository.GetAllFetcher().ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ICollection<personEntity>> GetOtherAccessList()
        {
            try
            {
                return await _unitOfWork.PersonRepository.GetAllOtherAccess().ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ICollection<personEntity>> GetAllPersonVisitor()
        {
            return await _unitOfWork.PersonRepository.GetAllPersonVisitor().ToListAsync();
        }

        public async Task<personEntity> GetByPersonVisitorID(int id)
        {
            return await _unitOfWork.PersonRepository.GetAllPersonVisitor().Where(x => x.Person_ID == id).FirstOrDefaultAsync();
        }

        public async Task<ResultModel> AddEmployee(personEntity employee, emergencyContactEntity emergencyContact, govIdsEntity govIds, int user)
        {
            {
                try
                {
                    var personcheck = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == employee.ID_Number);
                    if (personcheck != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Added_By, (int)Form.Person_Employee, "Add Employee", "INSERT", false, "Failed: " + employee.ID_Number, DateTime.UtcNow.ToLocalTime()));
                        result = new ResultModel();
                        result.resultCode = "409";
                        result.resultMessage = "Employee Registration Failed.";
                        result.isSuccess = false;
                        return result;
                    }

                    employee.Gender = (employee.Gender == "Male" ? "M" : "F");
                    employee.Person_Type = "E";
                    employee.Added_By = user;
                    employee.Updated_By = user;

                    var data = await _unitOfWork.PersonRepository.AddAsyncWithBase(employee);

                    if (data == null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Added_By, (int)Form.Person_Employee, "Add Employee", "INSERT", false, "Failed: " + employee.ID_Number, DateTime.UtcNow.ToLocalTime()));
                        result = new ResultModel();
                        result.resultCode = "409";
                        result.resultMessage = "Employee Registration Failed.";
                        result.isSuccess = false;
                        return result;
                    }

                    var exist = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == employee.ID_Number);

                    emergencyContact.Connected_PersonID = exist.Person_ID;
                    govIds.Person_ID = exist.Person_ID;
                    
                    var detailsResult = await _unitOfWork.PersonRepository.AddEmployee(emergencyContact, govIds, user);

                    if (detailsResult.isSuccess == false)
                        return detailsResult;

                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Added_By, (int)Form.Person_Employee, "Add Employee", "INSERT", true, "Success: " + employee.ID_Number, DateTime.UtcNow.ToLocalTime()));

                    result = new ResultModel();
                    result.resultCode = "200";
                    result.resultMessage = "Employee " + Constants.SuccessMessageAdd;
                    result.isSuccess = true;
                    return result;

                }
                catch (Exception err)
                {
                    result = new ResultModel();
                    result.resultCode = "500";
                    result.resultMessage = err.Message;
                    result.isSuccess = false;
                    return result;
                }
            }
        }

        public async Task<ResultModel> UpdateEmployee(personEntity employee, emergencyContactEntity emergencyContact, govIdsEntity govIds, int user)
        {
            try
            {
                var exist = await _unitOfWork.PersonRepository.GetEmployeeById(employee.Person_ID);

                employee.Gender = (employee.Gender == "Male" ? "M" : "F");
                employee.Person_Type = "E";
                employee.Updated_By = user;
                employee.IsActive = true;

                var data = await _unitOfWork.PersonRepository.UpdateAsyncWithBase(employee, employee.Person_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Updated_By, (int)Form.Person_Employee, "Update Employee", "UPDATE", false, "Failed: " + employee.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                emergencyContact.Connected_PersonID = employee.Person_ID;
                govIds.Person_ID = employee.Person_ID;

                var detailsResult = await _unitOfWork.PersonRepository.UpdateEmployee(emergencyContact, govIds, user);

                if (detailsResult.isSuccess == false)
                    return detailsResult;

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Updated_By, (int)Form.Person_Employee, "Update Employee", "UPDATE", true, "Success: ID Number: " + employee.ID_Number + " Employee Name: " + (employee.Last_Name + ", " + employee.First_Name), DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Employee " + Constants.SuccessMessageUpdate;
                result.isSuccess = true;

                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }

        public async Task<personEntity> GetEmployeeByIDNumber(string IDNumber)
        {
            return await _unitOfWork.PersonRepository.GetAllEmployee().Where(x => x.ID_Number == IDNumber).FirstOrDefaultAsync();
        }

        public async Task<personEntity> GetStudentByIDNumber(string IDNumber)
        {
            try {
                return await _unitOfWork.PersonRepository.GetAllStudent().Where(x => x.ID_Number == IDNumber).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task AddStudent(personEntity personInfo, emergencyContactEntity emergencyInfo, int user)
        //{
        //    try
        //    {
        //        await _unitOfWork.PersonRepository.AddStudent(personInfo, emergencyInfo, user);
        //        await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Person_Student,
        //            string.Format("Added: ID Number: {0}, First Name: {1}, Last Name: {2}", personInfo.ID_Number, personInfo.First_Name, personInfo.Last_Name));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task UpdateStudent(personEntity personInfo, emergencyContactEntity emergencyInfo, int user)
        //{
        //    try
        //    {
        //        await _unitOfWork.PersonRepository.UpdateStudent(personInfo, emergencyInfo, user);
        //        await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Person_Student,
        //            string.Format("Updated: ID Number: {0}, First Name: {1}, Last Name: {2}", personInfo.ID_Number, personInfo.First_Name, personInfo.Last_Name));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<ICollection<personEntity>> GetAllWithOutCard()
        {
            try
            {
                return await _unitOfWork.PersonRepository.GetAllWithOutCard().ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ICollection<personEntity>> GetAllWithCard()
        {
            try
            {
                return await _unitOfWork.PersonRepository.GetAllWithCard().ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<personEntity> GetPersonByID(long id)
        {
            try
            {
                return await _unitOfWork.PersonRepository.FindAsync(q => q.Person_ID == Convert.ToInt32(id));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<personEntity> GetPersonByIDNumber(string idNumber)
        {
            try
            {
                return await _unitOfWork.PersonRepository.FindAsync(q => q.ID_Number == idNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AddPersonVisitor(personEntity personInfo, int user)
        {
            try
            {
                await _unitOfWork.PersonRepository.AddPersonVisitor(personInfo, user);
                await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Person_Visitor,
                    string.Format("Added: ID Number: {0}, First Name: {1}, Last Name: {2}", personInfo.ID_Number, personInfo.First_Name, personInfo.Last_Name));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdatePersonVisitor(personEntity personInfo, int user)
        {
            try
            {
                await _unitOfWork.PersonRepository.UpdatePersonVisitor(personInfo, user);
                await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Person_Visitor,
                    string.Format("Updated: ID Number: {0}, First Name: {1}, Last Name: {2}", personInfo.ID_Number, personInfo.First_Name, personInfo.Last_Name));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<personEntity> GetPersonVisitor(long id)
        {
            return await _unitOfWork.PersonRepository.FindAsync(x => x.Person_ID == id);
        }

        public async Task DeletePersonVisitor(long id, int user)
        {
            try
            {
                personEntity person = await GetPersonVisitor(id);

                if (person.IsActive)
                {
                    person.IsActive = false;
                }
                else
                {
                    person.IsActive = true;
                }

                person.Last_Updated = DateTime.Now;
                person.Updated_By = user;

                await _unitOfWork.PersonRepository.UpdateAsyn(person, person.Person_ID);
                await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Person_Visitor, string.Format("Updated Person Visitor {1} status to {0} ", person.IsActive ? "Active" : "Inactive", person.ID_Number));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AddPersonVisitorFromBatchUpload(personEntity person, int user)
        {
            try
            {
                person.Person_Type = "V";
                person.Added_By = user;
                person.Date_Time_Added = DateTime.Now;
                person.Updated_By = user;
                person.Last_Updated = DateTime.Now;
                person.IsActive = true;
                person.ToDisplay = true;

                await _unitOfWork.PersonRepository.AddAsyn(person);
                await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Person_Visitor,
                    string.Format("Added: ID Number: {0}, First Name: {1}, Last Name: {2}", person.ID_Number, person.First_Name, person.Last_Name));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
        
        private EmailAddressAttribute mailCheck = new EmailAddressAttribute();

        public async Task<BatchUploadResponse> BatchUpload(ICollection<visitorPersonBatchUploadVM> personvisitors, int user,int uploadID,int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "Visitor_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = personvisitors.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var personvisitorVM in personvisitors)
            {
                i++;

                if (personvisitorVM.IDNumber == null || personvisitorVM.IDNumber == string.Empty)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> ID Number is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (personvisitorVM.IDNumber.Trim().Length > 20)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> ID Number accepts 20 characters only.");
                    response.Failed++;
                    continue;
                }

                if (personvisitorVM.FirstName == null || personvisitorVM.FirstName == string.Empty)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> First Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (personvisitorVM.FirstName.Trim().Length > 100)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> First Name accepts 100 characters only.");
                    response.Failed++;
                    continue;
                }

                if (personvisitorVM.LastName == null || personvisitorVM.LastName == string.Empty)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Last Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (personvisitorVM.LastName.Trim().Length > 100)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Last Name accepts 100 characters only.");
                    response.Failed++;
                    continue;
                }

                if (personvisitorVM.BirthDate == "" || personvisitorVM.BirthDate == null)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Birthdate is a required field.");
                    response.Failed++;
                    continue;
                }

                DateTime temp;
                if (!DateTime.TryParse(personvisitorVM.BirthDate, out temp))
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Birthdate is invalid.");
                    response.Failed++;
                    continue;
                }

                if (personvisitorVM.Gender == null || personvisitorVM.Gender == string.Empty)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Gender is a required field.");
                    response.Failed++;
                    continue;
                }

                if (personvisitorVM.Gender.Trim().ToLower() != "m" && personvisitorVM.Gender.Trim().ToLower() != "f")
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Gender is a required field.");
                    response.Failed++;
                    continue;
                }

                if (personvisitorVM.ContactNumber.Trim() == null || personvisitorVM.ContactNumber.Trim() == string.Empty)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!IsDigitsOnly(personvisitorVM.ContactNumber.Trim()))
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number is invalid.");
                    response.Failed++;
                    continue;
                }
                else if (personvisitorVM.ContactNumber.Trim().Length > 11)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number accepts 11 digits only.");
                    response.Failed++;
                    continue;
                }

                if (personvisitorVM.EmailAddress.Trim() == null || personvisitorVM.EmailAddress.Trim() == string.Empty)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Email Address is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!new EmailAddressAttribute().IsValid(personvisitorVM.EmailAddress.Trim()))
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Email Address is invalid.");
                    response.Failed++;
                    continue;
                }
                else if (personvisitorVM.EmailAddress.Trim().Length > 100)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Email Address accepts 100 characters only.");
                    response.Failed++;
                    continue;
                }

                if (personvisitorVM.Address == null || personvisitorVM.Address == string.Empty)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Address is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (personvisitorVM.Address.Trim().Length > 225)
                {
                    importLog.Logging(_personvisitorBatch, fileName, "Row: " + i.ToString() + " ---> Address accepts 225 characters only.");
                    response.Failed++;
                    continue;
                }

                personEntity person = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == personvisitorVM.IDNumber && x.IsActive == true);

                if (person != null)
                {
                    /*
                    Boolean isChange = false;

                    personEntity personinfo = new personEntity();
                    personinfo.ID_Number = personvisitorVM.IDNumber;

                    if (person.First_Name != personvisitorVM.FirstName)
                    {
                        isChange = true;
                    }
                    if (person.Middle_Name != (personvisitorVM.MiddleName == null ? string.Empty : personvisitorVM.MiddleName))
                    {
                        isChange = true;
                    }
                    if (person.Last_Name != personvisitorVM.LastName)
                    {
                        isChange = true;
                    }
                    if (person.Gender != (personvisitorVM.Gender.Trim().ToLower() == "m" ? "M" : "F"))
                    {
                        isChange = true;
                    }
                    if (person.Birthdate != (personvisitorVM.BirthDate == null ? DateTime.Now : Convert.ToDateTime(personvisitorVM.BirthDate)))
                    {
                        isChange = true;
                    }
                    if (person.Contact_Number != (personvisitorVM.ContactNumber == null ? string.Empty : personvisitorVM.ContactNumber))
                    {
                        isChange = true;
                    }
                    if (person.Address != personvisitorVM.Address)
                    {
                        isChange = true;
                    }
                    if (person.Email_Address != (personvisitorVM.EmailAddress == null ? string.Empty : personvisitorVM.EmailAddress))
                    {
                        isChange = true;
                    }

                    if (isChange == false)
                    {
                        //response.Details.Add(new BatchUploadResponse.UploadDetails { Item = personvisitorVM, Message = "Row: " + i.ToString() + " ---> ID Number " + personvisitorVM.IDNumber + " already exist/no changes made.", isError = true });
                        importLog.Logging(_personvisitorBatch, fileName, "Person Visitor " + personvisitorVM.IDNumber + " already exist/no changes made.");
                        response.Failed++;
                        continue;
                    }
                    else
                    {
                        personinfo.First_Name = personvisitorVM.FirstName;
                        personinfo.Middle_Name = (personvisitorVM.MiddleName == null ? string.Empty : personvisitorVM.MiddleName);
                        personinfo.Last_Name = personvisitorVM.LastName;
                        personinfo.Gender = (personvisitorVM.Gender.Trim().ToLower() == "m" ? "M" : "F");
                        personinfo.Birthdate = (personvisitorVM.BirthDate == null ? DateTime.Now : Convert.ToDateTime(personvisitorVM.BirthDate));
                        personinfo.Contact_Number = (personvisitorVM.ContactNumber == null ? string.Empty : personvisitorVM.ContactNumber);
                        personinfo.Address = personvisitorVM.Address;
                        personinfo.Email_Address = (personvisitorVM.EmailAddress == null ? string.Empty : personvisitorVM.EmailAddress);

                        Boolean isSuccess = await _unitOfWork.PersonRepository.UpdatePersonVisitorWithBoolReturn(personinfo, user);

                        if (isSuccess)
                        {
                            //response.Details.Add(new BatchUploadResponse.UploadDetails { Item = personvisitorVM, Message = "Row " + i.ToString() + " ---> Person Visitor " + personvisitorVM.IDNumber + " has been successfully updated.", isError = false });
                            importLog.Logging(_personvisitorBatch, fileName, "Person Visitor " + personvisitorVM.IDNumber + " successfully updated.");
                            response.Success++;
                            continue;
                        }
                        else
                        {
                            //response.Details.Add(new BatchUploadResponse.UploadDetails { Item = personvisitorVM, Message = "Row " + i.ToString() + " ---> Unable to update Person Visitor " + personvisitorVM.IDNumber + ".", isError = false });
                            importLog.Logging(_personvisitorBatch, fileName, "Person Visitor " + personvisitorVM.IDNumber + " failed to update.");
                            response.Failed++;
                            continue;
                        }
                    }
                    */

                    personEntity personinfo = new personEntity();
                    personinfo.ID_Number = personvisitorVM.IDNumber;
                    personinfo.First_Name = personvisitorVM.FirstName;
                    personinfo.Middle_Name = (personvisitorVM.MiddleName == null ? string.Empty : personvisitorVM.MiddleName);
                    personinfo.Last_Name = personvisitorVM.LastName;
                    personinfo.Gender = (personvisitorVM.Gender.Trim().ToLower() == "m" ? "M" : "F");
                    personinfo.Birthdate = (personvisitorVM.BirthDate == null ? DateTime.Now : Convert.ToDateTime(personvisitorVM.BirthDate));
                    personinfo.Contact_Number = (personvisitorVM.ContactNumber == null ? string.Empty : personvisitorVM.ContactNumber);
                    personinfo.Address = personvisitorVM.Address;
                    personinfo.Email_Address = (personvisitorVM.EmailAddress == null ? string.Empty : personvisitorVM.EmailAddress);

                    Boolean isSuccess = await _unitOfWork.PersonRepository.UpdatePersonVisitorWithBoolReturn(personinfo, user);

                    await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Person_Visitor,
                        string.Format("Updated: ID Number: {0}, First Name: {1}, Last Name: {2}", personinfo.ID_Number, personinfo.First_Name, personinfo.Last_Name));

                    if (isSuccess)
                    {
                        //response.Details.Add(new BatchUploadResponse.UploadDetails { Item = personvisitorVM, Message = "Row " + i.ToString() + " ---> Person Visitor " + personvisitorVM.IDNumber + " has been successfully updated.", isError = false });
                        importLog.Logging(_personvisitorBatch, fileName, "Person Visitor " + personvisitorVM.IDNumber + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        //response.Details.Add(new BatchUploadResponse.UploadDetails { Item = personvisitorVM, Message = "Row " + i.ToString() + " ---> Unable to update Person Visitor " + personvisitorVM.IDNumber + ".", isError = false });
                        importLog.Logging(_personvisitorBatch, fileName, "Person Visitor " + personvisitorVM.IDNumber + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    person = new personEntity();
                    person.ID_Number = personvisitorVM.IDNumber;
                    person.First_Name = personvisitorVM.FirstName;
                    person.Middle_Name = (personvisitorVM.MiddleName == null ? string.Empty : personvisitorVM.MiddleName);
                    person.Last_Name = personvisitorVM.LastName;
                    person.Gender = (personvisitorVM.Gender.Trim().ToLower() == "m" ? "M" : "F");
                    person.Birthdate = (personvisitorVM.BirthDate == null ? DateTime.Now : Convert.ToDateTime(personvisitorVM.BirthDate));
                    person.Contact_Number = (personvisitorVM.ContactNumber == null ? string.Empty : personvisitorVM.ContactNumber);
                    person.Address = personvisitorVM.Address;
                    person.Email_Address = (personvisitorVM.EmailAddress == null ? string.Empty : personvisitorVM.EmailAddress);
                    person.Person_Type = "V";
                    await AddPersonVisitorFromBatchUpload(person, user);
                    //response.Details.Add(new BatchUploadResponse.UploadDetails { Item = personvisitorVM, Message = "Row " + i.ToString() + " ---> Person Visitor " + personvisitorVM.IDNumber + " has been successfully added.", isError = false });
                    importLog.Logging(_personvisitorBatch, fileName, "Person Visitor " + personvisitorVM.IDNumber + " successfully added.");
                    response.Success++;
                    continue;
                }
            }
            return response;
        }

	    public async Task AddPersonEmployeeFromBatchUpload(personEntity person, int user)
        {
            try
            {
                person.Added_By = user;
                person.Date_Time_Added = DateTime.Now;
                person.Updated_By = user;
                person.Last_Updated = DateTime.Now;
                person.IsActive = true;

                await _unitOfWork.PersonRepository.AddAsyn(person);
                await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Person_Employee,
                    string.Format("Added: ID Number: {0}, First Name: {1}, Last Name: {2}", person.ID_Number, person.First_Name, person.Last_Name));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BatchUploadResponse> PersonEmployeeBatchUpload(ICollection<personEmployeeBatchUploadVM> personemployees, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "Employee_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = personemployees.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var employeeVM in personemployees)
            {
                i++;

                if (employeeVM.IDNumber == null || employeeVM.IDNumber == string.Empty)
                { 
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> ID Number is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.IDNumber.Trim().Length > 20)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> ID Number accepts 20 characters only.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.FirstName == null || employeeVM.FirstName == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> First Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.FirstName.Trim().Length > 100)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> First Name accepts 100 characters only.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.MiddleName != string.Empty)
                {
                    if (employeeVM.MiddleName.Trim().Length > 50)
                    {
                        importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Middle Name accepts 50 characters only.");
                        response.Failed++;
                        continue;
                    }
                }

                if (employeeVM.LastName == null || employeeVM.LastName == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Last Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.LastName.Trim().Length > 100)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Last Name accepts 100 characters only.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.BirthDate == "" || employeeVM.BirthDate == null)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Birthdate is a required field.");
                    response.Failed++;
                    continue;
                }

                DateTime temp;
                if (!DateTime.TryParse(employeeVM.BirthDate, out temp))
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Birthdate is invalid.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.Gender == null || employeeVM.Gender == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Gender is a required field.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.Gender.Trim().ToLower() != "m" && employeeVM.Gender.Trim().ToLower() != "f")
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Gender is invalid.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.ContactNumber.Trim() == null || employeeVM.ContactNumber.Trim() == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!IsDigitsOnly(employeeVM.ContactNumber.Trim()))
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number is invalid.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.ContactNumber.Trim().Length > 11)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number accepts 11 digits only.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.TelephoneNumber == null || employeeVM.TelephoneNumber.Trim() == string.Empty)
                {
                    if (employeeVM.TelephoneNumber.Trim().Length > 20)
                    {
                        importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Telephone Number field accepts 20 characters only.");
                        response.Failed++;
                        continue;
                    }
                }

                if (employeeVM.EmailAddress.Trim() == null || employeeVM.EmailAddress.Trim() == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Email Address is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!new EmailAddressAttribute().IsValid(employeeVM.EmailAddress.Trim()))
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Email Address is invalid.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.EmailAddress.Trim().Length > 100)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Email Address accepts 100 characters only.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.Address == null || employeeVM.Address == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Address is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.Address.Trim().Length > 225)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Address accepts 225 characters only.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.CampusName == null || employeeVM.CampusName == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.EmployeeTypeName == null || employeeVM.EmployeeTypeName == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Employee Type Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.EmployeeSubTypeName == null || employeeVM.EmployeeSubTypeName == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Employee Sub Type Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.DepartmentName == null || employeeVM.DepartmentName == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Department Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.PositionName == null || employeeVM.PositionName == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Position Name is a required field.");
                    response.Failed++;
                    continue;
                }
                
                if (employeeVM.EmergencyFullname == null || employeeVM.EmergencyFullname == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Full Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.EmergencyFullname.Trim().Length > 225)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Full Name accepts 225 characters only.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.EmergencyAddress == null || employeeVM.EmergencyAddress == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Address is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.EmergencyAddress.Trim().Length > 225)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Address accepts 225 characters only.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.EmergencyMobileNo == null || employeeVM.EmergencyMobileNo == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Mobile Number is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.EmergencyMobileNo.Trim().Length != 11)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Mobile Number should be 11 digits.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.EmergencyRelationship == null || employeeVM.EmergencyRelationship == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Relationship is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.EmergencyRelationship.Trim().Length > 50)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Relationship accepts 50 characters only.");
                    response.Failed++;
                    continue;
                }
                
                if (employeeVM.SSS == null || employeeVM.SSS == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> SSS is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.SSS.Trim().Length > 50)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> SSS accepts 50 characters only.");
                    response.Failed++;
                    continue;
                }

                if (employeeVM.TIN == null || employeeVM.TIN == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> TIN is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.TIN.Trim().Length > 50)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> TIN accepts 50 characters only.");
                    response.Failed++;
                    continue;
                }
                
                if (employeeVM.PAGIBIG == null || employeeVM.PAGIBIG == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> PAGIBIG is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.PAGIBIG.Trim().Length > 50)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> PAGIBIG accepts 50 characters only.");
                    response.Failed++;
                    continue;
                }
                
                if (employeeVM.Philhealth == null || employeeVM.Philhealth == string.Empty)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Philhealth is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (employeeVM.Philhealth.Trim().Length > 50)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Philhealth accepts 50 characters only.");
                    response.Failed++;
                    continue;
                }

                var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == employeeVM.CampusName && x.IsActive == true);

                if (campus == null)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Campus " + employeeVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                var employeetype = await _unitOfWork.EmployeeTypeRepository.FindAsync(x => x.EmpTypeDesc == employeeVM.EmployeeTypeName && x.Campus_ID == campus.Campus_ID && x.IsActive == true);

                if (employeetype == null)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Employee Type " + employeeVM.EmployeeTypeName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                var employeesubtype = await _unitOfWork.EmployeeSubTypeRepository.FindAsync(x => x.EmpSubTypeDesc == employeeVM.EmployeeSubTypeName && x.EmpType_ID == employeetype.EmpType_ID && x.IsActive == true);

                if (employeesubtype == null)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Employee Sub Type " + employeeVM.EmployeeSubTypeName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                var department = await _unitOfWork.DepartmentRepository.FindAsync(x => x.Department_Name == employeeVM.DepartmentName && x.Campus_ID == campus.Campus_ID && x.IsActive == true);

                if (department == null)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Department " + employeeVM.DepartmentName + " under Campus " + employeeVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                var position = await _unitOfWork.PositionRepository.FindAsync(x => x.Position_Name == employeeVM.PositionName && x.Department_ID == department.Department_ID && x.IsActive == true);

                if (position == null)
                {
                    importLog.Logging(_personemployeeBatch, fileName, "Row: " + i.ToString() + " ---> Position " + employeeVM.PositionName + " under Campus " + employeeVM.CampusName + " ---> Department " + employeeVM.DepartmentName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                personEntity person = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == employeeVM.IDNumber);
                
                if (person != null)
                {
                    personEntity personinfo = new personEntity();
                    personinfo.ID_Number = employeeVM.IDNumber;
                    personinfo.First_Name = employeeVM.FirstName;
                    personinfo.Middle_Name = (employeeVM.MiddleName == null ? string.Empty : employeeVM.MiddleName);
                    personinfo.Last_Name = employeeVM.LastName;
                    personinfo.Gender = (employeeVM.Gender.Trim().ToLower() == "m" ? "M" : "F");
                    personinfo.Birthdate = (employeeVM.BirthDate == null ? DateTime.Now : Convert.ToDateTime(employeeVM.BirthDate));
                    personinfo.Contact_Number = (employeeVM.ContactNumber == null ? string.Empty : employeeVM.ContactNumber);
                    personinfo.Telephone_Number = (employeeVM.TelephoneNumber == null ? string.Empty : employeeVM.TelephoneNumber);
                    personinfo.Address = employeeVM.Address;
                    personinfo.Email_Address = (employeeVM.EmailAddress == null ? string.Empty : employeeVM.EmailAddress);
                    personinfo.Campus_ID = campus.Campus_ID;
                    personinfo.EmpType_ID = employeetype.EmpType_ID;
                    personinfo.EmpSubtype_ID = employeesubtype.EmpSubtype_ID;
                    personinfo.Department_ID = department.Department_ID;
                    personinfo.Position_ID = position.Position_ID;

                    emergencyContactEntity personemergency = new emergencyContactEntity();
                    personemergency.Full_Name = (employeeVM.EmergencyFullname == null ? string.Empty : employeeVM.EmergencyFullname);
                    personemergency.Contact_Number = (employeeVM.EmergencyMobileNo == null ? string.Empty : employeeVM.EmergencyMobileNo);
                    personemergency.Address = (employeeVM.EmergencyAddress == null ? string.Empty : employeeVM.EmergencyAddress);
                    personemergency.Relationship = (employeeVM.EmergencyRelationship == null ? string.Empty : employeeVM.EmergencyRelationship);

                    govIdsEntity persongovids = new govIdsEntity();
                    persongovids.SSS = (employeeVM.SSS == null ? string.Empty : employeeVM.SSS);
                    persongovids.PAG_IBIG = (employeeVM.PAGIBIG == null ? string.Empty : employeeVM.PAGIBIG);
                    persongovids.TIN = (employeeVM.TIN == null ? string.Empty : employeeVM.TIN);
                    persongovids.PhilHealth = (employeeVM.Philhealth == null ? string.Empty : employeeVM.Philhealth);

                    Boolean isSuccess = await _unitOfWork.PersonRepository.UpdateEmployeeWithBoolReturn(personinfo, personemergency, persongovids, user);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Person_Employee, "Update Employee By Batch", "UPDATE", true, "Success: " + personinfo.ID_Number, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess)
                    {
                        importLog.Logging(_personemployeeBatch, fileName, "Person Employee " + employeeVM.IDNumber + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_personemployeeBatch, fileName, "Person Employee " + employeeVM.IDNumber + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    person = new personEntity();
                    person.ID_Number = employeeVM.IDNumber;
                    person.First_Name = employeeVM.FirstName;
                    person.Middle_Name = (employeeVM.MiddleName == null ? string.Empty : employeeVM.MiddleName);
                    person.Last_Name = employeeVM.LastName;
                    person.Gender = (employeeVM.Gender.Trim().ToLower() == "m" ? "M" : "F");
                    person.Birthdate = (employeeVM.BirthDate == null ? DateTime.Now : Convert.ToDateTime(employeeVM.BirthDate));
                    person.Contact_Number = (employeeVM.ContactNumber == null ? string.Empty : employeeVM.ContactNumber);
                    person.Telephone_Number = (employeeVM.TelephoneNumber == null ? string.Empty : employeeVM.TelephoneNumber);
                    person.Address = employeeVM.Address;
                    person.Email_Address = (employeeVM.EmailAddress == null ? string.Empty : employeeVM.EmailAddress);
                    person.Campus_ID = campus.Campus_ID;
                    person.EmpType_ID = employeetype.EmpType_ID;
                    person.EmpSubtype_ID = employeesubtype.EmpSubtype_ID;
                    person.Department_ID = department.Department_ID;
                    person.Position_ID = position.Position_ID;

                    emergencyContactEntity personemergency = new emergencyContactEntity();
                    personemergency.Full_Name = (employeeVM.EmergencyFullname == null ? string.Empty : employeeVM.EmergencyFullname);
                    personemergency.Contact_Number = (employeeVM.EmergencyMobileNo == null ? string.Empty : employeeVM.EmergencyMobileNo);
                    personemergency.Address = (employeeVM.EmergencyAddress == null ? string.Empty : employeeVM.EmergencyAddress);
                    personemergency.Relationship = (employeeVM.EmergencyRelationship == null ? string.Empty : employeeVM.EmergencyRelationship);

                    govIdsEntity persongovids = new govIdsEntity();
                    persongovids.SSS = (employeeVM.SSS == null ? string.Empty : employeeVM.SSS);
                    persongovids.PAG_IBIG = (employeeVM.PAGIBIG == null ? string.Empty : employeeVM.PAGIBIG);
                    persongovids.TIN = (employeeVM.TIN == null ? string.Empty : employeeVM.TIN);
                    persongovids.PhilHealth = (employeeVM.Philhealth == null ? string.Empty : employeeVM.Philhealth);

                    Boolean isSuccess = await _unitOfWork.PersonRepository.AddEmployeeWithBoolReturn(person, personemergency, persongovids, user);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Person_Employee, "Insert Employee By Batch", "INSERT", true, "Success: " + person.ID_Number, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess)
                    {
                        importLog.Logging(_personemployeeBatch, fileName, "Person Employee " + employeeVM.IDNumber + " successfully added.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_personemployeeBatch, fileName, "Person Employee " + employeeVM.IDNumber + " failed to add.");
                        response.Failed++;
                        continue;
                    }
                }
            }
            return response;
        }

        public async Task AddPersonStudentFromBatchUpload(personEntity person, int user)
        {
            try
            {
                person.Added_By = user;
                person.Date_Time_Added = DateTime.Now;
                person.Updated_By = user;
                person.Last_Updated = DateTime.Now;
                person.IsActive = true;

                await _unitOfWork.PersonRepository.AddAsyn(person);
                await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Person_Student,
                    string.Format("Added: ID Number: {0}, First Name: {1}, Last Name: {2}", person.ID_Number, person.First_Name, person.Last_Name));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BatchUploadResponse> PersonStudentBatchUpload(ICollection<personStudentBatchUploadVM> personstudents, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "Student_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = personstudents.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var studentVM in personstudents)
            {
                i++;

                try
                { 
                    if (studentVM.IDNumber == null || studentVM.IDNumber == string.Empty)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> ID Number is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (studentVM.IDNumber.Trim().Length > 20)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> ID Number accepts 20 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (studentVM.FirstName == null || studentVM.FirstName == string.Empty)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> First Name is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (studentVM.FirstName.Trim().Length > 100)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> First Name accepts 100 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (studentVM.MiddleName != string.Empty)
                    {
                        if (studentVM.MiddleName.Trim().Length > 50)
                        {
                            importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Middle Name accepts 50 characters only.");
                            response.Failed++;
                            continue;
                        }
                    }
                    
                    if (studentVM.LastName == null || studentVM.LastName == string.Empty)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Last Name is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (studentVM.LastName.Trim().Length > 100)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Last Name accepts 100 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (studentVM.BirthDate == "" || studentVM.BirthDate == null)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Birthdate is a required field.");
                        response.Failed++;
                        continue;
                    }

                    DateTime temp;
                    if (!DateTime.TryParse(studentVM.BirthDate, out temp))
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Birthdate is invalid.");
                        response.Failed++;
                        continue;
                    }

                    if (studentVM.Gender == null || studentVM.Gender == string.Empty)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Gender is a required field.");
                        response.Failed++;
                        continue;
                    }

                    if (!(studentVM.Gender.Trim().ToLower() == "m" || studentVM.Gender.Trim().ToLower() == "male" || studentVM.Gender.Trim().ToLower() == "f" || studentVM.Gender.Trim().ToLower() == "female"))
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Gender is invalid.");
                        response.Failed++;
                        continue;
                    }

                    if (studentVM.Address == null || studentVM.Address == string.Empty)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Address is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (studentVM.Address.Trim().Length > 225)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Address accepts 225 characters only.");
                        response.Failed++;
                        continue;
                    }
                    
                    if (studentVM.ContactNumber != string.Empty)
                    {
                        if (!IsDigitsOnly(studentVM.ContactNumber))
                        {
                            importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number is invalid.");
                            response.Failed++;
                            continue;
                        }
                        else if (studentVM.ContactNumber.Trim().Length != 11)
                        {
                            importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number should be 11 digits.");
                            response.Failed++;
                            continue;
                        }
                    }

                    if (studentVM.TelephoneNumber == null || studentVM.TelephoneNumber.Trim() == string.Empty)
                    {
                        if (studentVM.TelephoneNumber.Trim().Length > 20)
                        {
                            importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Telephone Number field accepts 20 characters only.");
                            response.Failed++;
                            continue;
                        }
                    }

                    if (studentVM.EmailAddress == null || studentVM.EmailAddress != string.Empty)
                    {
                        if (!new EmailAddressAttribute().IsValid(studentVM.EmailAddress.Trim()))
                        {
                            importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Email Address is invalid.");
                            response.Failed++;
                            continue;
                        }
                        else if (studentVM.EmailAddress.Trim().Length > 100)
                        {
                            importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Email Address accepts 100 characters only.");
                            response.Failed++;
                            continue;
                        }
                    }

                    if (studentVM.CampusName == null || studentVM.CampusName == string.Empty)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name is a required field.");
                        response.Failed++;
                        continue;
                    }

                    if (studentVM.EducationalLevelName == null || studentVM.EducationalLevelName == string.Empty)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Education Level Name is a required field.");
                        response.Failed++;
                        continue;
                    }

                    if (studentVM.YearLevelName == null || studentVM.YearLevelName == string.Empty)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Year Level Name is a required field.");
                        response.Failed++;
                        continue;
                    }

                    if (studentVM.SectionName == null || studentVM.SectionName == string.Empty)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Section Name is a required field.");
                        response.Failed++;
                        continue;
                    }

                    if (studentVM.DateEnrolled == "" || studentVM.DateEnrolled == null)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Date Enrolled is a required field.");
                        response.Failed++;
                        continue;
                    }

                    if (!DateTime.TryParse(studentVM.DateEnrolled, out temp))
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Date Enrolled is invalid.");
                        response.Failed++;
                        continue;
                    }

                    if (studentVM.EmergencyFullname == null || studentVM.EmergencyFullname == string.Empty)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Full Name is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (studentVM.EmergencyFullname.Trim().Length > 225)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Full Name accepts 225 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (studentVM.EmergencyAddress == null || studentVM.EmergencyAddress == string.Empty)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Address is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (studentVM.EmergencyAddress.Trim().Length > 225)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Address accepts 225 characters only.");
                        response.Failed++;
                        continue;
                    }
                    
                    if (studentVM.EmergencyContactNo == null || studentVM.EmergencyContactNo == string.Empty)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Contact Number is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (!IsDigitsOnly(studentVM.EmergencyContactNo.Trim()))
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Contact Number is invalid.");
                        response.Failed++;
                        continue;
                    }
                    else if (studentVM.EmergencyContactNo.Trim().Length != 11)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Contact Number accepts 11 digits only.");
                        response.Failed++;
                        continue;
                    }

                    if (studentVM.EmergencyRelationship == null || studentVM.EmergencyRelationship == string.Empty)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Relationship is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (studentVM.EmergencyRelationship.Trim().Length > 50)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Relationship accepts 50 characters only.");
                        response.Failed++;
                        continue;
                    }

                    var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == studentVM.CampusName && x.IsActive == true);
                    if (campus == null)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name " + studentVM.CampusName + " does not exist.");
                        response.Failed++;
                        continue;
                    }

                    var educlevel = await _unitOfWork.EducationLevelRepository.FindAsync(x => x.Level_Name == studentVM.EducationalLevelName && x.Campus_ID == campus.Campus_ID && x.IsActive == true);

                    var college = new collegeEntity();
                    var course = new courseEntity();

                    if (educlevel == null)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level " + studentVM.EducationalLevelName + " under Campus " + studentVM.CampusName + " does not exist.");
                        response.Failed++;
                        continue;
                    }
                    else if (educlevel.hasCourse)
                    {
                        if (studentVM.IsCollegeTemplate)
                        {
                            college = await _unitOfWork.CollegeRepository.FindAsync(x => x.Level_ID == educlevel.Level_ID && x.College_Name == studentVM.CollegeName && x.IsActive == true);
                            course = await _unitOfWork.CourseRepository.FindAsync(x => x.Course_Name == studentVM.CourseName && x.College_ID == college.College_ID && x.IsActive == true);

                            if (college == null)
                            {
                                importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> College Name " + studentVM.CollegeName + " under Campus " + studentVM.CampusName + " --> Educational Level " + studentVM.EducationalLevelName + " does not exist.");
                                response.Failed++;
                                continue;
                            }

                            if (course == null)
                            {
                                importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Course Name " + studentVM.CourseName + " under Campus " + studentVM.CampusName + " --> Educational Level " + studentVM.EducationalLevelName + " --> College Name " + studentVM.CollegeName + " does not exist.");
                                response.Failed++;
                                continue;
                            }
                        }
                        else
                        {
                            importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level " + studentVM.EducationalLevelName + " allows only Non-College Students. Please use the valid template for College Students.");
                            response.Failed++;
                            continue;
                        }
                    }

                    var yearlevel = await _unitOfWork.YearSectionRepository.FindAsync(x => x.YearSec_Name == studentVM.YearLevelName && x.Level_ID == educlevel.Level_ID && x.IsActive == true);

                    if (yearlevel == null)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Year Level Name " + studentVM.YearLevelName + " under Campus " + studentVM.CampusName + " --> Educational Level " + studentVM.EducationalLevelName + " does not exist.");
                        response.Failed++;
                        continue;
                    }

                    var section = await _unitOfWork.StudentSectionRepository.FindAsync(x => x.Description == studentVM.SectionName && x.YearSec_ID == yearlevel.YearSec_ID && x.IsActive == true);

                    if (section == null)
                    {
                        importLog.Logging(_personstudentBatch, fileName, "Row: " + i.ToString() + " ---> Section Name " + studentVM.SectionName + " under Campus " + studentVM.CampusName + " --> Educational Level " + studentVM.EducationalLevelName + " --> Year Level " + studentVM.YearLevelName + " does not exist.");
                        response.Failed++;
                        continue;
                    }

                    string gender = studentVM.Gender.Trim().ToLower();
                    if (gender == "male" || gender == "m")
                        gender = "M";
                    else
                        gender = "F";

                    personEntity person = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == studentVM.IDNumber);
                    
                    if (person != null)
                    {

                        personEntity personinfo = new personEntity();
                        personinfo.ID_Number = studentVM.IDNumber;
                        personinfo.First_Name = studentVM.FirstName;
                        personinfo.Middle_Name = (studentVM.MiddleName == null ? string.Empty : studentVM.MiddleName);
                        personinfo.Last_Name = studentVM.LastName;
                        personinfo.Gender = gender;
                        personinfo.Birthdate = (studentVM.BirthDate == null ? DateTime.Now : Convert.ToDateTime(studentVM.BirthDate));
                        personinfo.Contact_Number = (studentVM.ContactNumber == null ? string.Empty : studentVM.ContactNumber);
                        personinfo.Telephone_Number = (studentVM.TelephoneNumber == null ? string.Empty : studentVM.TelephoneNumber);
                        personinfo.Address = studentVM.Address;
                        personinfo.Email_Address = (studentVM.EmailAddress == null ? string.Empty : studentVM.EmailAddress);
                        personinfo.DateEnrolled = (studentVM.DateEnrolled == null ? DateTime.Now : Convert.ToDateTime(studentVM.DateEnrolled));
                        personinfo.Campus_ID = campus.Campus_ID;
                        personinfo.Educ_Level_ID = educlevel.Level_ID;
                        personinfo.Year_Section_ID = yearlevel.YearSec_ID;
                        personinfo.StudSec_ID = section.StudSec_ID;
                        personinfo.College_ID = educlevel.hasCourse == true ? college.College_ID : 0;
                        personinfo.Course_ID = educlevel.hasCourse == true ? course.Course_ID : 0;

                        personinfo.IsDropOut = person.IsDropOut;
                        personinfo.DropOutCode = person.DropOutCode;
                        personinfo.DropOutOtherRemark = person.DropOutOtherRemark;
                        personinfo.IsTransferredIn = person.IsTransferredIn;
                        personinfo.TransferredInSchoolName = person.TransferredInSchoolName;
                        personinfo.IsTransferred = person.IsTransferred;
                        personinfo.TransferredSchoolName = person.TransferredSchoolName;

                        emergencyContactEntity personemergency = new emergencyContactEntity();
                        personemergency.Full_Name = (studentVM.EmergencyFullname == null ? string.Empty : studentVM.EmergencyFullname);
                        personemergency.Contact_Number = (studentVM.EmergencyContactNo == null ? string.Empty : studentVM.EmergencyContactNo);
                        personemergency.Address = (studentVM.EmergencyAddress == null ? string.Empty : studentVM.EmergencyAddress);
                        personemergency.Relationship = (studentVM.EmergencyRelationship == null ? string.Empty : studentVM.EmergencyRelationship);

                        Boolean isSuccess = await _unitOfWork.PersonRepository.UpdateStudentWithBoolReturn(personinfo, personemergency, user);
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Person_Student, "Update Student By Batch", "UPDATE", true, "Success: " + personinfo.ID_Number, DateTime.UtcNow.ToLocalTime()));

                        if (isSuccess)
                        {
                            importLog.Logging(_personstudentBatch, fileName, "Person Student " + studentVM.IDNumber + " successfully updated.");
                            response.Success++;
                            continue;
                        }
                        else
                        {
                            importLog.Logging(_personstudentBatch, fileName, "Person Student " + studentVM.IDNumber + " failed to update.");
                            response.Failed++;
                            continue;
                        }
                    }
                    else 
                    {
                        person = new personEntity();
                        person.ID_Number = studentVM.IDNumber;
                        person.First_Name = studentVM.FirstName;
                        person.Middle_Name = (studentVM.MiddleName == null ? string.Empty : studentVM.MiddleName);
                        person.Last_Name = studentVM.LastName;
                        person.Gender = gender;
                        person.Birthdate = (studentVM.BirthDate == null ? DateTime.Now : Convert.ToDateTime(studentVM.BirthDate));
                        person.Contact_Number = (studentVM.ContactNumber == null ? string.Empty : studentVM.ContactNumber);
                        person.Telephone_Number = (studentVM.TelephoneNumber == null ? string.Empty : studentVM.TelephoneNumber);
                        person.Address = studentVM.Address;
                        person.Email_Address = (studentVM.EmailAddress == null ? string.Empty : studentVM.EmailAddress);
                        person.DateEnrolled = (studentVM.DateEnrolled == null ? DateTime.Now : Convert.ToDateTime(studentVM.DateEnrolled));
                        person.Campus_ID = campus.Campus_ID;
                        person.Educ_Level_ID = educlevel.Level_ID;
                        person.Year_Section_ID = yearlevel.YearSec_ID;
                        person.StudSec_ID = section.StudSec_ID;
                        person.College_ID = educlevel.hasCourse == true ? college.College_ID : 0;
                        person.Course_ID = educlevel.hasCourse == true ? course.Course_ID : 0;

                        person.IsDropOut = false;
                        person.DropOutCode = string.Empty;
                        person.DropOutOtherRemark = string.Empty;
                        person.IsTransferredIn = false;
                        person.TransferredInSchoolName = string.Empty;
                        person.IsTransferred = false;
                        person.TransferredSchoolName = string.Empty;

                        emergencyContactEntity personemergency = new emergencyContactEntity();
                        personemergency.Full_Name = studentVM.EmergencyFullname;
                        personemergency.Contact_Number = studentVM.EmergencyContactNo;
                        personemergency.Address = studentVM.EmergencyAddress;
                        personemergency.Relationship = studentVM.EmergencyRelationship;

                        Boolean isSuccess = await _unitOfWork.PersonRepository.AddStudentWithBoolReturn(person, personemergency, user);
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Person_Student, "Insert Student By Batch", "INSERT", true, "Success: " + person.ID_Number, DateTime.UtcNow.ToLocalTime()));

                        if (isSuccess)
                        {
                            importLog.Logging(_personstudentBatch, fileName, "Person Student " + studentVM.IDNumber + " successfully added.");
                            response.Success++;
                            continue;
                        }
                        else
                        {
                            importLog.Logging(_personstudentBatch, fileName, "Person Student " + studentVM.IDNumber + " failed to add.");
                            response.Failed++;
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    importLog.Logging(_personstudentBatch, fileName, ex.Message.ToString());
                }
            }
            return response;
        }

        public async Task<BatchUploadResponse> PersonFetcherBatchUpload(ICollection<personFetcherBatchUploadVM> personfetchers, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "Fetcher_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = personfetchers.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var fetcherVM in personfetchers)
            {
                i++;

                try
                {
                    if (fetcherVM.IDNumber == null || fetcherVM.IDNumber == string.Empty)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> ID Number field is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (fetcherVM.IDNumber.Trim().Length > 20)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> ID Number field accepts 20 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (fetcherVM.FirstName == null || fetcherVM.FirstName == string.Empty)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> First Name field is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (fetcherVM.FirstName.Trim().Length > 100)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> First Name field accepts 100 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (fetcherVM.MiddleName != string.Empty)
                    {
                        if (fetcherVM.MiddleName.Trim().Length > 50)
                        {
                            importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Middle Name field accepts 50 characters only.");
                            response.Failed++;
                            continue;
                        }
                    }

                    if (fetcherVM.LastName == null || fetcherVM.LastName == string.Empty)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Last Name field is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (fetcherVM.LastName.Trim().Length > 100)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Last Name field accepts 100 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (fetcherVM.BirthDate == "" || fetcherVM.BirthDate == null)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Birthdate field is a required field.");
                        response.Failed++;
                        continue;
                    }

                    DateTime temp;
                    if (!DateTime.TryParse(fetcherVM.BirthDate, out temp))
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Birthdate field is invalid.");
                        response.Failed++;
                        continue;
                    }

                    if (fetcherVM.Gender == null || fetcherVM.Gender == string.Empty)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Gender field is a required field.");
                        response.Failed++;
                        continue;
                    }

                    if (fetcherVM.Gender.Trim().ToLower() != "m" && fetcherVM.Gender.Trim().ToLower() != "f")
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Gender field is invalid.");
                        response.Failed++;
                        continue;
                    }
                    
                    if (fetcherVM.ContactNumber == null || fetcherVM.ContactNumber.Trim() == string.Empty)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (!IsDigitsOnly(fetcherVM.ContactNumber.Trim()))
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number is invalid.");
                        response.Failed++;
                        continue;
                    }
                    else if (fetcherVM.ContactNumber.Trim().Length != 11)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number should be 11 digits.");
                        response.Failed++;
                        continue;
                    }

                    if (fetcherVM.EmailAddress == null || fetcherVM.EmailAddress != string.Empty)
                    {
                        if (!new EmailAddressAttribute().IsValid(fetcherVM.EmailAddress.Trim()))
                        {
                            importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Email Address is invalid.");
                            response.Failed++;
                            continue;
                        }
                        else if (fetcherVM.EmailAddress.Trim().Length > 100)
                        {
                            importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Email Address field accepts 100 characters only.");
                            response.Failed++;
                            continue;
                        }
                    }

                    if (fetcherVM.Address == null || fetcherVM.Address == string.Empty)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Address field is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (fetcherVM.Address.Trim().Length > 225)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Address field accepts 225 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (fetcherVM.FetcherRelationship != string.Empty)
                    {
                        if (fetcherVM.FetcherRelationship.Trim().Length > 225)
                        {
                            importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Relationship field accepts 50 characters only.");
                            response.Failed++;
                            continue;
                        }
                    }

                    if (fetcherVM.CampusName == null || fetcherVM.CampusName == string.Empty)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name field is a required field.");
                        response.Failed++;
                        continue;
                    }

                    var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == fetcherVM.CampusName && x.IsActive == true);

                    if (campus == null)
                    {
                        importLog.Logging(_personfetcherBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name " + fetcherVM.CampusName + " does not exist.");
                        response.Failed++;
                        continue;
                    }

                    personEntity person = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == fetcherVM.IDNumber);

                    if (person != null)
                    {
                        personEntity personinfo = new personEntity();
                        personinfo.ID_Number = fetcherVM.IDNumber;
                        personinfo.Person_Type = "F";
                        personinfo.First_Name = fetcherVM.FirstName;
                        personinfo.Middle_Name = (fetcherVM.MiddleName == null ? string.Empty : fetcherVM.MiddleName);
                        personinfo.Last_Name = fetcherVM.LastName;
                        personinfo.Gender = (fetcherVM.Gender.Trim().ToLower() == "m" ? "M" : "F");
                        personinfo.Birthdate = (fetcherVM.BirthDate == null ? DateTime.Now : Convert.ToDateTime(fetcherVM.BirthDate));
                        personinfo.Contact_Number = (fetcherVM.ContactNumber == null ? string.Empty : fetcherVM.ContactNumber);
                        personinfo.Address = fetcherVM.Address;
                        personinfo.Email_Address = (fetcherVM.EmailAddress == null ? string.Empty : fetcherVM.EmailAddress);
                        personinfo.Campus_ID = campus.Campus_ID;
                        personinfo.Fetcher_Relationship = fetcherVM.FetcherRelationship == null ? string.Empty : fetcherVM.FetcherRelationship;

                        Boolean isSuccess = await _unitOfWork.PersonRepository.UpdateFetcherWithBoolReturn(personinfo, user);
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Person_Fetcher, "Update Fetcher By Batch", "UPDATE", true, "Success: " + personinfo.ID_Number, DateTime.UtcNow.ToLocalTime()));

                        if (isSuccess)
                        {
                            importLog.Logging(_personfetcherBatch, fileName, "Person Fetcher " + fetcherVM.IDNumber + " successfully updated.");
                            response.Success++;
                            continue;
                        }
                        else
                        {
                            importLog.Logging(_personfetcherBatch, fileName, "Person Fetcher " + fetcherVM.IDNumber + " failed to update.");
                            response.Failed++;
                            continue;
                        }
                    }
                    else
                    {
                        person = new personEntity();
                        person.ID_Number = fetcherVM.IDNumber;
                        person.First_Name = fetcherVM.FirstName;
                        person.Middle_Name = (fetcherVM.MiddleName == null ? string.Empty : fetcherVM.MiddleName);
                        person.Last_Name = fetcherVM.LastName;
                        person.Gender = (fetcherVM.Gender.Trim().ToLower() == "m" ? "M" : "F");
                        person.Birthdate = (fetcherVM.BirthDate == null ? DateTime.Now : Convert.ToDateTime(fetcherVM.BirthDate));
                        person.Contact_Number = (fetcherVM.ContactNumber == null ? string.Empty : fetcherVM.ContactNumber);
                        person.Address = fetcherVM.Address;
                        person.Email_Address = (fetcherVM.EmailAddress == null ? string.Empty : fetcherVM.EmailAddress);
                        person.Campus_ID = campus.Campus_ID;

                        Boolean isSuccess = await _unitOfWork.PersonRepository.AddFetcherWithBoolReturn(person, user);
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Person_Fetcher, "Insert Fetcher By Batch", "INSERT", true, "Success: " + person.ID_Number, DateTime.UtcNow.ToLocalTime()));

                        if (isSuccess)
                        {
                            importLog.Logging(_personfetcherBatch, fileName, "Person Fetcher " + fetcherVM.IDNumber + " successfully added.");
                            response.Success++;
                            continue;
                        }
                        else
                        {
                            importLog.Logging(_personfetcherBatch, fileName, "Person Fetcher " + fetcherVM.IDNumber + " failed to add.");
                            response.Failed++;
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    importLog.Logging(_personfetcherBatch, fileName, ex.Message.ToString());
                }
            }
            return response;
        }
        
        public async Task<BatchUploadResponse> PersonOtherAccessBatchUpload(ICollection<personOtherAccessBatchUploadVM> personotheraccess, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "Other_Access_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = personotheraccess.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var otheraccessVM in personotheraccess)
            {
                i++;

                try
                {
                    if (otheraccessVM.IDNumber == null || otheraccessVM.IDNumber == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> ID Number field is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (otheraccessVM.IDNumber.Trim().Length > 20)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> ID Number field accepts 20 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.FirstName == null || otheraccessVM.FirstName == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> First Name field is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (otheraccessVM.FirstName.Trim().Length > 100)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> First Name field accepts 100 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.MiddleName != string.Empty)
                    {
                        if (otheraccessVM.MiddleName.Trim().Length > 50)
                        {
                            importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Middle Name field accepts 50 characters only.");
                            response.Failed++;
                            continue;
                        }
                    }

                    if (otheraccessVM.LastName == null || otheraccessVM.LastName == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Last Name field is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (otheraccessVM.LastName.Trim().Length > 100)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Last Name field accepts 100 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.BirthDate == "" || otheraccessVM.BirthDate == null)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Birthdate field is a required field.");
                        response.Failed++;
                        continue;
                    }

                    DateTime temp;
                    if (!DateTime.TryParse(otheraccessVM.BirthDate, out temp))
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Birthdate field is invalid.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.Gender == null || otheraccessVM.Gender == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Gender field is a required field.");
                        response.Failed++;
                        continue;
                    }
                    
                    if (otheraccessVM.Gender.Trim().ToLower() != "m" && otheraccessVM.Gender.Trim().ToLower() != "f")
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Gender field is invalid.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.ContactNumber == null || otheraccessVM.ContactNumber.Trim() == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (!IsDigitsOnly(otheraccessVM.ContactNumber.Trim()))
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number field accepts numbers only.");
                        response.Failed++;
                        continue;
                    }
                    else if (otheraccessVM.ContactNumber.Trim().Length != 11)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Contact Number should be 11 digits.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.TelephoneNumber == null || otheraccessVM.TelephoneNumber.Trim() == string.Empty)
                    {
                        if (otheraccessVM.TelephoneNumber.Trim().Length > 20)
                        {
                            importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Telephone Number field accepts 20 characters only.");
                            response.Failed++;
                            continue;
                        }
                    }

                    if (otheraccessVM.EmailAddress == null || otheraccessVM.EmailAddress != string.Empty)
                    {
                        if (!new EmailAddressAttribute().IsValid(otheraccessVM.EmailAddress.Trim()))
                        {
                            importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Email Address is invalid.");
                            response.Failed++;
                            continue;
                        }
                        else if (otheraccessVM.EmailAddress.Trim().Length > 100)
                        {
                            importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Email Address field accepts 100 characters only.");
                            response.Failed++;
                            continue;
                        }
                    }

                    if (otheraccessVM.Address == null || otheraccessVM.Address == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Address field is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (otheraccessVM.Address.Trim().Length > 225)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Address field accepts 225 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.CampusName == null || otheraccessVM.CampusName == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name is a required field.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.DepartmentName == null || otheraccessVM.DepartmentName == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Department Name is a required field.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.PositionName == null || otheraccessVM.PositionName == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Position Name is a required field.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.OfficeName == null || otheraccessVM.OfficeName == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Office Name is a required field.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.EmergencyFullname == null || otheraccessVM.EmergencyFullname == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Fullname is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (otheraccessVM.EmergencyFullname.Trim().Length > 225)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Full Name accepts 225 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.EmergencyAddress == null || otheraccessVM.EmergencyAddress == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Address is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (otheraccessVM.EmergencyAddress.Trim().Length > 225)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Address accepts 225 characters only.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.EmergencyMobileNo == null || otheraccessVM.EmergencyMobileNo == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Mobile Number is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (!IsDigitsOnly(otheraccessVM.EmergencyMobileNo.Trim()))
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Mobile Number accepts numbers only.");
                        response.Failed++;
                        continue;
                    }
                    else if (otheraccessVM.EmergencyMobileNo.Trim().Length != 11)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Mobile Number should be 11 digits.");
                        response.Failed++;
                        continue;
                    }

                    if (otheraccessVM.EmergencyRelationship == null || otheraccessVM.EmergencyRelationship == string.Empty)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Relationship is a required field.");
                        response.Failed++;
                        continue;
                    }
                    else if (otheraccessVM.EmergencyRelationship.Trim().Length > 50)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Emergency Relationship accepts 50 characters only.");
                        response.Failed++;
                        continue;
                    }

                    var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == otheraccessVM.CampusName && x.IsActive == true); 

                    if (campus == null)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Campus " + otheraccessVM.CampusName + " does not exist.");
                        response.Failed++;
                        continue;
                    }

                    var department = await _unitOfWork.DepartmentRepository.FindAsync(x => x.Department_Name == otheraccessVM.DepartmentName && x.Campus_ID == campus.Campus_ID && x.IsActive == true);

                    if (department == null)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Department " + otheraccessVM.DepartmentName + " under Campus " + otheraccessVM.CampusName + " does not exist.");
                        response.Failed++;
                        continue;
                    }

                    var position = await _unitOfWork.PositionRepository.FindAsync(x => x.Position_Name == otheraccessVM.PositionName && x.Department_ID == department.Department_ID && x.IsActive == true);

                    if (position == null)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Position " + otheraccessVM.PositionName + " under Campus " + otheraccessVM.CampusName + " ---> Department " + otheraccessVM.DepartmentName + " does not exist.");
                        response.Failed++;
                        continue;
                    }

                    var office = await _unitOfWork.OfficeRepository.FindAsync(x => x.Office_Name == otheraccessVM.OfficeName && x.Campus_ID == campus.Campus_ID && x.IsActive == true);

                    if (office == null)
                    {
                        importLog.Logging(_personotheraccessBatch, fileName, "Row: " + i.ToString() + " ---> Office " + otheraccessVM.OfficeName + " under Campus " + otheraccessVM.CampusName + " ---> Department " + otheraccessVM.DepartmentName + " does not exist.");
                        response.Failed++;
                        continue;
                    }

                    personEntity person = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == otheraccessVM.IDNumber);

                    if (person != null)
                    {
                        personEntity personinfo = new personEntity();
                        personinfo.ID_Number = otheraccessVM.IDNumber;
                        personinfo.First_Name = otheraccessVM.FirstName;
                        personinfo.Middle_Name = (otheraccessVM.MiddleName == null ? string.Empty : otheraccessVM.MiddleName);
                        personinfo.Last_Name = otheraccessVM.LastName;
                        personinfo.Gender = (otheraccessVM.Gender.Trim().ToLower() == "m" ? "M" : "F");
                        personinfo.Birthdate = (otheraccessVM.BirthDate == null ? DateTime.Now : Convert.ToDateTime(otheraccessVM.BirthDate));
                        personinfo.Contact_Number = (otheraccessVM.ContactNumber == null ? string.Empty : otheraccessVM.ContactNumber);
                        personinfo.Telephone_Number = (otheraccessVM.TelephoneNumber == null ? string.Empty : otheraccessVM.TelephoneNumber);
                        personinfo.Address = otheraccessVM.Address;
                        personinfo.Email_Address = (otheraccessVM.EmailAddress == null ? string.Empty : otheraccessVM.EmailAddress);

                        personinfo.Campus_ID = campus.Campus_ID;
                        personinfo.Position_ID = position.Position_ID;
                        personinfo.Department_ID = department.Department_ID;
                        personinfo.Office_ID = office.Office_ID;

                        emergencyContactEntity personemergency = new emergencyContactEntity();
                        personemergency.Full_Name = (otheraccessVM.EmergencyFullname == null ? string.Empty : otheraccessVM.EmergencyFullname);
                        personemergency.Contact_Number = (otheraccessVM.EmergencyMobileNo == null ? string.Empty : otheraccessVM.EmergencyMobileNo);
                        personemergency.Address = (otheraccessVM.EmergencyAddress == null ? string.Empty : otheraccessVM.EmergencyAddress);
                        personemergency.Relationship = (otheraccessVM.EmergencyRelationship == null ? string.Empty : otheraccessVM.EmergencyRelationship);

                        Boolean isSuccess = await _unitOfWork.PersonRepository.UpdateOtherAccessWithBoolReturn(personinfo, personemergency, user);
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Person_OtherAccess, "Update Other Access By Batch", "UPDATE", true, "Success: " + personinfo.ID_Number, DateTime.UtcNow.ToLocalTime()));

                        if (isSuccess)
                        {
                            importLog.Logging(_personotheraccessBatch, fileName, "Person Other Access " + otheraccessVM.IDNumber + " successfully updated.");
                            response.Success++;
                            continue;
                        }
                        else
                        {
                            importLog.Logging(_personotheraccessBatch, fileName, "Person Other Access " + otheraccessVM.IDNumber + " failed to update.");
                            response.Failed++;
                            continue;
                        }
                    }
                    else
                    {
                        person = new personEntity();
                        person.ID_Number = otheraccessVM.IDNumber;
                        person.First_Name = otheraccessVM.FirstName;
                        person.Middle_Name = (otheraccessVM.MiddleName == null ? string.Empty : otheraccessVM.MiddleName);
                        person.Last_Name = otheraccessVM.LastName;
                        person.Gender = (otheraccessVM.Gender.Trim().ToLower() == "m" ? "M" : "F");
                        person.Birthdate = (otheraccessVM.BirthDate == null ? DateTime.Now : Convert.ToDateTime(otheraccessVM.BirthDate));
                        person.Contact_Number = (otheraccessVM.ContactNumber == null ? string.Empty : otheraccessVM.ContactNumber);
                        person.Telephone_Number = (otheraccessVM.TelephoneNumber == null ? string.Empty : otheraccessVM.TelephoneNumber);
                        person.Address = otheraccessVM.Address;
                        person.Email_Address = (otheraccessVM.EmailAddress == null ? string.Empty : otheraccessVM.EmailAddress);

                        person.Campus_ID = campus.Campus_ID;
                        person.Position_ID = position.Position_ID;
                        person.Department_ID = department.Department_ID;
                        person.Office_ID = office.Office_ID;

                        emergencyContactEntity personemergency = new emergencyContactEntity();
                        personemergency.Full_Name = (otheraccessVM.EmergencyFullname == null ? string.Empty : otheraccessVM.EmergencyFullname);
                        personemergency.Contact_Number = (otheraccessVM.EmergencyMobileNo == null ? string.Empty : otheraccessVM.EmergencyMobileNo);
                        personemergency.Address = (otheraccessVM.EmergencyAddress == null ? string.Empty : otheraccessVM.EmergencyAddress);
                        personemergency.Relationship = (otheraccessVM.EmergencyRelationship == null ? string.Empty : otheraccessVM.EmergencyRelationship);

                        Boolean isSuccess = await _unitOfWork.PersonRepository.AddOtherAccessWithBoolReturn(person, personemergency, user);
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Person_OtherAccess, "Insert Other Access By Batch", "INSERT", true, "Success: " + person.ID_Number, DateTime.UtcNow.ToLocalTime()));

                        if (isSuccess)
                        {
                            importLog.Logging(_personotheraccessBatch, fileName, "Person Other Access " + otheraccessVM.IDNumber + " successfully added.");
                            response.Success++;
                            continue;
                        }
                        else
                        {
                            importLog.Logging(_personotheraccessBatch, fileName, "Person Other Access " + otheraccessVM.IDNumber + " failed to add.");
                            response.Failed++;
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return response;
        }

        public async Task<studentPagedResultVM> GetAllStudents(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.PersonRepository.GetAllStudents(pageNo, pageSize, keyword);
        }

        public async Task<employeePagedResult> GetAllEmployees(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.PersonRepository.GetAllEmployees(pageNo, pageSize, keyword);
        }

        public async Task<visitorPagedResult> GetAllVisitors(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.PersonRepository.GetAllVisitors(pageNo, pageSize, keyword);
        }

        public async Task<fetcherPagedResult> GetAllFetchers(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.PersonRepository.GetAllFetchers(pageNo, pageSize, keyword);
        }

        public async Task<otherAccessPagedResultVM> GetAllOtherAccesses(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.PersonRepository.GetAllOtherAccesses(pageNo, pageSize, keyword);
        }

        public async Task<studentPagedResult> ExportAllStudents(string keyword, bool isCollege)
        {
            return await _unitOfWork.PersonRepository.ExportAllStudents(keyword, isCollege);
        }

        public async Task<fetcherPagedResult> ExportAllFetchers(string keyword)
        {
            return await _unitOfWork.PersonRepository.ExportAllFetchers(keyword);
        }

        public async Task<otherAccessPagedResult> ExportAllOtherAccess(string keyword)
        {
            return await _unitOfWork.PersonRepository.ExportAllOtherAccess(keyword);
        }

        public async Task<visitorPagedResult> ExportAllVisitors(string keyword)
        {
            return await _unitOfWork.PersonRepository.ExportAllVisitors(keyword);
        }

        public async Task<employeePagedResult> ExportAllEmployees(string keyword)
        {
            return await _unitOfWork.PersonRepository.ExportAllEmployees(keyword);
        }

        public async Task<personPagedResultVM> GetAllWithOutCardList(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.PersonRepository.GetAllWithOutCardList(pageNo, pageSize, keyword);
        }

        public async Task<personPagedResult> GetAllWithCardList(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.PersonRepository.GetAllWithCardList(pageNo, pageSize, keyword);
        }

        public async Task<ICollection<yearSectionEntity>> CheckboxChecker(int value1, int value2)
        {
            return await _unitOfWork.PersonRepository.CheckboxChecker(value1, value2);
        }

        public async Task<ICollection<studentSectionEntity>> CheckboxCheckerOne(int value1, int value2)
        {
            return await _unitOfWork.PersonRepository.CheckboxCheckerOne(value1, value2);
        }

        public async Task<personEntity> GetFetcherById(int id)
        {
            try
            {
                var result = await _unitOfWork.PersonRepository.GetFetcherById(id);
                result.Contact_Number = result.Contact_Number != null ? result.Contact_Number != string.Empty ? result.Contact_Number.Replace("-", "") : result.Contact_Number : result.Contact_Number;
                return result;
            }
            catch (Exception ex)
            {
                errorLogging.WriteError(ex);
                throw ex;
            }
        }

        public async Task<ResultModel> AddFetcher(personEntity fetcher, int user)
        {
            try
            {
                fetcher.Person_Type = "F";
                fetcher.Added_By = user;
                fetcher.Updated_By = user;
                fetcher.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                fetcher.Last_Updated = DateTime.UtcNow.ToLocalTime();
                fetcher.IsActive = true;
                fetcher.ToDisplay = true;
                fetcher.Contact_Number = fetcher.Contact_Number.Substring(0, 4) + "-" + fetcher.Contact_Number.Substring(4, 3) + "-" + fetcher.Contact_Number.Substring(7, 4);

                var personcheck = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == fetcher.ID_Number);

                if (personcheck != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(fetcher.Added_By, (int)Form.Person_Fetcher, "Add Fetcher", "INSERT", false, "Failed: " + fetcher.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.resultMessage = PERSON_FETCHER_EXIST;
                    result.isSuccess = false;
                    return result;
                }
                else
                {
                    var contactNumberCheck = await _unitOfWork.PersonRepository.FindAsync(x => x.Contact_Number == fetcher.Contact_Number && x.IsActive == true);
                    
                    if ( contactNumberCheck != null )
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(fetcher.Added_By, (int)Form.Person_Fetcher, "Add Fetcher", "INSERT", false, "Failed: " + fetcher.ID_Number, DateTime.UtcNow.ToLocalTime()));
                        result = new ResultModel();
                        result.resultCode = "409";
                        result.resultMessage = PERSON_FETCHER_NUMBER_EXIST;
                        result.isSuccess = false;
                        return result;
                    }
                }

                var data = await _unitOfWork.PersonRepository.AddFetcher(fetcher);

                if (data.resultCode == "500")
                {
                    return data;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(fetcher.Added_By, (int)Form.Person_Fetcher, "Add Fetcher", "INSERT", true, "Success: " + fetcher.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Fetcher " + Constants.SuccessMessageAdd;
                result.isSuccess = true;
                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;
                return result;
            }
        }

        public async Task<ResultModel> UpdateFetcher(personEntity fetcher, int user)
        {
            try
            {
                fetcher.Updated_By = user;
                fetcher.Contact_Number = fetcher.Contact_Number.Substring(0, 4) + "-" + fetcher.Contact_Number.Substring(4, 3) + "-" + fetcher.Contact_Number.Substring(7, 4);

                var data = await _unitOfWork.PersonRepository.UpdateFetcher(fetcher);

                if (data == null)
                {
                    return data;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(fetcher.Updated_By, (int)Form.Person_Fetcher, "Update Fetcher", "UPDATE", true, "Success: ID Number: " + fetcher.ID_Number + " Fetcher Name: " + (fetcher.Last_Name + ", " + fetcher.First_Name), DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Fetcher " + Constants.SuccessMessageUpdate;
                result.isSuccess = true;

                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }

        public async Task<ResultModel> DeleteTemporaryFetcher(int id, int user)
        {
            try
            {
                personEntity fetcher = await GetFetcherById(id);
                fetcher.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.DeleteAsyncTemporary(fetcher, fetcher.Person_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(fetcher.Updated_By, (int)Form.Person_Fetcher, "Deactivate Fetcher", "DEACTIVATE", false, "Failed: " + fetcher.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(fetcher.Updated_By, (int)Form.Person_Fetcher, "Deactivate Fetcher", "DEACTIVATE", true, "Success: " + fetcher.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Fetcher " + Constants.SuccessMessageTemporaryDelete;
                result.isSuccess = true;

                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;
                return result;
            }
        }

        public async Task<ResultModel> DeletePermanentFetcher(int id, int user)
        {
            try
            {
                personEntity fetcher = await GetFetcherById(id);
                fetcher.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.DeleteAsyncPermanent(fetcher, fetcher.Person_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(fetcher.Updated_By, (int)Form.Person_Fetcher, "Delete Permanently Fetcher", "PERMANENT DELETE", false, "Failed: " + fetcher.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(fetcher.Updated_By, (int)Form.Person_Fetcher, "Delete Permanently Fetcher", "PERMANENT DELETE", true, "Success: " + fetcher.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Fetcher " + Constants.SuccessMessagePermanentDelete;
                result.isSuccess = true;

                return result;
            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }

        public async Task<personEntity> GetOtherAccessById(int id)
        {
            try
            {
                return await _unitOfWork.PersonRepository.GetOtherAccessById(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResultModel> AddOtherAccess(personEntity otheraccess, emergencyContactEntity emergencyContact, int user)
        {
            try
            {
                var personcheck = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == otheraccess.ID_Number);

                if (personcheck != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(otheraccess.Added_By, (int)Form.Person_OtherAccess, "Add Other Access", "INSERT", false, "Failed: " + otheraccess.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.resultMessage = PERSON_OTHERACCESS_EXIST;
                    result.isSuccess = false;
                    return result;
                }

                otheraccess.Middle_Name = otheraccess.Middle_Name == "undefined" ? "" : otheraccess.Middle_Name;
                otheraccess.Person_Type = "O";
                otheraccess.Added_By = user;
                otheraccess.Updated_By = user;
                otheraccess.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                otheraccess.Last_Updated = DateTime.UtcNow.ToLocalTime();
                otheraccess.IsActive = true;
                otheraccess.ToDisplay = true;

                emergencyContact.Last_Updated = DateTime.UtcNow.ToLocalTime();
                emergencyContact.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                emergencyContact.Added_By = user;
                emergencyContact.Updated_By = user;
                emergencyContact.IsActive = true;
                emergencyContact.ToDisplay = true;

                var data = await _unitOfWork.PersonRepository.AddOtherAccess(otheraccess, emergencyContact);

                if (data.resultCode == "500")
                {
                    return data;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(otheraccess.Added_By, (int)Form.Person_OtherAccess, "Add Other Access", "INSERT", true, "Success: " + otheraccess.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Other Access " + Constants.SuccessMessageAdd;
                result.isSuccess = true;
                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;
                return result;
            }
        }

        public async Task<ResultModel> UpdateOtherAccess(personEntity otheraccess, emergencyContactEntity emergencyContact, int user)
        {
            try
            {
                emergencyContact.Updated_By = user;
                otheraccess.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.UpdateOtherAccess(otheraccess, emergencyContact);

                if (data == null)
                {
                    return data;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(otheraccess.Updated_By, (int)Form.Person_OtherAccess, "Update Other Access", "UPDATE", true, "Success: ID Number: " + otheraccess.ID_Number + " Other Access Name: " + (otheraccess.Last_Name + ", " + otheraccess.First_Name), DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Other Access " + Constants.SuccessMessageUpdate;
                result.isSuccess = true;

                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }

        public async Task<ResultModel> DeleteTemporaryOtherAccess(int id, int user)
        {
            try
            {
                personEntity otheraccess = await GetOtherAccessById(id);
                otheraccess.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.DeleteAsyncTemporary(otheraccess, otheraccess.Person_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(otheraccess.Updated_By, (int)Form.Person_OtherAccess, "Deactivate Other Access", "DEACTIVATE", false, "Failed: " + otheraccess.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(otheraccess.Updated_By, (int)Form.Person_OtherAccess, "Deactivate Other Access", "DEACTIVATE", true, "Success: " + otheraccess.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Other Access " + Constants.SuccessMessageTemporaryDelete;
                result.isSuccess = true;

                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;
                return result;
            }
        }

        public async Task<ResultModel> DeletePermanentOtherAccess(int id, int user)
        {
            try
            {
                personEntity otheraccess = await GetOtherAccessById(id);
                otheraccess.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.DeleteAsyncPermanent(otheraccess, otheraccess.Person_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(otheraccess.Updated_By, (int)Form.Person_OtherAccess, "Delete Permanently Other Access", "PERMANENT DELETE", false, "Failed: " + otheraccess.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(otheraccess.Updated_By, (int)Form.Person_OtherAccess, "Delete Permanently Other Access", "PERMANENT DELETE", true, "Success: " + otheraccess.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Other Access " + Constants.SuccessMessagePermanentDelete;
                result.isSuccess = true;

                return result;
            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }

        public async Task<personEntity> GetVisitorById(int id)
        {
            try
            {
                return await _unitOfWork.PersonRepository.GetVisitorById(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResultModel> AddVisitor(personEntity visitor, int user)
        {
            try
            {
                var personcheck = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == visitor.ID_Number);
                if (personcheck != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(visitor.Added_By, (int)Form.Person_Visitor, "Add Visitor", "INSERT", false, "Failed: " + visitor.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.resultMessage = PERSON_VISITOR_EXIST;
                    result.isSuccess = false;
                    return result;
                }

                visitor.Person_Type = "V";
                visitor.Added_By = user;
                visitor.Updated_By = user;
                visitor.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                visitor.Last_Updated = DateTime.UtcNow.ToLocalTime();
                visitor.IsActive = true;
                visitor.ToDisplay = true;

                var data = await _unitOfWork.PersonRepository.AddVisitor(visitor);

                if (data.resultCode == "500")
                {
                    return data;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(visitor.Added_By, (int)Form.Person_Visitor, "Add Visitor", "INSERT", true, "Success: " + visitor.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Visitor " + Constants.SuccessMessageAdd;
                result.isSuccess = true;
                return result;
            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;
                return result;
            }
        }

        public async Task<ResultModel> UpdateVisitor(personEntity visitor, int user)
        {
            try
            {
                visitor.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.UpdateVisitor(visitor);

                if (data == null)
                {
                    return data;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(visitor.Updated_By, (int)Form.Person_Visitor, "Update Visitor", "UPDATE", true, "Success: ID Number: " + visitor.ID_Number + " Visitor Name: " + (visitor.Last_Name + ", " + visitor.First_Name), DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Visitor " + Constants.SuccessMessageUpdate;
                result.isSuccess = true;

                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }

        public async Task<ResultModel> DeleteTemporaryVisitor(int id, int user)
        {
            try
            {
                personEntity visitor = await GetVisitorById(id);
                visitor.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.DeleteAsyncTemporary(visitor, visitor.Person_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(visitor.Updated_By, (int)Form.Person_Visitor, "Deactivate Visitor", "DEACTIVATE", false, "Failed: " + visitor.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(visitor.Updated_By, (int)Form.Person_Visitor, "Deactivate Visitor", "DEACTIVATE", true, "Success: " + visitor.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Visitor " + Constants.SuccessMessageTemporaryDelete;
                result.isSuccess = true;

                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;
                return result;
            }
        }

        public async Task<ResultModel> DeletePermanentVisitor(int id, int user)
        {
            try
            {
                personEntity visitor = await GetVisitorById(id);
                visitor.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.DeleteAsyncPermanent(visitor, visitor.Person_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(visitor.Updated_By, (int)Form.Person_Visitor, "Delete Permanently Visitor", "PERMANENT DELETE", false, "Failed: " + visitor.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(visitor.Updated_By, (int)Form.Person_Visitor, "Delete Permanently Visitor", "PERMANENT DELETE", true, "Success: " + visitor.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Visitor " + Constants.SuccessMessagePermanentDelete;
                result.isSuccess = true;

                return result;
            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }

        public async Task<personEntity> GetEmployeeById(int id)
        {
            try
            {
                return await _unitOfWork.PersonRepository.GetEmployeeById(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResultModel> AddEmployee(personEntity employee, int user)
        {
            try
            {
                var personcheck = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == employee.ID_Number);
                if (personcheck != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Added_By, (int)Form.Person_Employee, "Add Employee", "INSERT", false, "Failed: " + employee.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                employee.Person_Type = "E";
                employee.Added_By = user;

                var data = await _unitOfWork.PersonRepository.AddAsyncWithBase(employee);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Added_By, (int)Form.Person_Employee, "Add Employee", "INSERT", false, "Failed: " + employee.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Added_By, (int)Form.Person_Employee, "Add Employee", "INSERT", true, "Success: " + employee.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Employee " + Constants.SuccessMessageAdd;
                result.isSuccess = true;
                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;
                return result;
            }
        }

        public async Task<ResultModel> UpdateEmployee(personEntity employee, int user)
        {
            try
            {
                employee.Person_Type = "E";
                employee.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.UpdateAsyncWithBase(employee, employee.Person_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Updated_By, (int)Form.Person_Employee, "Update Employee", "UPDATE", false, "Failed: " + employee.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Updated_By, (int)Form.Person_Employee, "Update Employee", "UPDATE", true, "Success: ID Number: " + employee.ID_Number + " Employee Name: " + (employee.Last_Name + ", " + employee.First_Name), DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Employee " + Constants.SuccessMessageUpdate;
                result.isSuccess = true;

                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }

        public async Task<ResultModel> DeleteTemporaryEmployee(int id, int user)
        {
            try
            {
                personEntity employee = await GetEmployeeById(id);
                employee.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.DeleteAsyncTemporary(employee, employee.Person_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Updated_By, (int)Form.Person_Employee, "Deactivate Employee", "DEACTIVATE", false, "Failed: " + employee.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Updated_By, (int)Form.Person_Employee, "Deactivate Employee", "DEACTIVATE", true, "Success: " + employee.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Employee " + Constants.SuccessMessageTemporaryDelete;
                result.isSuccess = true;

                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;
                return result;
            }
        }

        public async Task<ResultModel> DeletePermanentEmployee(int id, int user)
        {
            try
            {
                personEntity employee = await GetEmployeeById(id);
                employee.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.DeleteAsyncPermanent(employee, employee.Person_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Updated_By, (int)Form.Person_Employee, "Delete Permanently Employee", "PERMANENT DELETE", false, "Failed: " + employee.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(employee.Updated_By, (int)Form.Person_Employee, "Delete Permanently Employee", "PERMANENT DELETE", true, "Success: " + employee.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Employee " + Constants.SuccessMessagePermanentDelete;
                result.isSuccess = true;

                return result;
            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }

        public async Task<personEntity> GetStudentById(int id)
        {
            try
            { 
                return await _unitOfWork.PersonRepository.GetStudentById(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResultModel> AddStudent(personEntity student, emergencyContactEntity emergencyContact, int user)
        {
            try
            {
                var personcheck = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == student.ID_Number);
                if (personcheck != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(student.Added_By, (int)Form.Person_Student, "Add Student", "INSERT", false, "Failed: " + student.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.resultMessage = "Student Registration Failed.";
                    result.isSuccess = false;
                    return result;
                }

                student.Gender = (student.Gender == "Male" ? "M" : "F");
                student.Person_Type = "S";
                student.Added_By = user;
                student.Updated_By = user;
                student.IsActive = true;

                var data = await _unitOfWork.PersonRepository.AddAsyncWithBase(student);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(student.Added_By, (int)Form.Person_Student, "Add Student", "INSERT", false, "Failed: " + student.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.resultMessage = "Student Registration Failed.";
                    result.isSuccess = false;
                    return result;
                }

                var exist = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == student.ID_Number);

                emergencyContact.Connected_PersonID = exist.Person_ID;
                await _unitOfWork.PersonRepository.AddStudent(emergencyContact, user);

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(student.Added_By, (int)Form.Person_Student, "Add Student", "INSERT", true, "Success: " + student.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Student " + Constants.SuccessMessageAdd;
                result.isSuccess = true;
                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;
                return result;
            }
        }

        public async Task<ResultModel> UpdateStudent(personEntity student, emergencyContactEntity emergencyContact, int user)
        {
            try
            {
                var exist = await _unitOfWork.PersonRepository.GetStudentById(student.Person_ID);

                student.Gender = (student.Gender == "Male" ? "M" : "F");
                student.Person_Type = "S";
                student.Updated_By = user;
                student.IsActive = true;
                
                var educlevel = await _unitOfWork.EducationLevelRepository.FindAsync(x => x.Level_ID == student.Educ_Level_ID && x.Campus_ID == student.Campus_ID && x.IsActive == true);

                student.College_ID = educlevel.hasCourse == true ? student.College_ID : 0;
                student.Course_ID = educlevel.hasCourse == true ? student.Course_ID : 0;

                if (student.EducStatus == "Dropped Out")
                {
                    student.IsDropOut = true;
                    student.DropOutCode = student.DropOutDesc;
                    student.DropOutOtherRemark = student.DropOutRemarks == null ? "" : student.DropOutRemarks;

                    student.IsTransferred = false;
                    student.TransferredSchoolName = string.Empty;
                    student.IsTransferredIn = false;
                    student.TransferredInSchoolName = string.Empty;
                }
                else if (student.EducStatus == "Transferred Out")
                {
                    student.IsTransferred = true;
                    student.TransferredSchoolName = student.SchoolName;

                    student.IsDropOut = false;
                    student.DropOutCode = string.Empty;
                    student.DropOutOtherRemark = string.Empty;
                    student.IsTransferredIn = false;
                    student.TransferredInSchoolName = string.Empty;
                }
                else if (student.EducStatus == "Transferred In")
                {
                    student.IsTransferredIn = true;
                    student.TransferredInSchoolName = student.SchoolName;

                    student.IsDropOut = false;
                    student.DropOutCode = string.Empty;
                    student.DropOutOtherRemark = string.Empty;
                    student.IsTransferred = false;
                    student.TransferredSchoolName = string.Empty;
                }
                else
                {
                    student.IsDropOut = false;
                    student.DropOutCode = string.Empty;
                    student.DropOutOtherRemark = string.Empty;

                    student.IsTransferred = false;
                    student.TransferredSchoolName = string.Empty;
                    student.IsTransferredIn = false;
                    student.TransferredInSchoolName = string.Empty;
                }
                
                //student.IsDropOut = exist.IsDropOut;
                //student.DropOutCode = exist.DropOutCode;
                //student.DropOutOtherRemark = exist.DropOutOtherRemark;
                //student.IsTransferred = exist.IsTransferred;
                //student.TransferredSchoolName = exist.TransferredSchoolName;
                //student.IsTransferredIn = exist.IsTransferredIn;
                //student.TransferredInSchoolName = exist.TransferredInSchoolName;

                var data = await _unitOfWork.PersonRepository.UpdateAsyncWithBase(student, student.Person_ID);

                emergencyContact.Connected_PersonID = student.Person_ID;

                await _unitOfWork.PersonRepository.UpdateStudent(emergencyContact, user);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(student.Updated_By, (int)Form.Person_Student, "Update Student", "UPDATE", false, "Failed: " + student.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(student.Updated_By, (int)Form.Person_Student, "Update Student", "UPDATE", true, "Success: ID Number: " + student.ID_Number + " Employee Name: " + (student.Last_Name + ", " + student.First_Name), DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Student " + Constants.SuccessMessageUpdate;
                result.isSuccess = true;

                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }

        public async Task<ResultModel> DeleteTemporaryStudent(int id, int user)
        {
            try
            {
                personEntity student = await GetStudentById(id);
                student.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.DeleteAsyncTemporary(student, student.Person_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(student.Updated_By, (int)Form.Person_Student, "Deactivate Student", "DEACTIVATE", false, "Failed: " + student.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(student.Updated_By, (int)Form.Person_Student, "Deactivate Student", "DEACTIVATE", true, "Success: " + student.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Student " + Constants.SuccessMessageTemporaryDelete;
                result.isSuccess = true;

                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;
                return result;
            }
        }

        public async Task<ResultModel> DeletePermanentStudent(int id, int user)
        {
            try
            {
                personEntity student = await GetStudentById(id);
                student.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.DeleteAsyncPermanent(student, student.Person_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(student.Updated_By, (int)Form.Person_Student, "Delete Permanently Student", "PERMANENT DELETE", false, "Failed: " + student.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(student.Updated_By, (int)Form.Person_Student, "Delete Permanently Student", "PERMANENT DELETE", true, "Success: " + student.ID_Number, DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Student " + Constants.SuccessMessagePermanentDelete;
                result.isSuccess = true;

                return result;
            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }

        public async Task<excusedStudentEntity> GetExcuse(string idNumber, string excusedDate)
        {
            try
            {
                return await _unitOfWork.PersonRepository.GetExcuse(idNumber, excusedDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<excusedStudentEntity> GetExcuseById(int id)
        {
            try
            {
                return await _unitOfWork.PersonRepository.GetExcuseById(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<excusePagedResult> GetAllExcuses(string id, int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.PersonRepository.GetAllExcuses(id, pageNo, pageSize, keyword);
        }

        public async Task<ResultModel> AddExcuse(excusedStudentEntity excuse, int user)
        {
            try
            {
                DateTime currDate = excuse.Excused_Start_Date;

                while (excuse.Excused_End_Date >= currDate)
                {
                    var exist = await _unitOfWork.PersonRepository.GetExcuse(excuse.IDNumber, currDate.ToString());

                    if (exist != null)
                        return CreateResult("409", STUDENT_EXCUSE_RANGE_EXIST, false);

                    currDate = currDate.AddDays(1);
                }
                
                currDate = excuse.Excused_Start_Date;
                
                while (excuse.Excused_End_Date >= currDate)
                {
                    var newExcuse = new excusedStudentEntity();
                    newExcuse.Excused_Date = currDate;
                    newExcuse.IDNumber = excuse.IDNumber;

                    await _unitOfWork.PersonRepository.AddExcuse(newExcuse, user);
                    currDate = currDate.AddDays(1);
                }
                
                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(excuse.Added_By, (int)Form.Person_Student, "Add Student Excused Date", "INSERT", true, "Success: " + excuse.IDNumber + " - " + excuse.Excused_Date.ToString(), DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Student Excused Dates" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteExcuseStudent(int id, int user)
        {
            try
            {
                excusedStudentEntity excusedStudent = await _unitOfWork.PersonRepository.GetExcuseById(id);

                await _unitOfWork.PersonRepository.DeleteExcuse(excusedStudent, user);

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(excusedStudent.Updated_By, (int)Form.Person_Student, "Delete Student Excused Date", "DELETE STUDENT EXCUSED DATE", true, "Success: " + excusedStudent.IDNumber + " - " + excusedStudent.Excused_Date.ToString(), DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Student Excused Date" + Constants.SuccessMessageDelete;
                result.isSuccess = true;

                return result;
            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }

        public async Task<ICollection<dropoutCodeEntity>> GetDropOutCode()
        {
            try
            {
                return await _unitOfWork.PersonRepository.GetDropOutCode().ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<ICollection<personEntity>> GetFetcherStudents()
        {
            try
            {
                return await _unitOfWork.PersonRepository.GetFetcherStudents().ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<ICollection<personEntity>> GetEmergencyLogoutStudents()
        {
            try
            {
                return await _unitOfWork.PersonRepository.GetEmergencyLogoutStudents().ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<ICollection<personEntity>> GetReportPersonList(string type)
        {
            try
            {
                return await _unitOfWork.PersonRepository.GetReportPersonList(type).ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<ResultModel> UpdateStudentStatus(personEntity student, int user)
        {
            try
            {
                student.Person_Type = "S";
                student.Updated_By = user;

                var data = await _unitOfWork.PersonRepository.UpdateAsyncWithBase(student, student.Person_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(student.Updated_By, (int)Form.Person_Student, "Update Student Status", "UPDATE", false, "Failed: " + student.ID_Number, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return result;
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(student.Updated_By, (int)Form.Person_Student, "Update Student Status", "UPDATE", true, "Success: ID Number: " + student.ID_Number + " Student Name: " + (student.Last_Name + ", " + student.First_Name), DateTime.UtcNow.ToLocalTime()));

                result = new ResultModel();
                result.resultCode = "200";
                result.resultMessage = "Student Status " + Constants.SuccessMessageUpdate;
                result.isSuccess = true;

                return result;

            }
            catch (Exception err)
            {
                result = new ResultModel();
                result.resultCode = "500";
                result.resultMessage = err.Message;
                result.isSuccess = false;

                return result;
            }
        }
    }
}