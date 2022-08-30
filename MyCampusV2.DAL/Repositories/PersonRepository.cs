using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using MyCampusV2.Common.Helpers;
using MySql.Data.MySqlClient;
using System.Data;

namespace MyCampusV2.DAL.Repositories
{
    public class PersonRepository : BaseRepository<personEntity>, IPersonRepository
    {
        private readonly MyCampusCardContext context;

        public PersonRepository(MyCampusCardContext Context) : base(Context)
        {
            context = Context;
        }

        public IQueryable<personEntity> GetAllEmployee()
        {
            return _context.PersonEntity
            .Include(c => c.GovIdsEntity)
            .Include(v => v.EmergencyContactEntity)
            .Include(b => b.PositionEntity)
            .Include(b => b.PositionEntity.DepartmentEntity)
            .Include(b => b.PositionEntity.DepartmentEntity.CampusEntity).Where(x => x.Person_Type.Equals("E"));
        }

        public IQueryable<personEntity> GetAllPersonVisitor()
        {
            try
            {
                return _context.PersonEntity.Where(x => x.Person_Type == "O");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResultModel> AddOtherAccess(personEntity person, emergencyContactEntity emergency)
        {
            ResultModel result = new ResultModel();

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.PersonEntity.AddAsync(person);

                    emergency.Connected_PersonID = person.Person_ID;
                    await _context.EmergencyContactEntity.AddAsync(emergency);

                    await _context.SaveChangesAsync();

                    dbcxtransaction.Commit();

                    result = new ResultModel();
                    result.resultCode = "200";
                    result.resultMessage = "Other Access " + Constants.SuccessMessageAdd;
                    result.isSuccess = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result = new ResultModel();
                    result.resultCode = "500";
                    result.resultMessage = ex.Message;
                    result.isSuccess = false;

                    dbcxtransaction.Rollback();
                    return result;
                }
            }
        }

        public async Task<ResultModel> UpdateOtherAccess(personEntity person, emergencyContactEntity emergency)
        {
            ResultModel result = new ResultModel();

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var newPerson = _context.PersonEntity.SingleOrDefault(x => x.ID_Number == person.ID_Number);

                    if (newPerson != null)
                    {
                        newPerson.Last_Name = person.Last_Name;
                        newPerson.Middle_Name = person.Middle_Name;
                        newPerson.First_Name = person.First_Name;
                        newPerson.Person_Type = "O";
                        newPerson.Gender = person.Gender;
                        newPerson.Birthdate = person.Birthdate;
                        newPerson.Email_Address = person.Email_Address;
                        newPerson.Contact_Number = person.Contact_Number;
                        newPerson.Telephone_Number = person.Telephone_Number;
                        newPerson.ID_Number = person.ID_Number;
                        newPerson.Updated_By = person.Updated_By;
                        newPerson.Last_Updated = DateTime.UtcNow.ToLocalTime();
                        newPerson.Campus_ID = person.Campus_ID;
                        newPerson.Position_ID = person.Position_ID;
                        newPerson.Department_ID = person.Department_ID;
                        newPerson.Office_ID = person.Office_ID;
                        newPerson.Address = person.Address;

                        emergencyContactEntity contact = _context.EmergencyContactEntity.SingleOrDefault(x => x.Connected_PersonID == newPerson.Person_ID);
                        if (contact != null)
                        {
                            contact.Address = emergency.Address;
                            contact.Contact_Number = emergency.Contact_Number;
                            contact.Full_Name = emergency.Full_Name;
                            contact.Relationship = emergency.Relationship;
                            contact.Last_Updated = DateTime.UtcNow.ToLocalTime();
                            contact.Updated_By = emergency.Updated_By;
                        }
                        else
                        {
                            emergencyContactEntity newContact = new emergencyContactEntity();
                            newContact.Connected_PersonID = emergency.Connected_PersonID;
                            newContact.Address = emergency.Address;
                            newContact.Contact_Number = emergency.Contact_Number;
                            newContact.Full_Name = emergency.Full_Name;
                            newContact.Relationship = emergency.Relationship;
                            newContact.Added_By = emergency.Updated_By;
                            newContact.Updated_By = emergency.Updated_By;
                            newContact.Date_Time_Added = DateTime.Now;
                            newContact.Last_Updated = DateTime.Now;
                            newContact.IsActive = true;
                            newContact.ToDisplay = true;
                            await _context.EmergencyContactEntity.AddAsync(newContact);
                        }
                        
                        _context.Entry(contact).Property(x => x.Last_Updated).IsModified = true;
                        _context.Entry(contact).Property(x => x.Date_Time_Added).IsModified = false;
                        _context.Entry(contact).Property(x => x.Added_By).IsModified = false;
                    }

                    _context.Entry(newPerson).Property(x => x.Last_Updated).IsModified = true;
                    _context.Entry(newPerson).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(newPerson).Property(x => x.Added_By).IsModified = false;

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();

                    result = new ResultModel();
                    result.resultCode = "200";
                    result.resultMessage = "Other Access " + Constants.SuccessMessageAdd;
                    result.isSuccess = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result = new ResultModel();
                    result.resultCode = "500";
                    result.resultMessage = ex.Message;
                    result.isSuccess = false;

                    dbcxtransaction.Rollback();
                    return result;
                }
            }
        }

        public async Task<ResultModel> AddFetcher(personEntity person)
        {
            ResultModel result = new ResultModel();

            using (IDbContextTransaction dbcxtransaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    await this.context.PersonEntity.AddAsync(person);

                    await _context.SaveChangesAsync();

                    dbcxtransaction.Commit();

                    result = new ResultModel();
                    result.resultCode = "200";
                    result.resultMessage = "Fetcher " + Constants.SuccessMessageAdd;
                    result.isSuccess = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result = new ResultModel();
                    result.resultCode = "500";
                    result.resultMessage = ex.Message;
                    result.isSuccess = false;

                    dbcxtransaction.Rollback();
                    return result;
                }
            }
        }

        public async Task<ResultModel> AddVisitor(personEntity person)
        {
            ResultModel result = new ResultModel();

            using (IDbContextTransaction dbcxtransaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    await this.context.PersonEntity.AddAsync(person);

                    await _context.SaveChangesAsync();

                    dbcxtransaction.Commit();

                    result = new ResultModel();
                    result.resultCode = "200";
                    result.resultMessage = "Visitor " + Constants.SuccessMessageAdd;
                    result.isSuccess = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result = new ResultModel();
                    result.resultCode = "500";
                    result.resultMessage = ex.Message;
                    result.isSuccess = false;

                    dbcxtransaction.Rollback();
                    return result;
                }
            }
        }

        public async Task<ResultModel> UpdateVisitor(personEntity person)
        {
            ResultModel result = new ResultModel();

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var newPerson = _context.PersonEntity.SingleOrDefault(x => x.ID_Number == person.ID_Number);

                    if (result != null)
                    {
                        newPerson.Last_Name = person.Last_Name;
                        newPerson.Middle_Name = person.Middle_Name;
                        newPerson.First_Name = person.First_Name;
                        newPerson.Person_Type = "V";
                        newPerson.Gender = person.Gender;
                        newPerson.Birthdate = person.Birthdate;
                        newPerson.Email_Address = person.Email_Address;
                        newPerson.Contact_Number = person.Contact_Number;
                        newPerson.ID_Number = person.ID_Number;
                        newPerson.Updated_By = person.Updated_By;
                        newPerson.Last_Updated = DateTime.UtcNow.ToLocalTime();
                        newPerson.Campus_ID = person.Campus_ID;
                        newPerson.Address = person.Address;
                    }

                    _context.Entry(newPerson).Property(x => x.Last_Updated).IsModified = true;
                    _context.Entry(newPerson).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(newPerson).Property(x => x.Added_By).IsModified = false;

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();

                    result = new ResultModel();
                    result.resultCode = "200";
                    result.resultMessage = "Fetcher " + Constants.SuccessMessageAdd;
                    result.isSuccess = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result = new ResultModel();
                    result.resultCode = "500";
                    result.resultMessage = ex.Message;
                    result.isSuccess = false;

                    dbcxtransaction.Rollback();
                    return result;
                }
            }
        }



        public async Task<ResultModel> AddEmployee(emergencyContactEntity emergencyContact, govIdsEntity govIds, int user)
        {
            ResultModel result = new ResultModel();

            using (IDbContextTransaction dbcxtransaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    emergencyContact.Added_By = user;
                    emergencyContact.Updated_By = user;
                    emergencyContact.Date_Time_Added = DateTime.Now;
                    emergencyContact.Last_Updated = DateTime.Now;
                    emergencyContact.IsActive = true;
                    emergencyContact.ToDisplay = true;
                    await _context.EmergencyContactEntity.AddAsync(emergencyContact);

                    govIds.Date_Added = DateTime.Now;
                    govIds.Last_Updated = DateTime.Now;
                    await _context.GovIdsEntity.AddAsync(govIds);

                    await _context.SaveChangesAsync();

                    dbcxtransaction.Commit();

                    result = new ResultModel();
                    result.resultCode = "200";
                    result.resultMessage = "Employee " + Constants.SuccessMessageAdd;
                    result.isSuccess = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result = new ResultModel();
                    result.resultCode = "500";
                    result.resultMessage = ex.Message;
                    result.isSuccess = false;

                    dbcxtransaction.Rollback();
                    return result;
                }
            }
        }

        public async Task<ResultModel> UpdateEmployee(emergencyContactEntity emergencyContact, govIdsEntity govIds, int user)
        {
            ResultModel result = new ResultModel();

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    emergencyContactEntity contact = _context.EmergencyContactEntity.SingleOrDefault(x => x.Connected_PersonID == emergencyContact.Connected_PersonID);
                    if (contact != null)
                    {
                        contact.Address = emergencyContact.Address;
                        contact.Contact_Number = emergencyContact.Contact_Number;
                        contact.Full_Name = emergencyContact.Full_Name;
                        contact.Relationship = emergencyContact.Relationship;

                        contact.Last_Updated = DateTime.Now;
                        contact.Updated_By = user;
                        _context.Entry(contact).Property(x => x.Connected_PersonID).IsModified = false;
                        _context.Entry(contact).Property(x => x.Date_Time_Added).IsModified = false;
                        _context.Entry(contact).Property(x => x.Added_By).IsModified = false;
                    }
                    else
                    {
                        emergencyContactEntity newContact = new emergencyContactEntity();
                        newContact.Connected_PersonID = emergencyContact.Connected_PersonID;
                        newContact.Address = emergencyContact.Address;
                        newContact.Contact_Number = emergencyContact.Contact_Number;
                        newContact.Full_Name = emergencyContact.Full_Name;
                        newContact.Relationship = emergencyContact.Relationship;
                        newContact.Added_By = user;
                        newContact.Updated_By = user;
                        newContact.Date_Time_Added = DateTime.Now;
                        newContact.Last_Updated = DateTime.Now;
                        newContact.IsActive = true;
                        newContact.ToDisplay = true;
                        await _context.EmergencyContactEntity.AddAsync(newContact);
                    }

                    govIdsEntity ids = _context.GovIdsEntity.SingleOrDefault(x => x.Person_ID == govIds.Person_ID);
                    if (ids != null)
                    {
                        ids.SSS = govIds.SSS;
                        ids.TIN = govIds.TIN;
                        ids.PhilHealth = govIds.PhilHealth;
                        ids.PAG_IBIG = govIds.PAG_IBIG;

                        ids.Last_Updated = DateTime.Now;
                        _context.Entry(ids).Property(x => x.Person_ID).IsModified = false;
                        _context.Entry(ids).Property(x => x.Date_Added).IsModified = false;
                    }
                    else
                    {
                        govIdsEntity newIds = new govIdsEntity();

                        newIds.Person_ID = govIds.Person_ID;
                        newIds.SSS = govIds.SSS;
                        newIds.TIN = govIds.TIN;
                        newIds.PhilHealth = govIds.PhilHealth;
                        newIds.PAG_IBIG = govIds.PAG_IBIG;
                        newIds.Date_Added = DateTime.Now;
                        newIds.Last_Updated = DateTime.Now;

                        await _context.GovIdsEntity.AddAsync(newIds);
                    }

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();

                    result = new ResultModel();
                    result.resultCode = "200";
                    result.resultMessage = "Employee " + Constants.SuccessMessageAdd;
                    result.isSuccess = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result = new ResultModel();
                    result.resultCode = "500";
                    result.resultMessage = ex.Message;
                    result.isSuccess = false;

                    dbcxtransaction.Rollback();
                    return result;
                }
            }
        }

        public async Task<Boolean> AddEmployeeWithBoolReturn(personEntity personInfo, emergencyContactEntity emergencyInfo, govIdsEntity govInfo, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    personInfo.Added_By = user;
                    personInfo.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                    personInfo.Person_Type = "E";
                    personInfo.ToDisplay = true;
                    personInfo.IsActive = true;
                    personInfo.Updated_By = user;
                    personInfo.Last_Updated = DateTime.UtcNow.ToLocalTime();

                    await _context.PersonEntity.AddAsync(personInfo);

                    emergencyInfo.Connected_PersonID = personInfo.Person_ID;

                    emergencyInfo.Added_By = user;
                    emergencyInfo.Updated_By = user;
                    emergencyInfo.Date_Time_Added = DateTime.Now;
                    emergencyInfo.Last_Updated = DateTime.Now;
                    emergencyInfo.IsActive = true;
                    emergencyInfo.ToDisplay = true;

                    await _context.EmergencyContactEntity.AddAsync(emergencyInfo);

                    govInfo.Person_ID = personInfo.Person_ID;
                    govInfo.Last_Updated = DateTime.UtcNow.ToLocalTime();
                    await _context.GovIdsEntity.AddAsync(govInfo); 

                    await _context.SaveChangesAsync();

                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public async Task<Boolean> UpdateEmployeeWithBoolReturn(personEntity personInfo, emergencyContactEntity emergencyInfo, govIdsEntity govInfo, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.PersonEntity.SingleOrDefault(x => x.ID_Number == personInfo.ID_Number);
                    if (result != null)
                    {
                        result.Last_Name = personInfo.Last_Name;
                        result.Middle_Name = (personInfo.Middle_Name == null ? string.Empty : personInfo.Middle_Name);
                        result.First_Name = personInfo.First_Name;
                        result.Gender = personInfo.Gender;
                        result.Birthdate = personInfo.Birthdate;
                        result.Email_Address = (personInfo.Email_Address == null ? string.Empty : personInfo.Email_Address);
                        result.Contact_Number = personInfo.Contact_Number;
                        result.ID_Number = personInfo.ID_Number;
                        result.Campus_ID = personInfo.Campus_ID;
                        result.EmpType_ID = personInfo.EmpType_ID;
                        result.EmpSubtype_ID = personInfo.EmpSubtype_ID;
                        result.Department_ID = personInfo.Department_ID;
                        result.Position_ID = personInfo.Position_ID;
                        result.Updated_By = user;
                        result.Last_Updated = DateTime.UtcNow.ToLocalTime();
                        result.ToDisplay = true;
                        result.IsActive = true;

                        emergencyContactEntity contact = _context.EmergencyContactEntity.SingleOrDefault(x => x.Connected_PersonID == result.Person_ID);
                        if (contact != null)
                        {
                            contact.Address = emergencyInfo.Address;
                            contact.Contact_Number = emergencyInfo.Contact_Number;
                            contact.Full_Name = emergencyInfo.Full_Name;
                            contact.Relationship = emergencyInfo.Relationship;
                            contact.ToDisplay = true;
                            contact.IsActive = true;
                            contact.Last_Updated = DateTime.UtcNow.ToLocalTime();
                        }
                        else
                        {
                            emergencyContactEntity newContact = new emergencyContactEntity();
                            newContact.Connected_PersonID = emergencyInfo.Connected_PersonID;
                            newContact.Address = emergencyInfo.Address;
                            newContact.Contact_Number = emergencyInfo.Contact_Number;
                            newContact.Full_Name = emergencyInfo.Full_Name;
                            newContact.Relationship = emergencyInfo.Relationship;
                            newContact.Added_By = user;
                            newContact.Updated_By = user;
                            newContact.Date_Time_Added = DateTime.Now;
                            newContact.Last_Updated = DateTime.Now;
                            newContact.IsActive = true;
                            newContact.ToDisplay = true;
                            await _context.EmergencyContactEntity.AddAsync(newContact);
                        }

                        govIdsEntity gov = _context.GovIdsEntity.SingleOrDefault(x => x.Person_ID == result.Person_ID);
                        if (gov != null)
                        {
                            gov.PAG_IBIG = govInfo.PAG_IBIG;
                            gov.PhilHealth = govInfo.PhilHealth;
                            gov.SSS = govInfo.SSS;
                            gov.TIN = govInfo.TIN;
                            gov.Last_Updated = DateTime.UtcNow.ToLocalTime();
                        }
                        else
                        {
                            govIdsEntity newIds = new govIdsEntity();
                            newIds.Person_ID = govInfo.Person_ID;
                            newIds.SSS = govInfo.SSS;
                            newIds.TIN = govInfo.TIN;
                            newIds.PhilHealth = govInfo.PhilHealth;
                            newIds.PAG_IBIG = govInfo.PAG_IBIG;
                            newIds.Date_Added = DateTime.Now;
                            newIds.Last_Updated = DateTime.Now;

                            await _context.GovIdsEntity.AddAsync(newIds);
                        }
                    }
                    
                    _context.Entry(result).Property(x => x.Last_Updated).IsModified = true;
                    
                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public IQueryable<personEntity> GetAllStudent()
        {
            try
            {
                var student = _context.PersonEntity
                    .Include(a => a.StudentSectionEntity)
                    .Include(b => b.StudentSectionEntity.YearSectionEntity)
                    .Include(c => c.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity)
                    .Include(d => d.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity)
                    .Include(e => e.EmergencyContactEntity)
                    .Include(f => f.CourseEntity)
                    .Include(g => g.CourseEntity.CollegeEntity)
                    .Where(x => x.Person_Type.Equals("S") && x.ToDisplay == true);
                return student;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IQueryable<personEntity> GetAllFetcher()
        {
            try
            {
                var student = _context.PersonEntity
                    .Include(a => a.CampusEntity)
                    .Where(b => b.Person_Type.Equals("F") && b.ToDisplay == true);
                return student;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IQueryable<personEntity> GetAllOtherAccess()
        {
            try
            {
                var otheraccess = _context.PersonEntity
                    .Include(v => v.EmergencyContactEntity)
                    .Include(b => b.OfficeEntity)
                    .Include(b => b.PositionEntity)
                    .Include(b => b.PositionEntity.DepartmentEntity)
                    .Include(b => b.PositionEntity.DepartmentEntity.CampusEntity)
                    .Where(x => x.Person_Type.Equals("O") && x.ToDisplay == true);
                return otheraccess;
            }
            catch (Exception ex)
            {
                throw;
            } 
        }

        public async Task AddStudent(emergencyContactEntity emergencyContact, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    emergencyContact.Added_By = user;
                    emergencyContact.Updated_By = user;
                    emergencyContact.Date_Time_Added = DateTime.Now;
                    emergencyContact.Last_Updated = DateTime.Now;
                    emergencyContact.IsActive = true;
                    emergencyContact.ToDisplay = true;
                    await _context.EmergencyContactEntity.AddAsync(emergencyContact);

                    await _context.SaveChangesAsync();
                    
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<Boolean> AddStudentWithBoolReturn(personEntity personInfo, emergencyContactEntity emergencyInfo, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    personInfo.Person_Type = "S";

                    personInfo.Added_By = user;
                    personInfo.Date_Time_Added = DateTime.Now;
                    personInfo.Last_Updated = DateTime.Now;
                    personInfo.Updated_By = user;
                    personInfo.IsActive = true;
                    personInfo.ToDisplay = true;

                    await _context.PersonEntity.AddAsync(personInfo);

                    emergencyInfo.Connected_PersonID = personInfo.Person_ID;

                    emergencyInfo.Added_By = user;
                    emergencyInfo.Updated_By = user;
                    emergencyInfo.Date_Time_Added = DateTime.Now;
                    emergencyInfo.Last_Updated = DateTime.Now;
                    emergencyInfo.IsActive = true;
                    emergencyInfo.ToDisplay = true;

                    await _context.EmergencyContactEntity.AddAsync(emergencyInfo);

                    await _context.SaveChangesAsync();

                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public async Task<Boolean> AddFetcherWithBoolReturn(personEntity personInfo, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    personInfo.Person_Type = "F";

                    personInfo.Middle_Name = (personInfo.Middle_Name == null ? string.Empty : personInfo.Middle_Name);
                    personInfo.Email_Address = (personInfo.Email_Address == null ? string.Empty : personInfo.Email_Address);

                    personInfo.IsActive = true;
                    personInfo.ToDisplay = true;
                    personInfo.Added_By = user;
                    personInfo.Date_Time_Added = DateTime.Now;
                    personInfo.Last_Updated = DateTime.Now;
                    personInfo.Updated_By = user;

                    await _context.PersonEntity.AddAsync(personInfo);

                    await _context.SaveChangesAsync();

                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public async Task<Boolean> AddOtherAccessWithBoolReturn(personEntity personInfo, emergencyContactEntity emergencyInfo, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    personInfo.Added_By = user;
                    personInfo.Date_Time_Added = DateTime.Now;
                    personInfo.Person_Type = "O";

                    personInfo.Updated_By = user;
                    personInfo.Last_Updated = DateTime.Now;
                    personInfo.IsActive = true;
                    personInfo.ToDisplay = true;

                    await _context.PersonEntity.AddAsync(personInfo);

                    emergencyInfo.Connected_PersonID = personInfo.Person_ID;

                    emergencyInfo.Added_By = user;
                    emergencyInfo.Updated_By = user;
                    emergencyInfo.Date_Time_Added = DateTime.Now;
                    emergencyInfo.Last_Updated = DateTime.Now;
                    emergencyInfo.IsActive = true;
                    emergencyInfo.ToDisplay = true;

                    await _context.EmergencyContactEntity.AddAsync(emergencyInfo);

                    await _context.SaveChangesAsync();

                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public async Task UpdateStudent(emergencyContactEntity emergencyContact, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    emergencyContactEntity contact = _context.EmergencyContactEntity.SingleOrDefault(x => x.Connected_PersonID == emergencyContact.Connected_PersonID);
                    if (contact != null)
                    {
                        contact.Address = emergencyContact.Address;
                        contact.Contact_Number = emergencyContact.Contact_Number;
                        contact.Full_Name = emergencyContact.Full_Name;
                        contact.Relationship = emergencyContact.Relationship;

                        contact.Last_Updated = DateTime.Now;
                        contact.Updated_By = user;
                        _context.Entry(contact).Property(x => x.Connected_PersonID).IsModified = false;
                        _context.Entry(contact).Property(x => x.Date_Time_Added).IsModified = false;
                        _context.Entry(contact).Property(x => x.Added_By).IsModified = false;
                    }
                    else
                    {
                        emergencyContactEntity newContact = new emergencyContactEntity();
                        newContact.Connected_PersonID = emergencyContact.Connected_PersonID;
                        newContact.Address = emergencyContact.Address;
                        newContact.Contact_Number = emergencyContact.Contact_Number;
                        newContact.Full_Name = emergencyContact.Full_Name;
                        newContact.Relationship = emergencyContact.Relationship;
                        newContact.Added_By = user;
                        newContact.Updated_By = user;
                        newContact.Date_Time_Added = DateTime.Now;
                        newContact.Last_Updated = DateTime.Now;
                        newContact.IsActive = true;
                        newContact.ToDisplay = true;
                        await _context.EmergencyContactEntity.AddAsync(newContact);
                    }
                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<Boolean> UpdateStudentWithBoolReturn(personEntity personInfo, emergencyContactEntity emergencyInfo, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.PersonEntity.SingleOrDefault(x => x.ID_Number == personInfo.ID_Number);

                    if (result != null)
                    {
                        result.Last_Name = personInfo.Last_Name;
                        result.Middle_Name = (personInfo.Middle_Name == null ? string.Empty : personInfo.Middle_Name);
                        result.First_Name = personInfo.First_Name;
                        result.Gender = personInfo.Gender;
                        result.Birthdate = personInfo.Birthdate;
                        result.Email_Address = (personInfo.Email_Address == null ? string.Empty : personInfo.Email_Address);
                        result.Contact_Number = personInfo.Contact_Number;
                        result.Telephone_Number = personInfo.Telephone_Number;

                        result.DateEnrolled = (personInfo.DateEnrolled == null ? DateTime.Now : Convert.ToDateTime(personInfo.DateEnrolled));
                        result.Campus_ID = personInfo.Campus_ID;
                        result.Educ_Level_ID = personInfo.Educ_Level_ID;
                        result.Year_Section_ID = personInfo.Year_Section_ID;
                        result.StudSec_ID = personInfo.StudSec_ID;
                        result.College_ID = personInfo.College_ID;
                        result.Course_ID = personInfo.Course_ID;
                        result.IsDropOut = personInfo.IsDropOut;
                        result.DropOutCode = personInfo.DropOutCode;
                        result.DropOutOtherRemark = personInfo.DropOutOtherRemark;
                        result.IsTransferredIn = personInfo.IsTransferredIn;
                        result.TransferredInSchoolName = personInfo.TransferredInSchoolName;
                        result.IsTransferred = personInfo.IsTransferred;
                        result.TransferredSchoolName = personInfo.TransferredSchoolName;

                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;
                        result.IsActive = true;
                        result.ToDisplay = true;

                        emergencyContactEntity contact = _context.EmergencyContactEntity.SingleOrDefault(x => x.Connected_PersonID == result.Person_ID);
                        if (contact != null)
                        {
                            contact.Address = emergencyInfo.Address;
                            contact.Contact_Number = emergencyInfo.Contact_Number;
                            contact.Full_Name = emergencyInfo.Full_Name;
                            contact.Relationship = emergencyInfo.Relationship;
                        }
                        else
                        {
                            emergencyContactEntity newContact = new emergencyContactEntity();
                            newContact.Connected_PersonID = emergencyInfo.Connected_PersonID;
                            newContact.Address = emergencyInfo.Address;
                            newContact.Contact_Number = emergencyInfo.Contact_Number;
                            newContact.Full_Name = emergencyInfo.Full_Name;
                            newContact.Relationship = emergencyInfo.Relationship;
                            newContact.Added_By = user;
                            newContact.Updated_By = user;
                            newContact.Date_Time_Added = DateTime.Now;
                            newContact.Last_Updated = DateTime.Now;
                            newContact.IsActive = true;
                            newContact.ToDisplay = true;
                            await _context.EmergencyContactEntity.AddAsync(newContact);
                        }
                    }
                    _context.Entry(result).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(result).Property(x => x.Added_By).IsModified = false;
                    _context.Entry(result).Property(x => x.Last_Updated).IsModified = true;
                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public async Task<Boolean> UpdateFetcherWithBoolReturn(personEntity personInfo, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.PersonEntity.SingleOrDefault(x => x.ID_Number == personInfo.ID_Number);

                    if (result != null)
                    {
                        result.Last_Name = personInfo.Last_Name;
                        result.Middle_Name = (personInfo.Middle_Name == null ? string.Empty : personInfo.Middle_Name);
                        result.First_Name = personInfo.First_Name;
                        result.Person_Type = personInfo.Person_Type;
                        result.Gender = personInfo.Gender;
                        result.Birthdate = personInfo.Birthdate;
                        result.Email_Address = (personInfo.Email_Address == null ? string.Empty : personInfo.Email_Address);
                        result.Contact_Number = personInfo.Contact_Number;
                        result.Address = personInfo.Address;
                        result.Fetcher_Relationship = personInfo.Fetcher_Relationship;
                        result.Campus_ID = personInfo.Campus_ID;

                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;
                    }
                    _context.Entry(result).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(result).Property(x => x.Added_By).IsModified = false;
                    _context.Entry(result).Property(x => x.Last_Updated).IsModified = true;
                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public async Task<ResultModel> UpdateFetcher(personEntity person)
        {
            ResultModel result = new ResultModel();

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var newPerson = _context.PersonEntity.SingleOrDefault(x => x.ID_Number == person.ID_Number);

                    if (result != null)
                    {
                        newPerson.Last_Name = person.Last_Name;
                        newPerson.Middle_Name = person.Middle_Name;
                        newPerson.First_Name = person.First_Name;
                        newPerson.Person_Type = "F";
                        newPerson.Gender = person.Gender;
                        newPerson.Birthdate = person.Birthdate;
                        newPerson.Email_Address = person.Email_Address;
                        newPerson.Contact_Number = person.Contact_Number;
                        newPerson.ID_Number = person.ID_Number;
                        newPerson.Updated_By = person.Updated_By;
                        newPerson.Last_Updated = DateTime.UtcNow.ToLocalTime();
                        newPerson.Campus_ID = person.Campus_ID;
                        newPerson.Fetcher_Relationship = person.Fetcher_Relationship;
                        newPerson.Address = person.Address;
                    }

                    _context.Entry(newPerson).Property(x => x.Last_Updated).IsModified = true;
                    _context.Entry(newPerson).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(newPerson).Property(x => x.Added_By).IsModified = false;

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();

                    result = new ResultModel();
                    result.resultCode = "200";
                    result.resultMessage = "Fetcher " + Constants.SuccessMessageAdd;
                    result.isSuccess = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result = new ResultModel();
                    result.resultCode = "500";
                    result.resultMessage = ex.Message;
                    result.isSuccess = false;

                    dbcxtransaction.Rollback();
                    return result;
                }
            }
        }

        public async Task<Boolean> UpdateOtherAccessWithBoolReturn(personEntity personInfo, emergencyContactEntity emergencyInfo, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.PersonEntity.SingleOrDefault(x => x.ID_Number == personInfo.ID_Number);

                    if (result != null)
                    {
                        result.Last_Name = personInfo.Last_Name;
                        result.Middle_Name = (personInfo.Middle_Name == null ? string.Empty : personInfo.Middle_Name);
                        result.First_Name = personInfo.First_Name;
                        result.Person_Type = "O";
                        result.Gender = personInfo.Gender;
                        result.Birthdate = personInfo.Birthdate;
                        result.Email_Address = (personInfo.Email_Address == null ? string.Empty : personInfo.Email_Address);
                        result.Contact_Number = personInfo.Contact_Number;
                        result.Telephone_Number = (personInfo.Telephone_Number == null ? string.Empty : personInfo.Telephone_Number);
                        result.ID_Number = personInfo.ID_Number;
                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;

                        result.Campus_ID = personInfo.Campus_ID;
                        result.Position_ID = personInfo.Position_ID;
                        result.Department_ID = personInfo.Department_ID;
                        result.Office_ID = personInfo.Office_ID;

                        //personEntity emp = _context.PersonEntity.SingleOrDefault(x => x.Person_ID == result.Person_ID);
                        //emp.Campus_ID = personInfo.Campus_ID;
                        //emp.Position_ID = personInfo.Position_ID;
                        //emp.Department_ID = personInfo.Department_ID;
                        //emp.Office_ID = personInfo.Office_ID;
                        //emp.Updated_By = user;
                        //emp.Last_Updated = DateTime.Now;

                        emergencyContactEntity contact = _context.EmergencyContactEntity.SingleOrDefault(x => x.Connected_PersonID == result.Person_ID);
                        if (contact != null)
                        {
                            contact.Address = emergencyInfo.Address;
                            contact.Contact_Number = emergencyInfo.Contact_Number;
                            contact.Full_Name = emergencyInfo.Full_Name;
                            contact.Relationship = emergencyInfo.Relationship;
                        }
                        else
                        {
                            emergencyContactEntity newContact = new emergencyContactEntity();
                            newContact.Connected_PersonID = emergencyInfo.Connected_PersonID;
                            newContact.Address = emergencyInfo.Address;
                            newContact.Contact_Number = emergencyInfo.Contact_Number;
                            newContact.Full_Name = emergencyInfo.Full_Name;
                            newContact.Relationship = emergencyInfo.Relationship;
                            newContact.Added_By = user;
                            newContact.Updated_By = user;
                            newContact.Date_Time_Added = DateTime.Now;
                            newContact.Last_Updated = DateTime.Now;
                            newContact.IsActive = true;
                            newContact.ToDisplay = true;
                            await _context.EmergencyContactEntity.AddAsync(newContact);
                        }

                        //_context.Entry(emp).Property(x => x.Last_Updated).IsModified = true;
                    }

                    _context.Entry(result).Property(x => x.Last_Updated).IsModified = true;


                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public IQueryable<personEntity> GetAllWithOutCard()
        {
            try
            {
                var persons = _context.PersonEntity
                    .Include(a => a.CardDetailsEntity)
                    .Where(a => (a.CardDetailsEntity.Count() == 0 ||
                    a.CardDetailsEntity.Where(b => b.IsActive && (b.Person_ID != 1 && b.Person_ID != 2)).Count() == 0));

                return persons;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IQueryable<personEntity> GetAllWithCard()
        {
            try
            {
                var persons = _context.PersonEntity
                    .Include(a => a.CardDetailsEntity)
                    .Where(a => (a.CardDetailsEntity.Where(b => b.IsActive).Count() > 0));

                return persons;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task AddPersonVisitor(personEntity personInfo, int user)
        {
            if (_context.PersonEntity.Any(x => x.ID_Number == personInfo.ID_Number))
            {
                throw new ArgumentException("ID Number already exist.");
            }

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    personInfo.Middle_Name = (personInfo.Middle_Name == "" || personInfo.Middle_Name == "undefined" || personInfo.Middle_Name == "null" || personInfo.Middle_Name == null ? "" : personInfo.Middle_Name);
                    personInfo.Added_By = user;
                    personInfo.Date_Time_Added = DateTime.Now;
                    personInfo.Updated_By = user;
                    personInfo.Last_Updated = DateTime.Now;
                    personInfo.Person_Type = "O";

                    await _context.PersonEntity.AddAsync(personInfo);

                    await _context.SaveChangesAsync();

                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task UpdatePersonVisitor(personEntity personInfo, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.PersonEntity.SingleOrDefault(x => x.ID_Number == personInfo.ID_Number);

                    if (result != null)
                    {
                        result.Last_Name = personInfo.Last_Name;
                        result.Middle_Name = (personInfo.Middle_Name == "" || personInfo.Middle_Name == "undefined" || personInfo.Middle_Name == "null" || personInfo.Middle_Name == null ? string.Empty : personInfo.Middle_Name);
                        result.First_Name = personInfo.First_Name;
                        result.Gender = personInfo.Gender;
                        result.Birthdate = personInfo.Birthdate;
                        result.Email_Address = (personInfo.Email_Address == null ? string.Empty : personInfo.Email_Address);
                        result.Contact_Number = (personInfo.Contact_Number == null ? string.Empty : personInfo.Contact_Number);
                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<Boolean> UpdatePersonVisitorWithBoolReturn(personEntity personInfo, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.PersonEntity.SingleOrDefault(x => x.ID_Number == personInfo.ID_Number);

                    if (result != null)
                    {
                        result.Last_Name = personInfo.Last_Name;
                        result.Middle_Name = (personInfo.Middle_Name == null ? string.Empty : personInfo.Middle_Name);
                        result.First_Name = personInfo.First_Name;
                        result.Gender = personInfo.Gender;
                        result.Birthdate = personInfo.Birthdate;
                        result.Email_Address = (personInfo.Email_Address == null ? string.Empty : personInfo.Email_Address);
                        result.Contact_Number = (personInfo.Contact_Number == null ? string.Empty : personInfo.Contact_Number);
                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public async Task<studentPagedResultVM> GetAllStudents(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var studentEntity = new List<studentVM>();
                int pageCount = 0;
                int rowCount = 0;
                using (MySqlConnection conn = new MySqlConnection(this.context.Database.GetDbConnection().ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "get_student_list";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pageNo", pageNo);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                        cmd.Parameters.AddWithValue("@keyword", keyword);

                        cmd.Parameters.AddWithValue("@rowCount", MySqlDbType.Int32);
                        cmd.Parameters["@rowCount"].Direction = ParameterDirection.Output;

                        cmd.Parameters.AddWithValue("@pageCount", MySqlDbType.Int32);
                        cmd.Parameters["@pageCount"].Direction = ParameterDirection.Output;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                studentEntity.Add(new studentVM()
                                {
                                    personId = Convert.ToInt32(reader["PersonID"]),
                                    idNumber = Convert.ToString(reader["IDNumber"]),
                                    campusName = Convert.ToString(reader["CampusName"]),
                                    educLevelName = Convert.ToString(reader["LevelName"]),
                                    collegeName = Convert.ToString(reader["CollegeName"]),
                                    courseName = Convert.ToString(reader["CourseName"]),
                                    firstName = Convert.ToString(reader["FirstName"]),
                                    lastName = Convert.ToString(reader["LastName"]),
                                    yearSecName = Convert.ToString(reader["YearSection"]),
                                    studSecName = Convert.ToString(reader["StudentSection"]),
                                    status = Convert.ToBoolean(reader["IsActive"]) == true ? "Active" : "Inactive",
                                    isDropOut = Convert.ToBoolean(reader["IsDropOut"]) == true ? "True" : "False",
                                    isTransferred = Convert.ToBoolean(reader["IsTransferred"]) == true ? "True" : "False",
                                    isTransferredIn = Convert.ToBoolean(reader["IsTransferredIn"]) == true ? "True" : "False",
                                    dropOutCode = Convert.ToString(reader["DropOutCode"]),
                                    dropOutOtherRemark = Convert.ToString(reader["DropOutOtherRemark"]),
                                    transferredInSchoolName = Convert.ToString(reader["TransferredInSchoolName"]),
                                    transferredSchoolName = Convert.ToString(reader["TransferredSchoolName"]),
                                });
                            }
                        }

                        rowCount = Convert.ToInt32(cmd.Parameters["@rowCount"].Value);
                        pageCount = Convert.ToInt32(cmd.Parameters["@pageCount"].Value);
                    }
                }

                studentPagedResultVM notifPagedResultVM = new studentPagedResultVM();
                notifPagedResultVM.PageCount = pageCount;
                notifPagedResultVM.RowCount = rowCount;
                notifPagedResultVM.students = studentEntity;
                notifPagedResultVM.CurrentPage = pageNo;
                notifPagedResultVM.PageSize = pageSize;

                return notifPagedResultVM;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public async Task<studentPagedResult> GetAllStudents(int pageNo, int pageSize, string keyword)
        //{
        //    try
        //    {
        //        var result = new studentPagedResult();
        //        result.CurrentPage = pageNo;
        //        result.PageSize = pageSize;

        //        if (keyword == null || keyword == "")
        //        {
        //            result.RowCount = _context.PersonEntity
        //                .Where(x => x.Person_Type == "S" && x.ToDisplay == true)
        //                .Count();
        //        }
        //        else if (keyword.Contains(","))
        //        {
        //            string[] fullname = keyword.Split(",");
        //            result.RowCount = _context.PersonEntity
        //                .Include(a => a.StudentSectionEntity)
        //                    .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
        //                    .ThenInclude(w => w.CampusEntity)
        //                .Include(b => b.StudentSectionEntity.YearSectionEntity)
        //                .Include(c => c.EmergencyContactEntity)
        //                .Include(d => d.CourseEntity)
        //                    .ThenInclude(y => y.CollegeEntity)
        //                    .ThenInclude(z => z.EducationalLevelEntity)
        //                .Where(x => x.Person_Type == "S" && x.ToDisplay == true && (x.ID_Number.Contains(keyword)
        //                    || (x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim()))
        //                    || x.CourseEntity.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
        //                    || x.CourseEntity.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.YearSectionEntity.YearSec_Name.Contains(keyword)
        //                    || x.CourseEntity.Course_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.Description.Contains(keyword))).Count();
        //        }
        //        else
        //        {
        //            List<personEntity> test = await this.context.PersonEntity
        //                    .Include(c => c.CampusEntity)
        //                    .Include(v => v.EducationalLevelEntity)
        //                    .Include(b => b.CollegeEntity)
        //                    //.Include(n => n.CourseEntity)
        //                    //.Include(m => m.YearSectionEntity)
        //                    //.Include(p => p.StudentSectionEntity)
        //                    .Where(x => x.Person_Type == "S" && x.ToDisplay == true && (
        //                           x.ID_Number.Contains(keyword)
        //                        || x.First_Name.Contains(keyword)
        //                        || x.Last_Name.Contains(keyword)
        //                        || x.EducationalLevelEntity.Level_Name.Contains(keyword)
        //                        //|| x.CollegeEntity.College_Name.Contains(keyword)
        //                        //|| x.CourseEntity.Course_Name.Contains(keyword)
        //                        //|| x.YearSectionEntity.YearSec_Name.Contains(keyword)
        //                        //|| x.StudentSectionEntity.Description.Contains(keyword)
        //                        //|| x.CampusEntity.Campus_Name.Contains(keyword)
        //                        )).ToListAsync();
        //            result.RowCount = _context.PersonEntity
        //                .Include(a => a.StudentSectionEntity)
        //                    .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
        //                        .ThenInclude(w => w.CampusEntity)
        //                .Include(b => b.StudentSectionEntity.YearSectionEntity)
        //                .Include(c => c.EmergencyContactEntity)
        //                .Include(d => d.CourseEntity)
        //                    .ThenInclude(y => y.CollegeEntity)
        //                        .ThenInclude(z => z.EducationalLevelEntity)
        //                .Where(x => x.Person_Type == "S" && x.ToDisplay == true && (x.ID_Number.Contains(keyword)
        //                    || x.First_Name.Contains(keyword)
        //                    || x.Last_Name.Contains(keyword)
        //                    || x.CourseEntity.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
        //                    || x.CourseEntity.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.YearSectionEntity.YearSec_Name.Contains(keyword)
        //                    || x.CourseEntity.Course_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.Description.Contains(keyword))).Count();
        //        }

        //        var pageCount = (double)result.RowCount / pageSize;
        //        result.PageCount = (int)Math.Ceiling(pageCount);

        //        var skip = (pageNo - 1) * pageSize;

        //        if (keyword == null || keyword == "")
        //        {
        //            result.students = await _context.PersonEntity
        //                .Include(a => a.StudentSectionEntity)
        //                    .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
        //                    .ThenInclude(w => w.CampusEntity)
        //                .Include(b => b.StudentSectionEntity.YearSectionEntity)
        //                .Include(c => c.EmergencyContactEntity)
        //                .Include(d => d.CourseEntity)
        //                    .ThenInclude(y => y.CollegeEntity)
        //                .Where(x => x.Person_Type == "S" && x.ToDisplay == true)
        //                .OrderBy(e => e.ID_Number)
        //                .Skip(skip).Take(pageSize).ToListAsync();
        //        }
        //        else if (keyword.Contains(","))
        //        {
        //            string[] fullname = keyword.Split(",");
        //            result.students = await _context.PersonEntity
        //                .Include(a => a.StudentSectionEntity)
        //                    .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
        //                    .ThenInclude(w => w.CampusEntity)
        //                .Include(b => b.StudentSectionEntity.YearSectionEntity)
        //                .Include(c => c.EmergencyContactEntity)
        //                .Include(d => d.CourseEntity)
        //                    .ThenInclude(y => y.CollegeEntity)
        //                    .ThenInclude(z => z.EducationalLevelEntity)
        //                    .ThenInclude(w => w.CampusEntity)
        //                .Where(x => x.Person_Type == "S" && x.ToDisplay == true && (x.ID_Number.Contains(keyword)
        //                    || (x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim()))
        //                    || x.CourseEntity.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
        //                    || x.CourseEntity.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.YearSectionEntity.YearSec_Name.Contains(keyword)
        //                    || x.CourseEntity.Course_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.Description.Contains(keyword)))
        //                .OrderBy(e => e.ID_Number)
        //                .Skip(skip).Take(pageSize).ToListAsync();
        //        }
        //        else
        //        {
        //            result.students = await _context.PersonEntity
        //                .Include(a => a.StudentSectionEntity)
        //                    .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
        //                    .ThenInclude(w => w.CampusEntity)
        //                .Include(b => b.StudentSectionEntity.YearSectionEntity)
        //                .Include(c => c.EmergencyContactEntity)
        //                .Include(d => d.CourseEntity)
        //                    .ThenInclude(y => y.CollegeEntity)
        //                    .ThenInclude(z => z.EducationalLevelEntity)
        //                    .ThenInclude(w => w.CampusEntity)
        //                .Where(x => x.Person_Type == "S" && x.ToDisplay == true && (x.ID_Number.Contains(keyword)
        //                    || x.First_Name.Contains(keyword)
        //                    || x.Last_Name.Contains(keyword)
        //                    || x.CourseEntity.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
        //                    || x.CourseEntity.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.YearSectionEntity.YearSec_Name.Contains(keyword)
        //                    || x.CourseEntity.Course_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
        //                    || x.StudentSectionEntity.Description.Contains(keyword)))
        //                .OrderBy(e => e.ID_Number)
        //                .Skip(skip).Take(pageSize).ToListAsync();
        //        }

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        public async Task<excusePagedResult> GetAllExcuses(string id, int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new excusePagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                {
                    result.RowCount = _context.ExcusedStudentEntity
                        .Where(x => x.IDNumber == id && x.IsActive == true && x.ToDisplay == true)
                        .Count();
                }
                else
                {
                    result.RowCount = _context.ExcusedStudentEntity
                        .Where(x => x.IDNumber == id && x.IsActive == true && x.ToDisplay == true && x.Excused_Date.ToString().Contains(keyword))
                        .Count();
                }

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                {
                    result.excuses = await _context.ExcusedStudentEntity
                        .Where(x => x.IDNumber == id && x.IsActive == true && x.ToDisplay == true)
                        .OrderByDescending(e => e.Excused_Date)
                        .Skip(skip).Take(pageSize).ToListAsync();
                }
                else
                {
                    result.excuses = await _context.ExcusedStudentEntity
                        .Where(x => x.IDNumber == id && x.IsActive == true && x.ToDisplay == true && x.Excused_Date.ToString().Contains(keyword))
                        .OrderByDescending(e => e.Excused_Date)
                        .Skip(skip).Take(pageSize).ToListAsync();
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<studentPagedResult> ExportAllStudents(string keyword, bool isCollege)
        {
            try
            {
                var result = new studentPagedResult();

                if (isCollege)
                {
                    if (keyword == null || keyword == "")
                    {
                        result.students = await _context.PersonEntity
                            .Include(a => a.StudentSectionEntity)
                                .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Include(b => b.StudentSectionEntity.YearSectionEntity)
                            .Include(c => c.EmergencyContactEntity)
                            .Include(d => d.CourseEntity)
                                .ThenInclude(y => y.CollegeEntity)
                                .ThenInclude(z => z.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Where(x => x.CourseEntity.CollegeEntity.EducationalLevelEntity.hasCourse.Equals(true) 
                                && x.IsActive == true
                                && x.Person_Type == "S")
                            .OrderBy(e => e.ID_Number).ToListAsync();
                    }
                    else if (keyword.Contains(","))
                    {
                        string[] fullname = keyword.Split(",");
                        result.students = await _context.PersonEntity
                            .Include(a => a.StudentSectionEntity)
                                .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Include(b => b.StudentSectionEntity.YearSectionEntity)
                            .Include(c => c.EmergencyContactEntity)
                            .Include(d => d.CourseEntity)
                                .ThenInclude(y => y.CollegeEntity)
                                .ThenInclude(z => z.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Where(x => (x.ID_Number.Contains(keyword)
                                || (x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim()))
                                || x.CourseEntity.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.CourseEntity.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.YearSec_Name.Contains(keyword)
                                || x.CourseEntity.Course_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.StudentSectionEntity.Description.Contains(keyword))
                                && x.CourseEntity.CollegeEntity.EducationalLevelEntity.hasCourse.Equals(true) 
                                && x.IsActive == true
                                && x.Person_Type == "S")
                            .OrderBy(e => e.ID_Number).ToListAsync();
                    }
                    else
                    {
                        result.students = await _context.PersonEntity
                            .Include(a => a.StudentSectionEntity)
                                .ThenInclude(z => z.YearSectionEntity.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Include(b => b.StudentSectionEntity.YearSectionEntity)
                            .Include(c => c.EmergencyContactEntity)
                            .Include(d => d.CourseEntity)
                                .ThenInclude(y => y.CollegeEntity)
                                .ThenInclude(z => z.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .OrderBy(e => e.ID_Number)
                            .Where(x => (x.ID_Number.Contains(keyword)
                                || x.First_Name.Contains(keyword)
                                || x.Last_Name.Contains(keyword)
                                || x.CourseEntity.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.CourseEntity.CollegeEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.YearSec_Name.Contains(keyword)
                                || x.CourseEntity.Course_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.StudentSectionEntity.Description.Contains(keyword))
                                && x.CourseEntity.CollegeEntity.EducationalLevelEntity.hasCourse.Equals(true)
                                && x.IsActive == true
                                && x.Person_Type == "S").ToListAsync();
                    }
                    return result;
                }
                else
                {
                    if (keyword == null || keyword == "")
                    {
                        result.students = await _context.PersonEntity
                            .Include(a => a.StudentSectionEntity)
                                .ThenInclude(z => z.YearSectionEntity)
                                .ThenInclude(z => z.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Include(b => b.StudentSectionEntity.YearSectionEntity)
                            .Include(c => c.EmergencyContactEntity)
                            .Where(x => x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.hasCourse.Equals(false) 
                                && x.IsActive == true
                                && x.Person_Type == "S")
                            .OrderBy(e => e.ID_Number).ToListAsync();
                    }
                    else if (keyword.Contains(","))
                    {
                        string[] fullname = keyword.Split(",");
                        result.students = await _context.PersonEntity
                            .Include(a => a.StudentSectionEntity)
                                .ThenInclude(z => z.YearSectionEntity)
                                .ThenInclude(z => z.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Include(b => b.StudentSectionEntity.YearSectionEntity)
                            .Include(c => c.EmergencyContactEntity)
                            .Where(x => (x.ID_Number.Contains(keyword)
                                || x.First_Name.Contains(fullname[0].Trim()) || x.First_Name.Contains(fullname[0].Trim())
                                || x.Last_Name.Contains(fullname[1].Trim()) || x.Last_Name.Contains(fullname[0].Trim())
                                || x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim())
                                || x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim())
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.YearSec_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.StudentSectionEntity.Description.Contains(keyword))
                                && x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.hasCourse.Equals(false) 
                                && x.IsActive == true
                                && x.Person_Type == "S")
                            .OrderBy(e => e.ID_Number).ToListAsync();
                    }
                    else
                    {
                        result.students = await _context.PersonEntity
                            .Include(a => a.StudentSectionEntity)
                                .ThenInclude(z => z.YearSectionEntity)
                                .ThenInclude(z => z.EducationalLevelEntity)
                                .ThenInclude(w => w.CampusEntity)
                            .Include(b => b.StudentSectionEntity.YearSectionEntity)
                            .Include(c => c.EmergencyContactEntity)
                            .OrderBy(e => e.ID_Number)
                            .Where(x => (x.ID_Number.Contains(keyword)
                                || x.First_Name.Contains(keyword)
                                || x.Last_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.YearSec_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_Name.Contains(keyword)
                                || x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.StudentSectionEntity.Description.Contains(keyword))
                                && x.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.hasCourse.Equals(false)
                                && x.IsActive == true
                                && x.Person_Type == "S").ToListAsync();
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<fetcherPagedResult> ExportAllFetchers(string keyword)
        {
            try
            {
                var result = new fetcherPagedResult();

                if (keyword == null || keyword == "")
                {
                    result.personfetchers = await _context.PersonEntity
                        .Include(a => a.CampusEntity)
                        .Where(b => b.Person_Type.Contains("F") && b.ToDisplay == true)
                        .OrderBy(e => e.ID_Number).ToListAsync();
                }
                else if (keyword.Contains(","))
                {
                    string[] fullname = keyword.Split(",");
                    result.personfetchers = await _context.PersonEntity
                        .Include(a => a.CampusEntity)
                        .Where(b => b.Person_Type.Contains("F") && b.ToDisplay == true && ((b.ID_Number.Contains(keyword)
                            || (b.First_Name.Contains(fullname[1].Trim()) && b.Last_Name.Contains(fullname[0].Trim())))))
                        .OrderBy(e => e.ID_Number).ToListAsync();
                }
                else
                {
                    result.personfetchers = await _context.PersonEntity
                        .Include(a => a.CampusEntity)
                        .Where(b => b.Person_Type.Contains("F") && b.ToDisplay == true && ((b.ID_Number.Contains(keyword)
                            || b.First_Name.Contains(keyword)
                            || b.Last_Name.Contains(keyword))))
                       .OrderBy(e => e.ID_Number).ToListAsync();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<otherAccessPagedResult> ExportAllOtherAccess(string keyword)
        {
            try
            {
                var result = new otherAccessPagedResult();

                if (keyword == null || keyword == "")
                {
                    result.personotheraccess = await _context.PersonEntity
                        .Include(a => a.EmergencyContactEntity)
                        .Include(a => a.OfficeEntity)
                        .Include(x => x.PositionEntity)
                        .ThenInclude(c => c.DepartmentEntity)
                        .ThenInclude(v => v.CampusEntity)
                        .Where(b => b.Person_Type.Contains("O") && b.ToDisplay == true)
                        .OrderBy(e => e.ID_Number).ToListAsync();
                }
                else if (keyword.Contains(","))
                {
                    string[] fullname = keyword.Split(",");
                    result.personotheraccess = await _context.PersonEntity
                        .Include(a => a.OfficeEntity)
                        .Include(x => x.PositionEntity)
                        .ThenInclude(c => c.DepartmentEntity)
                        .ThenInclude(v => v.CampusEntity)
                        .Where(x => x.Person_Type.Contains("O") && x.ToDisplay == true && (x.ID_Number.Contains(keyword)
                            || (x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim()))
                            || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
                            || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                            || x.OfficeEntity.Office_Name.Contains(keyword)
                            || x.PositionEntity.Position_Name.Contains(keyword)))
                        .OrderBy(e => e.ID_Number).ToListAsync();
                }
                else
                {
                    result.personotheraccess = await _context.PersonEntity
                        .Include(a => a.EmergencyContactEntity)
                        .Include(a => a.OfficeEntity)
                        .Include(x => x.PositionEntity)
                        .ThenInclude(c => c.DepartmentEntity)
                        .ThenInclude(v => v.CampusEntity)
                        .OrderBy(e => e.ID_Number)
                        .Where(x => x.Person_Type.Contains("O") && x.ToDisplay == true && (x.ID_Number.Contains(keyword)
                            || x.First_Name.Contains(keyword)
                            || x.Last_Name.Contains(keyword)
                            || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
                            || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                            || x.OfficeEntity.Office_Name.Contains(keyword)
                            || x.PositionEntity.Position_Name.Contains(keyword))).ToListAsync();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<employeePagedResult> GetAllEmployees(int pageNo, int pageSize, string keyword)
        {
            try
            { 
                var result = new employeePagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                {
                    result.RowCount = _context.PersonEntity
                        .Include(a => a.EmployeeSubTypeEntity)
                        .Include(c => c.PositionEntity)
                        .Include(d => d.DepartmentEntity)
                        .Include(e => e.CampusEntity)
                        .Include(f => f.EmergencyContactEntity)
                        .Include(g => g.GovIdsEntity)
                        .Where(x => x.Person_Type == "E" && x.IsActive == true 
                            && x.Person_ID != 1 & x.Person_ID != 2)
                        .Count();
                }
                else if (keyword.Contains(","))
                {
                    string[] fullname = keyword.Split(",");

                    result.RowCount = _context.PersonEntity
                        .Include(a => a.EmployeeSubTypeEntity)
                        .Include(c => c.PositionEntity)
                        .Include(d => d.DepartmentEntity)
                        .Include(e => e.CampusEntity)
                        .Include(f => f.EmergencyContactEntity)
                        .Include(g => g.GovIdsEntity)
                        .Where(x => x.Person_Type == "E" && x.IsActive == true 
                            && x.Person_ID != 1 & x.Person_ID != 2
                            && (x.ID_Number.Contains(keyword)
                            || (x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim()))
                            || x.EmployeeTypeEntity.EmpTypeDesc.Contains(keyword)
                            || x.EmployeeSubTypeEntity.EmpSubTypeDesc.Contains(keyword)
                            || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
                            || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                            || x.PositionEntity.Position_Name.Contains(keyword))).Count();
                }
                else
                {
                    result.RowCount = _context.PersonEntity
                        .Include(a => a.EmployeeSubTypeEntity)
                        .Include(c => c.PositionEntity)
                        .Include(d => d.DepartmentEntity)
                        .Include(e => e.CampusEntity)
                        .Include(f => f.EmergencyContactEntity)
                        .Include(g => g.GovIdsEntity)
                        .Where(x => x.Person_Type == "E" && x.IsActive == true 
                            && x.Person_ID != 1 & x.Person_ID != 2
                            && (x.ID_Number.Contains(keyword)
                            || x.First_Name.Contains(keyword)
                            || x.Last_Name.Contains(keyword)
                            || x.EmployeeTypeEntity.EmpTypeDesc.Contains(keyword)
                            || x.EmployeeSubTypeEntity.EmpSubTypeDesc.Contains(keyword)
                            || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
                            || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                            || x.PositionEntity.Position_Name.Contains(keyword))).Count();
                }

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                {
                    result.employees = await _context.PersonEntity
                        .Include(a => a.EmployeeSubTypeEntity)
                        .Include(c => c.PositionEntity)
                        .Include(d => d.DepartmentEntity)
                        .Include(e => e.CampusEntity)
                        .Include(f => f.EmergencyContactEntity)
                        .Include(g => g.GovIdsEntity)
                        .Where(x => x.Person_Type == "E" && x.IsActive == true 
                            && x.Person_ID != 1 & x.Person_ID != 2)
                        .OrderByDescending(e => e.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                }
                else if (keyword.Contains(","))
                {
                    string[] fullname = keyword.Split(",");
                    result.employees = await _context.PersonEntity
                        .Include(a => a.EmployeeSubTypeEntity)
                        .Include(c => c.PositionEntity)
                        .Include(d => d.DepartmentEntity)
                        .Include(e => e.CampusEntity)
                        .Include(f => f.EmergencyContactEntity)
                        .Include(g => g.GovIdsEntity)
                        .Where(x => x.Person_Type == "E" && x.IsActive == true
                            && x.Person_ID != 1 & x.Person_ID != 2
                            && (x.ID_Number.Contains(keyword)
                            || (x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim()))
                            || x.EmployeeTypeEntity.EmpTypeDesc.Contains(keyword)
                            || x.EmployeeSubTypeEntity.EmpSubTypeDesc.Contains(keyword)
                            || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
                            || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                            || x.PositionEntity.Position_Name.Contains(keyword)))
                        .OrderByDescending(e => e.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                }
                else
                {
                    result.employees = await _context.PersonEntity
                        .Include(a => a.EmployeeSubTypeEntity)
                        .Include(c => c.PositionEntity)
                        .Include(d => d.DepartmentEntity)
                        .Include(e => e.CampusEntity)
                        .Include(f => f.EmergencyContactEntity)
                        .Include(g => g.GovIdsEntity)
                        .Where(x => x.Person_Type == "E" && x.IsActive == true 
                            && x.Person_ID != 1 & x.Person_ID != 2
                            && (x.ID_Number.Contains(keyword)
                            || x.First_Name.Contains(keyword)
                            || x.Last_Name.Contains(keyword)
                            || x.EmployeeTypeEntity.EmpTypeDesc.Contains(keyword)
                            || x.EmployeeSubTypeEntity.EmpSubTypeDesc.Contains(keyword)
                            || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
                            || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                            || x.PositionEntity.Position_Name.Contains(keyword)))
                        .OrderByDescending(e => e.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public async Task<employeePagedResult> ExportAllEmployees(string keyword)
        {
            try
            {
                var result = new employeePagedResult(); 

                if (keyword == null || keyword == "") 
                {
                    result.employees = await _context.PersonEntity
                        .Include(a => a.EmployeeSubTypeEntity)
                        .Include(b => b.EmployeeSubTypeEntity.EmployeeType)
                        .Include(c => c.PositionEntity)
                        .Include(d => d.DepartmentEntity)
                        .Include(e => e.CampusEntity)
                        .Include(f => f.EmergencyContactEntity)
                        .Include(g => g.GovIdsEntity)
                        .Where(x => x.Person_Type == "E" && x.ToDisplay == true)
                        .OrderBy(e => e.ID_Number).ToListAsync();
                }
                else if (keyword.Contains(","))
                {
                    string[] fullname = keyword.Split(",");
                    result.employees = await _context.PersonEntity
                        .Include(a => a.EmployeeSubTypeEntity)
                        .Include(b => b.EmployeeSubTypeEntity.EmployeeType)
                        .Include(c => c.PositionEntity)
                        .Include(d => d.DepartmentEntity)
                        .Include(e => e.CampusEntity)
                        .Include(f => f.EmergencyContactEntity)
                        .Include(g => g.GovIdsEntity)
                        .Where(x => x.Person_Type == "E" && x.ToDisplay == true && (x.ID_Number.Contains(keyword)
                            || (x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim()))
                            || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
                            || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                            || x.PositionEntity.Position_Name.Contains(keyword)))
                        .OrderBy(e => e.ID_Number).ToListAsync();
                }
                else
                {
                    result.employees = await _context.PersonEntity
                        .Include(a => a.EmployeeSubTypeEntity)
                        .Include(b => b.EmployeeSubTypeEntity.EmployeeType)
                        .Include(c => c.PositionEntity)
                        .Include(d => d.DepartmentEntity)
                        .Include(e => e.CampusEntity)
                        .Include(f => f.EmergencyContactEntity)
                        .Include(g => g.GovIdsEntity)
                        .Where(x => x.Person_Type == "E" && x.ToDisplay == true && (x.ID_Number.Contains(keyword)
                                || x.First_Name.Contains(keyword)
                                || x.Last_Name.Contains(keyword)
                                || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
                                || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                                || x.PositionEntity.Position_Name.Contains(keyword)))
                        .OrderBy(e => e.ID_Number).ToListAsync();
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<visitorPagedResult> GetAllVisitors(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new visitorPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = _context.PersonEntity.Where(x => x.Person_Type == "V" && x.IsActive == true).Count();
                else
                    result.RowCount = _context.PersonEntity
                        .Where(x => x.Person_Type == "V" && x.IsActive == true && (x.ID_Number.Contains(keyword)
                            || x.First_Name.Contains(keyword)
                            || x.Last_Name.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.personvisitors = await _context.PersonEntity.Where(x => x.Person_Type == "V" && x.IsActive == true)
                        .OrderByDescending(e => e.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.personvisitors = await _context.PersonEntity
                        .Where(x => x.Person_Type == "V" && x.IsActive == true && (x.ID_Number.Contains(keyword)
                            || x.First_Name.Contains(keyword)
                            || x.Last_Name.Contains(keyword)))
                        .OrderByDescending(e => e.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<fetcherPagedResult> GetAllFetchers(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new fetcherPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = _context.PersonEntity
                        .Include(a => a.CampusEntity)
                        .Where(x => x.Person_Type == "F" && x.ToDisplay == true).Count();
                else
                    result.RowCount = _context.PersonEntity
                        .Include(a => a.CampusEntity)
                        .Where(x => x.Person_Type == "F" && x.ToDisplay == true && (x.ID_Number.Contains(keyword)
                            || x.First_Name.Contains(keyword)
                            || x.Last_Name.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.personfetchers = await _context.PersonEntity
                        .Include(a => a.CampusEntity)
                        .Where(x => x.Person_Type == "F" && x.ToDisplay == true)
                        .OrderByDescending(e => e.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.personfetchers = await _context.PersonEntity
                        .Include(a => a.CampusEntity)
                        .Where(x => x.Person_Type == "F" && x.ToDisplay == true && (x.ID_Number.Contains(keyword)
                            || x.First_Name.Contains(keyword)
                            || x.Last_Name.Contains(keyword)))
                        .OrderByDescending(e => e.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<otherAccessPagedResultVM> GetAllOtherAccesses(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var otherAccessEntity = new List<personOtherAccessVM>();
                int pageCount = 0;
                int rowCount = 0;
                using (MySqlConnection conn = new MySqlConnection(this.context.Database.GetDbConnection().ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "get_other_access_list";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pageNo", pageNo);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                        cmd.Parameters.AddWithValue("@keyword", keyword);

                        cmd.Parameters.AddWithValue("@rowCount", MySqlDbType.Int32);
                        cmd.Parameters["@rowCount"].Direction = ParameterDirection.Output;

                        cmd.Parameters.AddWithValue("@pageCount", MySqlDbType.Int32);
                        cmd.Parameters["@pageCount"].Direction = ParameterDirection.Output;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                otherAccessEntity.Add(new personOtherAccessVM()
                                {
                                    personId = Convert.ToInt32(reader["PersonID"]),
                                    idNumber = Convert.ToString(reader["IDNumber"]),
                                    campusName = Convert.ToString(reader["CampusName"]),
                                    officeName = Convert.ToString(reader["OfficeName"]),
                                    departmentName = Convert.ToString(reader["DepartmentName"]),
                                    positionName = Convert.ToString(reader["PositionName"]),
                                    firstName = Convert.ToString(reader["FirstName"]),
                                    lastName = Convert.ToString(reader["LastName"]),
                                    status = Convert.ToBoolean(reader["IsActive"]) == true ? "Active" : "Inactive"
                                });
                            }
                        }

                        rowCount = Convert.ToInt32(cmd.Parameters["@rowCount"].Value);
                        pageCount = Convert.ToInt32(cmd.Parameters["@pageCount"].Value);
                    }
                }

                otherAccessPagedResultVM otherAccessPagedResultVM = new otherAccessPagedResultVM();
                otherAccessPagedResultVM.PageCount = pageCount;
                otherAccessPagedResultVM.RowCount = rowCount;
                otherAccessPagedResultVM.personotheraccess = otherAccessEntity;
                otherAccessPagedResultVM.CurrentPage = pageNo;
                otherAccessPagedResultVM.PageSize = pageSize;

                return otherAccessPagedResultVM;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public async Task<otherAccessPagedResult> GetAllOtherAccesses(int pageNo, int pageSize, string keyword)
        //{
        //    try
        //    {
        //        var result = new otherAccessPagedResult();
        //        result.CurrentPage = pageNo;
        //        result.PageSize = pageSize;

        //        if (keyword == null || keyword == "")
        //        {
        //            result.RowCount = _context.PersonEntity
        //                .Include(y => y.OfficeEntity)
        //                .Include(x => x.PositionEntity)
        //                .Include(c => c.DepartmentEntity)
        //                .Include(v => v.CampusEntity)
        //                .Where(x => x.Person_Type == "O" && x.IsActive == true).Count();
        //        }
        //        else if (keyword.Contains(","))
        //        {
        //            string[] fullname = keyword.Split(",");

        //            result.RowCount = _context.PersonEntity
        //                .Include(y => y.OfficeEntity)
        //                .Include(x => x.PositionEntity)
        //                .Include(c => c.DepartmentEntity)
        //                .Include(v => v.CampusEntity)
        //                .Where(x => x.Person_Type == "O" && x.IsActive == true && (x.ID_Number.Contains(keyword)
        //                    || (x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim()))
        //                    || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
        //                    || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
        //                    || x.OfficeEntity.Office_Name.Contains(keyword)
        //                    || x.PositionEntity.Position_Name.Contains(keyword))).Count();
        //        }
        //        else
        //        {
        //            result.RowCount = _context.PersonEntity
        //                .Include(y => y.OfficeEntity)
        //                .Include(x => x.PositionEntity)
        //                .Include(c => c.DepartmentEntity)
        //                .Include(v => v.CampusEntity)
        //                .Where(x => x.Person_Type == "O" && x.IsActive == true && (x.ID_Number.Contains(keyword)
        //                    || x.First_Name.Contains(keyword)
        //                    || x.Last_Name.Contains(keyword)
        //                    || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
        //                    || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
        //                    || x.OfficeEntity.Office_Name.Contains(keyword)
        //                    || x.PositionEntity.Position_Name.Contains(keyword))).Count();
        //        }

        //        var pageCount = (double)result.RowCount / pageSize;
        //        result.PageCount = (int)Math.Ceiling(pageCount);

        //        var skip = (pageNo - 1) * pageSize;

        //        if (keyword == null || keyword == "")
        //        {
        //            result.personotheraccess = await _context.PersonEntity
        //                .Include(y => y.OfficeEntity)
        //                .Include(x => x.PositionEntity)
        //                .Include(c => c.DepartmentEntity)
        //                .Include(v => v.CampusEntity)
        //                .Where(x => x.Person_Type == "O" && x.IsActive == true)
        //                .OrderBy(e => e.ID_Number)
        //                .Skip(skip).Take(pageSize).ToListAsync();
        //        }
        //        else if (keyword.Contains(","))
        //        {
        //            string[] fullname = keyword.Split(",");

        //            result.personotheraccess = await _context.PersonEntity
        //                .Include(y => y.OfficeEntity)
        //                .Include(x => x.PositionEntity)
        //                .Include(c => c.DepartmentEntity)
        //                .Include(v => v.CampusEntity)
        //                .Where(x => x.Person_Type == "O" && x.IsActive == true && (x.ID_Number.Contains(keyword)
        //                    || (x.First_Name.Contains(fullname[1].Trim()) && x.Last_Name.Contains(fullname[0].Trim()))
        //                    || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
        //                    || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
        //                    || x.OfficeEntity.Office_Name.Contains(keyword)
        //                    || x.PositionEntity.Position_Name.Contains(keyword)))
        //                .OrderBy(e => e.ID_Number)
        //                .Skip(skip).Take(pageSize).ToListAsync();
        //        }
        //        else
        //        {
        //            result.personotheraccess = await _context.PersonEntity
        //                .Include(y => y.OfficeEntity)
        //                .Include(x => x.PositionEntity)
        //                .Include(c => c.DepartmentEntity)
        //                .Include(v => v.CampusEntity)
        //                .Where(x => x.Person_Type == "O" && x.IsActive == true && (x.ID_Number.Contains(keyword)
        //                    || x.First_Name.Contains(keyword)
        //                    || x.Last_Name.Contains(keyword)
        //                    || x.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)
        //                    || x.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
        //                    || x.OfficeEntity.Office_Name.Contains(keyword)
        //                    || x.PositionEntity.Position_Name.Contains(keyword)))
        //                    .OrderBy(e => e.ID_Number)
        //                    .Skip(skip).Take(pageSize).ToListAsync();
        //        }

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        public async Task<visitorPagedResult> ExportAllVisitors(string keyword)
        {
            try
            {
                var result = new visitorPagedResult();

                if (keyword == null || keyword == "")
                    result.personvisitors = await _context.PersonEntity.Where(x => x.Person_Type == "O")
                        .OrderByDescending(x => x.ID_Number).ToListAsync();
                else
                    result.personvisitors = await _context.PersonEntity
                        .Where(x => x.Person_Type == "V" && (x.ID_Number.Contains(keyword)
                            || x.First_Name.Contains(keyword)
                            || x.Last_Name.Contains(keyword)))
                        .OrderByDescending(e => e.ID_Number).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<personPagedResultVM> GetAllWithOutCardList(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new List<personVM>();
                int pageCount = 0;
                int rowCount = 0;
                using (MySqlConnection conn = new MySqlConnection(this.context.Database.GetDbConnection().ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "get_noncardholder_list_web";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pageNo", pageNo);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                        cmd.Parameters.AddWithValue("@keyword", keyword);

                        cmd.Parameters.AddWithValue("@rowCount", MySqlDbType.Int32);
                        cmd.Parameters["@rowCount"].Direction = ParameterDirection.Output;

                        cmd.Parameters.AddWithValue("@pageCount", MySqlDbType.Int32);
                        cmd.Parameters["@pageCount"].Direction = ParameterDirection.Output;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                result.Add(new personVM()
                                {
                                    personID = Convert.ToInt32(reader["PersonID"]),
                                    idNumber = reader["IDNumber"].ToString(),
                                    firstName = reader["FirstName"].ToString(),
                                    middleName = reader["MiddleName"].ToString(),
                                    lastName = reader["LastName"].ToString(),
                                    campus = reader["Campus"].ToString(),
                                    personType = reader["PersonType"].ToString()
                                });
                            }
                        }

                        rowCount = Convert.ToInt32(cmd.Parameters["@rowCount"].Value);
                        pageCount = Convert.ToInt32(cmd.Parameters["@pageCount"].Value);
                    }
                }

                personPagedResultVM personPagedResultVM = new personPagedResultVM();
                personPagedResultVM.PageCount = pageCount;
                personPagedResultVM.RowCount = rowCount;
                personPagedResultVM.people = result;
                personPagedResultVM.CurrentPage = pageNo;
                personPagedResultVM.PageSize = pageSize;

                return personPagedResultVM;

            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<personPagedResult> GetAllWithCardList(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new personPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                //if (keyword == null || keyword == "")
                //    result.RowCount = (_context.PersonEntity
                //        .Include(a => a.CardDetailsEntity)
                //        .Where(a => (a.CardDetailsEntity.Where(c => c.IsActive).Select(n => n.Person_ID).Contains(a.Person_ID))
                //        ).Where(b => b.UserEntity == null || (b.UserEntity.UserRoleEntity.Role_ID != 1 && b.UserEntity.UserRoleEntity.Role_ID != 3))).Count();
                //else
                //    result.RowCount = (_context.PersonEntity
                //        .Include(a => a.CardDetailsEntity)
                //        .Where(a => (a.CardDetailsEntity.Where(c => c.IsActive).Select(n => n.Person_ID).Contains(a.Person_ID))
                //        && (string.Concat(a.First_Name, " ", a.Last_Name).Trim().ToLower().Contains(keyword.ToLower()) || string.Concat(a.Last_Name, " ", a.First_Name).Trim().ToLower().Contains(keyword.ToLower()) || a.ID_Number.ToLower().Contains(keyword.ToLower())
                //        || (a.Person_Type.Equals("S") ? "student" : a.Person_Type.Equals("E") ? "employee" : "visitor").Contains(keyword.ToLower()))
                //        ).Where(b => b.UserEntity == null || (b.UserEntity.UserRoleEntity.Role_ID != 1 && b.UserEntity.UserRoleEntity.Role_ID != 3))).Count();

                if (keyword == null || keyword == "")
                    result.RowCount = (_context.PersonEntity
                        .Include(a => a.CardDetailsEntity)
                        .Where(a => (a.CardDetailsEntity.Where(c => c.IsActive).Select(n => n.Person_ID).Contains(a.Person_ID))
                        ).Where(b => b.UserEntity == null)).Count();
                else
                    result.RowCount = (_context.PersonEntity
                        .Include(a => a.CardDetailsEntity)
                        .Where(a => (a.CardDetailsEntity.Where(c => c.IsActive).Select(n => n.Person_ID).Contains(a.Person_ID))
                        && (string.Concat(a.First_Name, " ", a.Last_Name).Trim().ToLower().Contains(keyword.ToLower()) || string.Concat(a.Last_Name, " ", a.First_Name).Trim().ToLower().Contains(keyword.ToLower()) || a.ID_Number.ToLower().Contains(keyword.ToLower())
                        || (a.Person_Type.Equals("S") ? "student" : a.Person_Type.Equals("E") ? "employee" : "visitor").Contains(keyword.ToLower()))
                        ).Where(b => b.UserEntity == null)).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                //if (keyword == null || keyword == "")
                //    result.people = await (_context.PersonEntity
                //        .Include(a => a.CardDetailsEntity)
                //        .Where(a => (a.CardDetailsEntity.Where(c => c.IsActive).Select(n => n.Person_ID).Contains(a.Person_ID))
                //        ).Where(b => b.UserEntity == null || (b.UserEntity.UserRoleEntity.Role_ID != 1 && b.UserEntity.UserRoleEntity.Role_ID != 3)))
                //        .OrderByDescending(c => c.Person_ID)
                //        .Skip(skip).Take(pageSize).ToListAsync();
                //else
                //    result.people = await (_context.PersonEntity
                //        .Include(a => a.CardDetailsEntity)
                //        .Where(a => (a.CardDetailsEntity.Where(c => c.IsActive).Select(n => n.Person_ID).Contains(a.Person_ID))
                //        && (string.Concat(a.First_Name, " ", a.Last_Name).Trim().ToLower().Contains(keyword.ToLower()) || 
                //        string.Concat(a.Last_Name, " ", a.First_Name).Trim().ToLower().Contains(keyword.ToLower()) || 
                //        a.ID_Number.ToLower().Contains(keyword.ToLower()) ||
                //        (a.Person_Type.Equals("S") ? "student" : a.Person_Type.Equals("E") ? "employee" : "visitor").Contains(keyword.ToLower()))
                //        ).Where(b => b.UserEntity == null || (b.UserEntity.UserRoleEntity.Role_ID != 1 && b.UserEntity.UserRoleEntity.Role_ID != 3)))
                //        .OrderByDescending(c => c.Person_ID)
                //        .Skip(skip).Take(pageSize).ToListAsync();

                if (keyword == null || keyword == "")
                    result.people = await (_context.PersonEntity
                        .Include(a => a.CardDetailsEntity)
                        .Where(a => (a.CardDetailsEntity.Where(c => c.IsActive).Select(n => n.Person_ID).Contains(a.Person_ID))
                        ).Where(b => b.UserEntity == null))
                        .OrderByDescending(c => c.Person_ID)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.people = await (_context.PersonEntity
                            .Include(a => a.CardDetailsEntity)
                            .Where(a => (a.CardDetailsEntity.Where(c => c.IsActive).Select(n => n.Person_ID).Contains(a.Person_ID))
                            && (string.Concat(a.First_Name, " ", a.Last_Name).Trim().ToLower().Contains(keyword.ToLower()) ||
                            string.Concat(a.Last_Name, " ", a.First_Name).Trim().ToLower().Contains(keyword.ToLower()) ||
                            a.ID_Number.ToLower().Contains(keyword.ToLower()) ||
                            (a.Person_Type.Equals("S") ? "student" : a.Person_Type.Equals("E") ? "employee" : "visitor").Contains(keyword.ToLower()))
                            ).Where(b => b.UserEntity == null))
                            .OrderByDescending(c => c.Person_ID)
                            .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ICollection<yearSectionEntity>> CheckboxChecker(int value1, int value2)
        {
            return await _context.YearSectionEntity.Where(yearlevel => yearlevel.Level_ID == value1 && yearlevel.YearSec_ID == value2).ToListAsync();
        }

        public async Task<ICollection<studentSectionEntity>> CheckboxCheckerOne(int value1, int value2)
        {
            return await _context.StudentSectionEntity.Where(section => section.YearSec_ID == value1 && section.StudSec_ID == value2).ToListAsync();
        }

        private string RemoveSpecialChar(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public async Task<personEntity> GetFetcherById(int id)
        {
            try
            {
                return await this.context.PersonEntity
                    .Include(a => a.CampusEntity)
                    .Where(b => b.Person_ID == id && b.Person_Type == "F").SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<personEntity> GetOtherAccessById(int id)
        {
            try
            {
                return await this.context.PersonEntity
                    .Include(b => b.EmergencyContactEntity)
                    .Include(x => x.OfficeEntity)
                    .Include(x => x.PositionEntity)
                    .ThenInclude(c => c.DepartmentEntity)
                    .ThenInclude(v => v.CampusEntity)
                    .Where(b => b.Person_ID == id && b.Person_Type == "O").SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<personEntity> GetVisitorById(int id)
        {
            try
            {
                return await this.context.PersonEntity
                    .Where(b => b.Person_ID == id && b.Person_Type == "V").SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<personEntity> GetEmployeeById(int id)
        {
            try 
            {
                return await this.context.PersonEntity
                    .Include(a => a.EmployeeSubTypeEntity)
                    .Include(b => b.EmployeeSubTypeEntity.EmployeeType)
                    .Include(c => c.PositionEntity)
                    .Include(d => d.DepartmentEntity)
                    .Include(e => e.CampusEntity)
                    .Include(f => f.EmergencyContactEntity)
                    .Include(g => g.GovIdsEntity)
                    .Where(b => b.Person_ID == id && b.Person_Type == "E").SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<personEntity> GetStudentById(int id)
        {
            personEntity data = await this.context.PersonEntity.Where(h => h.Person_ID == id).SingleOrDefaultAsync();

            personEntity result = new personEntity();

            if ( data.College_ID != 0 && data.Course_ID != 0 && data.Year_Section_ID != 0 && data.StudSec_ID != 0 )
            {
                try
                {
                    result = await this.context.PersonEntity
                    .Include(a => a.CampusEntity)
                    .Include(b => b.EducationalLevelEntity)
                    .Include(c => c.CollegeEntity)
                    .Include(c => c.CourseEntity)
                    .Include(c => c.YearSectionEntity)
                    .Include(d => d.StudentSectionEntity)
                    .Include(e => e.EmergencyContactEntity)
                    .Where(h => h.Person_ID == id).SingleOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            if (data.College_ID != 0 && data.Course_ID != 0 && data.Year_Section_ID != 0 && data.StudSec_ID == 0)
            {
                try
                {
                    result = await this.context.PersonEntity
                    .Include(a => a.CampusEntity)
                    .Include(b => b.EducationalLevelEntity)
                    .Include(c => c.CollegeEntity)
                    .Include(c => c.CourseEntity)
                    .Include(c => c.YearSectionEntity)
                    .Include(e => e.EmergencyContactEntity)
                    .Where(h => h.Person_ID == id).SingleOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            if (data.College_ID != 0 && data.Course_ID != 0 && data.Year_Section_ID == 0 && data.StudSec_ID == 0)
            {
                try
                {
                    result = await this.context.PersonEntity
                    .Include(a => a.CampusEntity)
                    .Include(b => b.EducationalLevelEntity)
                    .Include(c => c.CollegeEntity)
                    .Include(c => c.CourseEntity)
                    .Include(e => e.EmergencyContactEntity)
                    .Where(h => h.Person_ID == id).SingleOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            if (data.College_ID != 0 && data.Course_ID == 0 && data.Year_Section_ID == 0 && data.StudSec_ID == 0)
            {
                try
                {
                    result = await this.context.PersonEntity
                    .Include(a => a.CampusEntity)
                    .Include(b => b.EducationalLevelEntity)
                    .Include(c => c.CollegeEntity)
                    .Include(e => e.EmergencyContactEntity)
                    .Where(h => h.Person_ID == id).SingleOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            if (data.College_ID == 0 && data.Course_ID == 0 && data.Year_Section_ID == 0 && data.StudSec_ID == 0)
            {
                try
                {
                    result = await this.context.PersonEntity
                    .Include(a => a.CampusEntity)
                    .Include(b => b.EducationalLevelEntity)
                    .Include(e => e.EmergencyContactEntity)
                    .Where(h => h.Person_ID == id).SingleOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            if (data.College_ID == 0 && data.Course_ID == 0 && data.Year_Section_ID != 0 && data.StudSec_ID != 0)
            {
                try
                {
                    result = await this.context.PersonEntity
                    .Include(a => a.CampusEntity)
                    .Include(b => b.EducationalLevelEntity)
                    .Include(c => c.YearSectionEntity)
                    .Include(d => d.StudentSectionEntity)
                    .Include(e => e.EmergencyContactEntity)
                    .Where(h => h.Person_ID == id).SingleOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            if (data.College_ID == 0 && data.Course_ID == 0 && data.Year_Section_ID != 0 && data.StudSec_ID == 0)
            {
                try
                {
                    result = await this.context.PersonEntity
                    .Include(a => a.CampusEntity)
                    .Include(b => b.EducationalLevelEntity)
                    .Include(c => c.YearSectionEntity)
                    .Include(e => e.EmergencyContactEntity)
                    .Where(h => h.Person_ID == id).SingleOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            if (result.DropOutCode == null || result.DropOutCode == string.Empty)
            {
                result.DropOutType = string.Empty;
            }
            else
            {
                var dropoutEntity = await context.DropoutCodeEntity.Where(x => x.Description == result.DropOutCode).FirstOrDefaultAsync();
                result.DropOutType = dropoutEntity.Name;
            }

            return result;
            
            //{
            //    var studentEntity = new personEntity();
            //    int pageCount = 0;
            //    int rowCount = 0;
            //    using (MySqlConnection conn = new MySqlConnection(this.context.Database.GetDbConnection().ConnectionString))
            //    {
            //        conn.Open();
            //        using (MySqlCommand cmd = new MySqlCommand())
            //        {
            //            cmd.Connection = conn;
            //            cmd.CommandText = "get_student_by_id";
            //            cmd.CommandType = CommandType.StoredProcedure;

            //            cmd.Parameters.AddWithValue("@id", id);

            //            using (var reader = await cmd.ExecuteReaderAsync())
            //            {
            //                while (reader.Read())
            //                {
            //                    studentEntity.Person_ID = Convert.ToInt32(reader["PersonID"]);
            //                    studentEntity.ID_Number = Convert.ToString(reader["IDNumber"]);
            //                    studentEntity.Campus_ID = Convert.ToInt32(reader["CampusID"]);
            //                    studentEntity.Educ_Level_ID = Convert.ToInt32(reader["LevelName"]);
            //                    studentEntity.College_ID = Convert.ToInt32(reader["CollegeName"]);
            //                    studentEntity.Course_ID = Convert.ToInt32(reader["CourseName"]);
            //                    studentEntity.StudSec_ID = Convert.ToInt32(reader["CourseName"]);
            //                    studentEntity.First_Name = Convert.ToString(reader["FirstName"]);
            //                    studentEntity.Last_Name = Convert.ToString(reader["LastName"]);
            //                    studentEntity.Year_Section_ID = Convert.ToInt32(reader["YearSectionID"]);
            //                    studentEntity.StudSec_ID = Convert.ToInt32(reader["StudSecID"]);
            //                    studentEntity.emere = Convert.ToInt32(reader["StudSecID"]);
            //                }
            //            }
            //        }
            //    }

            //    return studentEntity;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}




        }

        public async Task<excusedStudentEntity> GetExcuseById(int id)
        {
            try
            {
                return await this.context.ExcusedStudentEntity
                    .Where(b => b.ID == id).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<excusedStudentEntity> GetExcuse(string idNumber, string excusedDate)
        {
            try
            {
                return await this.context.ExcusedStudentEntity
                    .Where(b => b.IDNumber == idNumber 
                        && String.Format(b.Excused_Date.ToString(), "yyyyMMdd") == String.Format(excusedDate, "yyyyMMdd") 
                        && b.IsActive == true && b.ToDisplay == true)
                     .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task AddExcuse(excusedStudentEntity excuse, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    excuse.IDNumber = excuse.IDNumber;
                    excuse.Excused_Date = excuse.Excused_Date;

                    excuse.Added_By = user;
                    excuse.Date_Time_Added = DateTime.Now;

                    excuse.Updated_By = user;
                    excuse.Last_Updated = DateTime.Now;

                    excuse.IsActive = true;
                    excuse.ToDisplay = true;

                    await _context.ExcusedStudentEntity.AddAsync(excuse);

                    await _context.SaveChangesAsync();

                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task DeleteExcuse(excusedStudentEntity excuse, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.ExcusedStudentEntity.SingleOrDefault(x => x.ID == excuse.ID);

                    if (result != null)
                    {
                        result.IsActive = false;
                        result.ToDisplay = false;
                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    throw ex;
                }
            }
        }

        public IQueryable<dropoutCodeEntity> GetDropOutCode()
        {
            try
            {
                return _context.DropoutCodeEntity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IQueryable<personEntity> GetFetcherStudents()
        {
            try
            {
                return _context.PersonEntity
                    .Where(x => x.Person_Type == "S" && x.IsActive == true && x.ToDisplay == true);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IQueryable<personEntity> GetEmergencyLogoutStudents()
        {
            try
            {
                return _context.PersonEntity
                    .Where(x => x.Person_Type == "S" && x.IsActive == true && x.ToDisplay == true);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IQueryable<personEntity> GetReportPersonList(string type)
        {
            try
            {
                if (type == "ALL")
                {
                    return _context.PersonEntity
                        .Where(x => x.IsActive == true && x.ToDisplay == true && x.Person_ID != 1 && x.Person_ID != 2);
                }
                else
                {
                    return _context.PersonEntity
                        .Where(x => x.Person_Type == type && x.IsActive == true && x.ToDisplay == true && x.Person_ID != 1 && x.Person_ID != 2);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}