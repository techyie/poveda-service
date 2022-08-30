using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Common.ViewModels;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IPersonRepository : IBaseRepository<personEntity>
    {
        IQueryable<personEntity> GetAllWithOutCard();
        Task<personPagedResultVM> GetAllWithOutCardList(int pageNo, int pageSize, string keyword);
        Task<personPagedResult> GetAllWithCardList(int pageNo, int pageSize, string keyword);
        IQueryable<personEntity> GetAllWithCard();

        IQueryable<personEntity> GetAllEmployee();
        IQueryable<personEntity> GetAllStudent();
        IQueryable<personEntity> GetAllFetcher();
        IQueryable<personEntity> GetAllOtherAccess();

        IQueryable<personEntity> GetAllPersonVisitor();

        Task<ResultModel> AddOtherAccess(personEntity person, emergencyContactEntity emergency);
        Task<ResultModel> UpdateOtherAccess(personEntity person, emergencyContactEntity emergency);

        Task<ResultModel> AddVisitor(personEntity person);
        Task<ResultModel> UpdateVisitor(personEntity person);

        Task<ResultModel> AddFetcher(personEntity person);
        Task<ResultModel> UpdateFetcher(personEntity person);

        Task<ResultModel> AddEmployee(emergencyContactEntity emergencyContact, govIdsEntity govIds, int user);
        Task<ResultModel> UpdateEmployee(emergencyContactEntity emergencyContact, govIdsEntity govIds, int user);
        Task AddStudent(emergencyContactEntity emergencyContact, int user);
        Task UpdateStudent(emergencyContactEntity emergencyContact, int user);

        Task AddPersonVisitor(personEntity personInfo, int user);
        Task UpdatePersonVisitor(personEntity personInfo, int user);

        Task<Boolean> UpdatePersonVisitorWithBoolReturn(personEntity personInfo, int user);

        Task<Boolean> AddStudentWithBoolReturn(personEntity personInfo, emergencyContactEntity emergencyInfo, int user);
        Task<Boolean> AddEmployeeWithBoolReturn(personEntity personInfo, emergencyContactEntity emergencyInfo, govIdsEntity govInfo, int user);
        Task<Boolean> AddFetcherWithBoolReturn(personEntity personInfo, int user);
        Task<Boolean> AddOtherAccessWithBoolReturn(personEntity personInfo, emergencyContactEntity emergencyInfo, int user);

        Task<Boolean> UpdateStudentWithBoolReturn(personEntity personInfo, emergencyContactEntity emergencyInfo, int user);
        Task<Boolean> UpdateEmployeeWithBoolReturn(personEntity personInfo, emergencyContactEntity emergencyInfo, govIdsEntity govInfo, int user);
        Task<Boolean> UpdateFetcherWithBoolReturn(personEntity personInfo, int user);
        Task<Boolean> UpdateOtherAccessWithBoolReturn(personEntity personInfo, emergencyContactEntity emergencyInfo, int user);

        Task<studentPagedResultVM> GetAllStudents(int pageNo, int pageSize, string keyword);
        Task<employeePagedResult> GetAllEmployees(int pageNo, int pageSize, string keyword);
        Task<visitorPagedResult> GetAllVisitors(int pageNo, int pageSize, string keyword);
        Task<fetcherPagedResult> GetAllFetchers(int pageNo, int pageSize, string keyword);
        Task<otherAccessPagedResultVM> GetAllOtherAccesses(int pageNo, int pageSize, string keyword);

        Task<studentPagedResult> ExportAllStudents(string keyword, bool isCollege);
        Task<visitorPagedResult> ExportAllVisitors(string keyword);
        Task<employeePagedResult> ExportAllEmployees(string keyword);
        Task<fetcherPagedResult> ExportAllFetchers(string keyword);
        Task<otherAccessPagedResult> ExportAllOtherAccess(string keyword);

        Task<ICollection<yearSectionEntity>> CheckboxChecker(int value1, int value2);
        Task<ICollection<studentSectionEntity>> CheckboxCheckerOne(int value1, int value2);

        Task<personEntity> GetFetcherById(int id);
        Task<personEntity> GetOtherAccessById(int id);
        Task<personEntity> GetVisitorById(int id);
        Task<personEntity> GetEmployeeById(int id);
        Task<personEntity> GetStudentById(int id);
        Task<excusedStudentEntity> GetExcuseById(int id);
        Task<excusedStudentEntity> GetExcuse(string idNumber, string excusedDate);
        Task<excusePagedResult> GetAllExcuses(string id, int pageNo, int pageSize, string keyword);
        Task AddExcuse(excusedStudentEntity excuse, int user);
        Task DeleteExcuse(excusedStudentEntity excuse, int user);
        IQueryable<dropoutCodeEntity> GetDropOutCode();
        IQueryable<personEntity> GetFetcherStudents();
        IQueryable<personEntity> GetEmergencyLogoutStudents();
        IQueryable<personEntity> GetReportPersonList(string type);
    }
}
