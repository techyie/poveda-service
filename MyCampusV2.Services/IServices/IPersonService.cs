using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCampusV2.IServices
{
    public interface IPersonService
    {
        Task<ICollection<personEntity>> GetAllWithOutCard();
        Task<personPagedResultVM> GetAllWithOutCardList(int pageNo, int pageSize, string keyword);
        Task<personPagedResult> GetAllWithCardList(int pageNo, int pageSize, string keyword);
        Task<ICollection<personEntity>> GetAllWithCard();
        Task<personEntity> GetPersonByID(long id);
        Task<personEntity> GetPersonByIDNumber(string idNumber);

        Task<ICollection<personEntity>> GetAllPersonVisitor();

        Task<personEntity> GetByPersonVisitorID(int id);

        Task<ICollection<personEntity>> GetStudentList();
        Task<ICollection<personEntity>> GetEmployeeList();
        Task<ICollection<personEntity>> GetFetcherList();
        Task<ICollection<personEntity>> GetOtherAccessList();
        Task<ResultModel> AddEmployee(personEntity employee, emergencyContactEntity emergencyContact, govIdsEntity govIds, int user);
        Task<ResultModel> UpdateEmployee(personEntity employee, emergencyContactEntity emergencyContact, govIdsEntity govIds, int user);
        Task<personEntity> GetEmployeeByIDNumber(string IDNumber);
        Task<personEntity> GetStudentByIDNumber(string idNumber);

        Task DeletePersonVisitor(long id, int user);
        Task AddPersonVisitor(personEntity personInfo, int user);
        Task UpdatePersonVisitor(personEntity personInfo, int user);

        Task<BatchUploadResponse> BatchUpload(ICollection<visitorPersonBatchUploadVM> personvisitors, int user, int uploadID, int row);
        Task<BatchUploadResponse> PersonEmployeeBatchUpload(ICollection<personEmployeeBatchUploadVM> personemployees, int user, int uploadID, int row);
        Task<BatchUploadResponse> PersonStudentBatchUpload(ICollection<personStudentBatchUploadVM> personstudents, int user,int uploadID,int row);
        Task<BatchUploadResponse> PersonFetcherBatchUpload(ICollection<personFetcherBatchUploadVM> personfetchers, int user, int uploadID, int row);
        Task<BatchUploadResponse> PersonOtherAccessBatchUpload(ICollection<personOtherAccessBatchUploadVM> personotheraccess, int user, int uploadID, int row);

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
        Task<ResultModel> AddFetcher(personEntity fetcher, int user);
        Task<ResultModel> UpdateFetcher(personEntity fetcher, int user);
        Task<ResultModel> DeleteTemporaryFetcher(int id, int user);
        Task<ResultModel> DeletePermanentFetcher(int id, int user);

        Task<personEntity> GetOtherAccessById(int id);
        Task<ResultModel> AddOtherAccess(personEntity otheraccess, emergencyContactEntity emergencyContact, int user);
        Task<ResultModel> UpdateOtherAccess(personEntity otheraccess, emergencyContactEntity emergencyContact, int user);
        Task<ResultModel> DeleteTemporaryOtherAccess(int id, int user);
        Task<ResultModel> DeletePermanentOtherAccess(int id, int user);

        Task<personEntity> GetVisitorById(int id);
        Task<ResultModel> AddVisitor(personEntity visitor, int user);
        Task<ResultModel> UpdateVisitor(personEntity visitor, int user);
        Task<ResultModel> DeleteTemporaryVisitor(int id, int user);
        Task<ResultModel> DeletePermanentVisitor(int id, int user);

        Task<personEntity> GetEmployeeById(int id);
        Task<ResultModel> AddEmployee(personEntity employee, int user);
        Task<ResultModel> UpdateEmployee(personEntity employee, int user);
        Task<ResultModel> DeleteTemporaryEmployee(int id, int user);
        Task<ResultModel> DeletePermanentEmployee(int id, int user);

        Task<personEntity> GetStudentById(int id);
        Task<ResultModel> AddStudent(personEntity student, emergencyContactEntity emergencyContact, int user);
        Task<ResultModel> UpdateStudent(personEntity student, emergencyContactEntity emergencyContact, int user);
        Task<ResultModel> DeleteTemporaryStudent(int id, int user);
        Task<ResultModel> DeletePermanentStudent(int id, int user);
        Task<excusedStudentEntity> GetExcuseById(int id);
        Task<excusedStudentEntity> GetExcuse(string idNumber, string excusedDate);
        Task<excusePagedResult> GetAllExcuses(string id, int pageNo, int pageSize, string keyword);
        Task<ResultModel> AddExcuse(excusedStudentEntity excuse, int user);
        Task<ResultModel> DeleteExcuseStudent(int id, int user);
        Task<ICollection<dropoutCodeEntity>> GetDropOutCode();
        Task<ICollection<personEntity>> GetFetcherStudents();
        Task<ICollection<personEntity>> GetEmergencyLogoutStudents();
        Task<ResultModel> UpdateStudentStatus(personEntity student, int user);
        Task<ICollection<personEntity>> GetReportPersonList(string type);
    }
}
