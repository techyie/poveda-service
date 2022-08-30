using Microsoft.AspNetCore.Http;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Common.ViewModels.OfficeViewModel;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.IServices
{
    public interface IBatchUploadService 
    {
        Task<ICollection<batchUploadEntity>> GetAll();
        batchUploadEntity GetById(int id);
        Task<batchUploadEntity> GetByIdSync(int id);
        Task AddBatchUplaod(batchUploadEntity entity, int userID);
        Task UpdateBatchUplaod(batchUploadEntity entity, int user);
        Task DeleteBatchUplaod(int id, int user);

        Task Upload(IFormFile file, batchUploadEntity entity, int userID);
        Int32 GetRecordsCount(IFormFile file);

        IEnumerable<papAccountBatchUploadVM> GetPapAccountRecords(string path);


        IEnumerable<campusBatchUploadVM> GetCampusRecords(string path);
        IEnumerable<educLevelBatchUploadVM> GetEducLevelRecords(string path);
        IEnumerable<yearSectionBatchUploadVM> GetYearSectionRecords(string path);
        IEnumerable<studentSectionBatchUploadVM> GetSectionRecords(string path);
        IEnumerable<collegeBatchUploadVM> GetCollegeRecords(string path);
        IEnumerable<courseBatchUploadVM> GetCourseRecords(string path);
        IEnumerable<departmentBatchUploadVM> GetDepartmentRecords(string path);
        IEnumerable<officeBatchUploadVM> GetOfficeRecords(string path);
        IEnumerable<positionBatchUploadVM> GetPositionRecords(string path);
        IEnumerable<areaBatchUploadVM> GetAreaRecords(string path);
        IEnumerable<schoolCalendarBatchUploadVM> GetCalendarRecords(string path);


        IEnumerable<personStudentBatchUploadVM> GetStudentRecords(string path, int studcols, int studcollegecols);
        IEnumerable<personEmployeeBatchUploadVM> GetEmployeeRecords(string path);
        IEnumerable<visitorPersonBatchUploadVM> GetVisitorRecords(string path);
        IEnumerable<personFetcherBatchUploadVM> GetFetcherRecords(string path);
        IEnumerable<personOtherAccessBatchUploadVM> GetOtherAccessRecords(string path); 

        IEnumerable<cardBatchUpdateVM> GetCardUpdateRecords(string path);
        IEnumerable<cardBatchDeactiveVM> GetCardDeactivateRecords(string path);

        IEnumerable<notificationBatchUploadVM> GetGeneralNotifRecords(string path);
        IEnumerable<personalNotificationBatchUploadVM> GetPersonalNotifRecords(string path);

        IEnumerable<emergencyLogoutBatchUploadVM> GetEmergencyLogoutRecords(string path);
    }
}
