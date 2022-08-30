using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCampusV2.Models;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Common.ViewModels.ReportViewModel;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Models.Reports;

namespace MyCampusV2.Services.IServices
{
    public interface IReportService
    {
        Task<detailsPerPersonPagedResult> GetDetailsPerPersonByID(string idNumber, string date);

        #region Time and Attendance

        Task<timeAndAttendanceEmployeePagedResult> GetAllTimeAttendanceEmployee(int campusId, int employeeTypeId, int employeeSubTypeId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<timeAndAttendanceStudentPagedResult> GetAllTimeAttendanceStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<timeAndAttendanceOtherAccessPagedResult> GetAllTimeAttendanceOtherAccess(int campusId, int officeId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<timeAndAttendanceFetcherPagedResult> GetAllTimeAttendanceFetcher(int campusId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<timeAndAttendanceAllPagedResult> GetAllTimeAttendanceAll(int campusId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<timeAndAttendanceVisitorPagedResult> GetAllTimeAttendanceVisitor(string logstat, string keyword, long terminalid, long areaid, long campusid, DateTime from, DateTime to, int pageNo, int pageSize);

        Task<timeAndAttendanceAllPagedResult> ExportTimeAttendanceAll(int campusId, int personId, string reportType, DateTime from, DateTime to);
        Task<timeAndAttendanceStudentPagedResult> ExportTimeAttendanceStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string reportType, DateTime from, DateTime to);
        Task<timeAndAttendanceEmployeePagedResult> ExportTimeAttendanceEmployee(int campusId, int employeeTypeId, int employeeSubTypeId, int personId, string reportType, DateTime from, DateTime to);
        Task<timeAndAttendanceOtherAccessPagedResult> ExportTimeAttendanceOtherAccess(int campusId, int officeId, int personId, string reportType, DateTime from, DateTime to);
        Task<timeAndAttendanceFetcherPagedResult> ExportTimeAttendanceFetcher(int campusId, int personId, string reportType, DateTime from, DateTime to);

        #endregion

        #region Alarm

        Task<alarmEmployeePagedResult> GetAllAlarmEmployee(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<alarmStudentPagedResult> GetAllAlarmStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<alarmOtherAccessPagedResult> GetAllAlarmOtherAccess(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<alarmFetcherPagedResult> GetAllAlarmFetcher(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<alarmAllPagedResult> GetAllAlarmAll(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize);

        Task<alarmAllPagedResult> ExportAlarmAll(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to);
        Task<alarmStudentPagedResult> ExportAlarmStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string persontype, string logMessage, DateTime from, DateTime to);
        Task<alarmEmployeePagedResult> ExportAlarmEmployee(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to);
        Task<alarmOtherAccessPagedResult> ExportAlarmOtherAccess(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to);
        Task<alarmFetcherPagedResult> ExportAlarmFetcher(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to);

        #endregion

        #region Visitor

        Task<visitorReportPagedResult> GetAllVisitorReport(int personId, DateTime from, DateTime to, int pageNo, int pageSize);

        Task<visitorReportPagedResult> ExportAllVisitorReport(int personId, DateTime from, DateTime to);

        #endregion

        #region Library Attendance

        Task<libraryAttendanceAllPagedResult> GetAllLibraryAttendanceAll(int campusId, int personId, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<libraryAttendanceEmployeePagedResult> GetAllLibraryAttendanceEmployee(int campusId, int departmentId, int positionId, int personId, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<libraryAttendanceStudentPagedResult> GetAllLibraryAttendanceStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, DateTime from, DateTime to, int pageNo, int pageSize);

        Task<libraryAttendanceAllPagedResult> ExportLibraryAttendanceAll(int campusId, int personId, DateTime from, DateTime to);
        Task<libraryAttendanceStudentPagedResult> ExportLibraryAttendanceStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, DateTime from, DateTime to);
        Task<libraryAttendanceEmployeePagedResult> ExportLibraryAttendanceEmployee(int campusId, int departmentId, int positionId, int personId, DateTime from, DateTime to);

        #endregion

        #region Library Usage

        Task<libraryUsageAllPagedResult> GetAllLibraryUsageAll(int campusId, string persontype, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<libraryUsageEmployeePagedResult> GetAllLibraryUsageEmployee(int campusId, string persontype, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<libraryUsageStudentPagedResult> GetAllLibraryUsageStudent(int campusId, string persontype, DateTime from, DateTime to, int pageNo, int pageSize);

        Task<libraryUsageAllPagedResult> ExportLibraryUsageAll(int campusId, string persontype, DateTime from, DateTime to);
        Task<libraryUsageStudentPagedResult> ExportLibraryUsageStudent(int campusId, string persontype, DateTime from, DateTime to);
        Task<libraryUsageEmployeePagedResult> ExportLibraryUsageEmployee(int campusId, string persontype, DateTime from, DateTime to);

        #endregion  

        #region Card List

        Task<cardEmployeePagedResult> GetAllCardEmployee(int campusId, int departmentId, int positionId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<cardStudentPagedResult> GetAllCardStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<cardOtherAccessPagedResult> GetAllCardOtherAccess(int campusId, int departmentId, int positionId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<cardFetcherPagedResult> GetAllCardFetcher(int campusId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<cardAllPagedResult> GetAllCardAll(int campusId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize);

        #endregion

        #region Card Export

        Task<cardAllPagedResult> ExportCardAll(int campusId, int personId, string statustype, DateTime from, DateTime to);
        Task<cardStudentPagedResult> ExportCardStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string statustype, DateTime from, DateTime to);
        Task<cardEmployeePagedResult> ExportCardEmployee(int campusId, int departmentId, int positionId, int personId, string statustype, DateTime from, DateTime to);
        Task<cardOtherAccessPagedResult> ExportCardOtherAccess(int campusId, int departmentId, int positionId, int personId, string statustype, DateTime from, DateTime to);
        Task<cardFetcherPagedResult> ExportCardFetcher(int campusId, int personId, string statustype, DateTime from, DateTime to);

        #endregion

        #region Audit Trail

        Task<auditTrailPagedResult> GetAllAuditTrail(int user, string status, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<auditTrailPagedResult> ExportAuditTrail(int user, string status, DateTime from, DateTime to);

        #endregion

        /* ------------------------------------------------------------------------------------------------------------- */

        Task<campusEntity> GetReportCampusByID(int id);

        Task<List<ScheduleVM>> GetSchedule(string schoolYear, string month, int sectionId);
        Task<DepedReportHeaderVM> GetHeaders(string schoolYear, string month, int sectionId);
        Task<List<RecordsVM>> GetAttendance(List<ScheduleVM> schedule, string schoolYear, string month, int sectionId);
        Task<ICollection<TimeAttendanceVisitorEntity>> GetAllTimeAttendanceVisitor(timeAttendanceVisitorFilter filter);
        Task<ICollection<AlarmEmployeeEntity>> GetAllAlarmEmployee(alarmEmployeeFilter emp);
        Task<ICollection<AlarmEntity>> GetAllAlarmStudent(alarmFilter filter);
        Task<cardReportPagedResult> GetAllCardReport(string persontype,
            string keyword,
            string cardstatus,
            DateTime from,
            DateTime to,
            int pageNo,
            int pageSize);

        Task<cardReportPagedResult> ExportCardReport(string persontype, string keyword, string cardstatus, DateTime from, DateTime to);
        Task<ICollection<AlarmVisitorEntity>> GetAllAlarmVisitor(alarmVisitorFilter visitor);
        Task<ICollection<userEntity>> GetAuditTrailUserList();
        Task<ICollection<formEntity>> GetAllForms();
        Task<ICollection<CardEntity>> GetAllCardReport(cardFilterVM filter);
        Task<AlarmVisitorPagedResult> GetAllAlarmVisitor(string logstat, string keyword, long terminalid, long areaid, long campusid, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<AlarmVisitorPagedResult> ExportAllAlarmVisitor(string logstat, string keyword, long terminalid, long areaid, long campusid, DateTime from, DateTime to);

        Task<totalHoursOfEmployeePagedResult> GetTotalHoursOfEmployee(string emptype, string keyword, string departmentid, DateTime from, DateTime to, int pageNo, int pageSize);
        Task<totalHoursOfEmployeePagedResult> ExportTotalHoursOfEmployee(string emptype, string keyword, string departmentid, DateTime from, DateTime to);

        Task<timeAndAttendanceVisitorPagedResult> ExportTimeAndAttendanceVisitor(string logstat,
                string keyword,
                long terminalid,
                long areaid,
                long campusid,
                DateTime from,
                DateTime to);

        Task<timeAndAttendanceStudentPagedResult> ExportTimeAndAttendanceStudent(string logstat,
                string keyword,
                long terminalid,
                long areaid,
                long campusid,
                DateTime from,
                DateTime to);
    }
}
