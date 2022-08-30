using System;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IEmergencyLogoutRepository : IBaseRepository<emergencyLogoutEntity>
    {
        Task<emergencyLogoutEntity> GetEmergencyLogoutById(int id);
        Task<emergencyLogoutPagedResult> GetAll(int pageNo, int pageSize, string keyword);
        Task<emergencyLogoutPagedResult> ExportEmergencyLogoutStudentsExcelFile(int campusId, int educLevelId, int yearSecId, int studSecId);
        Task<studentListPagedResult> GetStudentList(int campusId, int educLevelId, int yearSecId, int studSecId);
        Task<ResultModel> EmergencyLogout(string[] studentList, string studentRemarks, string studentEffectiveDate, int user);
    }
}
