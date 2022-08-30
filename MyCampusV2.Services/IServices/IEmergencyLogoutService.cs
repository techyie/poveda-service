using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.IServices
{
    public interface IEmergencyLogoutService
    {
        Task<emergencyLogoutEntity> GetEmergencyLogoutById(int id);
        Task<emergencyLogoutPagedResult> GetAll(int pageNo, int pageSize, string keyword);
        Task<ResultModel> AddEmergencyLogout(emergencyLogoutEntity entity);
        Task<ResultModel> UpdateEmergencyLogout(emergencyLogoutEntity entity);
        Task<ResultModel> DeleteEmergencyLogoutPermanent(int id, int user);
        Task<ResultModel> DeleteEmergencyLogoutTemporary(int id, int user);
        Task<ResultModel> RetrieveEmergencyLogout(int id, int user);
        Task<emergencyLogoutPagedResult> ExportEmergencyLogoutStudentsExcelFile(int campusId, int educLevelId, int yearSecId, int studSecId);
        Task<BatchUploadResponse> BatchUpload(ICollection<emergencyLogoutBatchUploadVM> emergencyLogouts, int user, int uploadID, int row);
        Task<studentListPagedResult> GetStudentList(int campusId, int educLevelId, int yearSecId, int studSecId);
        Task<ResultModel> CreateByBatch(string[] studentList, string studentRemarks, string studentEffectiveDate, int user);
    }
}
