using Microsoft.EntityFrameworkCore;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.Models;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.IServices;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels.ReportViewModel;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Models.Reports;

namespace MyCampusV2.Services.Services
{
    public class ReportService : BaseService, IReportService
    {
        public ReportService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<campusEntity> GetReportCampusByID(int id)
        {
            return await _unitOfWork.CampusRepository.GetCampusByID(id);
        }

        public async Task<List<ScheduleVM>> GetSchedule(string schoolYear, string month, int sectionId)
        {
            return await _unitOfWork.DepedReportRepository.GetSchedule(schoolYear, month, sectionId);
        }

        public async Task<DepedReportHeaderVM> GetHeaders(string schoolYear, string month, int sectionId)
        {
            return await _unitOfWork.DepedReportRepository.GetHeaders(schoolYear, month, sectionId);
        }

        public async Task<List<RecordsVM>> GetAttendance(List<ScheduleVM> schedule, string schoolYear, string month, int sectionId)
        {
            return await _unitOfWork.DepedReportRepository.GetAttendance(schedule, schoolYear, month, sectionId);
        }

        public async Task<IList<studentSectionEntity>> GetSectionList(int id)
        {
            return await _unitOfWork.StudentSectionRepository.GetStudentSectionsUsingYearSectionId(id);
        }

        public async Task<ICollection<formEntity>> GetAllForms()
        {
            return await _unitOfWork.ReportRepository.GetAllForms().Where(q => q.Searchable == true).ToListAsync();
        }

        public async Task<detailsPerPersonPagedResult> GetDetailsPerPersonByID(string idNumber, string date)
        {
            return await _unitOfWork.ReportRepository.GetDetailsPerPersonByID(idNumber, date);
        }

        #region Time and Attendance List

        public async Task<timeAndAttendanceEmployeePagedResult> GetAllTimeAttendanceEmployee(int campusId, int employeeTypeId, int employeeSubTypeId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllTimeAttendanceEmployee(campusId, employeeTypeId, employeeSubTypeId, personId, reportType, from, to, pageNo, pageSize);
        }

        public async Task<timeAndAttendanceStudentPagedResult> GetAllTimeAttendanceStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllTimeAttendanceStudent(campusId, educLevelId, yearSecId, studSecId, personId, reportType, from, to, pageNo, pageSize);
        }

        public async Task<timeAndAttendanceOtherAccessPagedResult> GetAllTimeAttendanceOtherAccess(int campusId, int officeId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllTimeAttendanceOtherAccess(campusId, officeId, personId, reportType, from, to, pageNo, pageSize);
        }

        public async Task<timeAndAttendanceFetcherPagedResult> GetAllTimeAttendanceFetcher(int campusId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllTimeAttendanceFetcher(campusId, personId, reportType, from, to, pageNo, pageSize);
        }

        public async Task<timeAndAttendanceAllPagedResult> GetAllTimeAttendanceAll(int campusId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllTimeAttendanceAll(campusId, personId, reportType, from, to, pageNo, pageSize);
        }

        #endregion

        #region Time and Attendance Export

        public async Task<timeAndAttendanceAllPagedResult> ExportTimeAttendanceAll(int campusId, int personId, string reportType, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportTimeAttendanceAll(campusId, personId, reportType, from, to);
        }

        public async Task<timeAndAttendanceStudentPagedResult> ExportTimeAttendanceStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string reportType, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportTimeAttendanceStudent(campusId, educLevelId, yearSecId, studSecId, personId, reportType, from, to);
        }

        public async Task<timeAndAttendanceEmployeePagedResult> ExportTimeAttendanceEmployee(int campusId, int employeeTypeId, int employeeSubTypeId, int personId, string reportType, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportTimeAttendanceEmployee(campusId, employeeTypeId, employeeSubTypeId, personId, reportType, from, to);
        }

        public async Task<timeAndAttendanceOtherAccessPagedResult> ExportTimeAttendanceOtherAccess(int campusId, int officeId, int personId, string reportType, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportTimeAttendanceOtherAccess(campusId, officeId, personId, reportType, from, to);
        }

        public async Task<timeAndAttendanceFetcherPagedResult> ExportTimeAttendanceFetcher(int campusId, int personId, string reportType, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportTimeAttendanceFetcher(campusId, personId, reportType, from, to);
        }

        #endregion

        #region Alarm List

        public async Task<alarmEmployeePagedResult> GetAllAlarmEmployee(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllAlarmEmployee(campusId, personId, persontype, logMessage, from, to, pageNo, pageSize);
        }

        public async Task<alarmStudentPagedResult> GetAllAlarmStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllAlarmStudent(campusId, educLevelId, yearSecId, studSecId, personId, persontype, logMessage, from, to, pageNo, pageSize);
        }

        public async Task<alarmOtherAccessPagedResult> GetAllAlarmOtherAccess(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllAlarmOtherAccess(campusId, personId, persontype, logMessage, from, to, pageNo, pageSize);
        }

        public async Task<alarmFetcherPagedResult> GetAllAlarmFetcher(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllAlarmFetcher(campusId, personId, persontype, logMessage, from, to, pageNo, pageSize);
        }

        public async Task<alarmAllPagedResult> GetAllAlarmAll(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllAlarmAll(campusId, personId, persontype, logMessage, from, to, pageNo, pageSize);
        }

        #endregion

        #region Alarm Export

        public async Task<alarmAllPagedResult> ExportAlarmAll(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportAlarmAll(campusId, personId, persontype, logMessage, from, to);
        }

        public async Task<alarmStudentPagedResult> ExportAlarmStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string persontype, string logMessage, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportAlarmStudent(campusId, educLevelId, yearSecId, studSecId, personId, persontype, logMessage, from, to);
        }

        public async Task<alarmEmployeePagedResult> ExportAlarmEmployee(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportAlarmEmployee(campusId, personId, persontype, logMessage, from, to);
        }

        public async Task<alarmOtherAccessPagedResult> ExportAlarmOtherAccess(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportAlarmOtherAccess(campusId, personId, persontype, logMessage, from, to);
        }

        public async Task<alarmFetcherPagedResult> ExportAlarmFetcher(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportAlarmFetcher(campusId, personId, persontype, logMessage, from, to);
        }

        #endregion

        #region Visitor

        public async Task<visitorReportPagedResult> GetAllVisitorReport(int personId, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllVisitorReport(personId, from, to, pageNo, pageSize);
        }

        public async Task<visitorReportPagedResult> ExportAllVisitorReport(int personId, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportAllVisitorReport(personId, from, to);
        }

        #endregion

        #region Library Attendance List

        public async Task<libraryAttendanceAllPagedResult> GetAllLibraryAttendanceAll(int campusId, int personId, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllLibraryAttendanceAll(campusId, personId, from, to, pageNo, pageSize);
        }

        public async Task<libraryAttendanceEmployeePagedResult> GetAllLibraryAttendanceEmployee(int campusId, int departmentId, int positionId, int personId, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllLibraryAttendanceEmployee(campusId, departmentId, positionId, personId, from, to, pageNo, pageSize);
        }

        public async Task<libraryAttendanceStudentPagedResult> GetAllLibraryAttendanceStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllLibraryAttendanceStudent(campusId, educLevelId, yearSecId, studSecId, personId, from, to, pageNo, pageSize);
        }

        #endregion

        #region Library Attendance Export

        public async Task<libraryAttendanceAllPagedResult> ExportLibraryAttendanceAll(int campusId, int personId, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportLibraryAttendanceAll(campusId, personId, from, to);
        }

        public async Task<libraryAttendanceStudentPagedResult> ExportLibraryAttendanceStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportLibraryAttendanceStudent(campusId, educLevelId, yearSecId, studSecId, personId, from, to);
        }

        public async Task<libraryAttendanceEmployeePagedResult> ExportLibraryAttendanceEmployee(int campusId, int departmentId, int positionId, int personId, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportLibraryAttendanceEmployee(campusId, departmentId, positionId, personId, from, to);
        }

        #endregion

        #region Library Usage List

        public async Task<libraryUsageAllPagedResult> GetAllLibraryUsageAll(int campusId, string persontype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllLibraryUsageAll(campusId, persontype, from, to, pageNo, pageSize);
        }

        public async Task<libraryUsageEmployeePagedResult> GetAllLibraryUsageEmployee(int campusId, string persontype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllLibraryUsageEmployee(campusId, persontype, from, to, pageNo, pageSize);
        }

        public async Task<libraryUsageStudentPagedResult> GetAllLibraryUsageStudent(int campusId, string persontype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllLibraryUsageStudent(campusId, persontype, from, to, pageNo, pageSize);
        }

        #endregion

        #region Library Usage Export

        public async Task<libraryUsageAllPagedResult> ExportLibraryUsageAll(int campusId, string persontype, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportLibraryUsageAll(campusId, persontype, from, to);
        }

        public async Task<libraryUsageStudentPagedResult> ExportLibraryUsageStudent(int campusId, string persontype, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportLibraryUsageStudent(campusId, persontype, from, to);
        }

        public async Task<libraryUsageEmployeePagedResult> ExportLibraryUsageEmployee(int campusId, string persontype, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportLibraryUsageEmployee(campusId, persontype, from, to);
        }

        #endregion

        #region Card List

        public async Task<cardEmployeePagedResult> GetAllCardEmployee(int campusId, int departmentId, int positionId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllCardEmployee(campusId, departmentId, positionId, personId, statustype, from, to, pageNo, pageSize);
        }

        public async Task<cardStudentPagedResult> GetAllCardStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllCardStudent(campusId, educLevelId, yearSecId, studSecId, personId, statustype, from, to, pageNo, pageSize);
        }

        public async Task<cardOtherAccessPagedResult> GetAllCardOtherAccess(int campusId, int departmentId, int positionId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllCardOtherAccess(campusId, departmentId, positionId, personId, statustype, from, to, pageNo, pageSize);
        }

        public async Task<cardFetcherPagedResult> GetAllCardFetcher(int campusId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllCardFetcher(campusId, personId, statustype, from, to, pageNo, pageSize);
        }

        public async Task<cardAllPagedResult> GetAllCardAll(int campusId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllCardAll(campusId, personId, statustype, from, to, pageNo, pageSize);
        }

        #endregion

        #region Card Export

        public async Task<cardAllPagedResult> ExportCardAll(int campusId, int personId, string statustype, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportCardAll(campusId, personId, statustype, from, to);
        }

        public async Task<cardStudentPagedResult> ExportCardStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string statustype, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportCardStudent(campusId, educLevelId, yearSecId, studSecId, personId, statustype, from, to);
        }

        public async Task<cardEmployeePagedResult> ExportCardEmployee(int campusId, int departmentId, int positionId, int personId, string statustype, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportCardEmployee(campusId, departmentId, positionId, personId, statustype, from, to);
        }

        public async Task<cardOtherAccessPagedResult> ExportCardOtherAccess(int campusId, int departmentId, int positionId, int personId, string statustype, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportCardOtherAccess(campusId, departmentId, positionId, personId, statustype, from, to);
        }

        public async Task<cardFetcherPagedResult> ExportCardFetcher(int campusId, int personId, string statustype, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportCardFetcher(campusId, personId, statustype, from, to);
        }

        #endregion

        #region Audit Trail

        public async Task<auditTrailPagedResult> GetAllAuditTrail(int user, string status, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllAuditTrail(user, status, from, to, pageNo, pageSize);
        }

        public async Task<auditTrailPagedResult> ExportAuditTrail(int user, string status, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportAuditTrail(user, status, from, to);
        }

        #endregion

        /* ------------------------------------------------------------------------------------------------------------- */

        public async Task<ICollection<userEntity>> GetAuditTrailUserList()
        {
            return await _unitOfWork.UserRepository.GetUserList().ToListAsync();
        }

        public async Task<timeAndAttendanceVisitorPagedResult> ExportTimeAndAttendanceVisitor(string logstat,
                string keyword,
                long terminalid,
                long areaid,
                long campusid,
                DateTime from,
                DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportTimeAndAttendanceVisitor(logstat, keyword, terminalid, areaid, campusid, from, to);
        }

        public async Task<timeAndAttendanceStudentPagedResult> ExportTimeAndAttendanceStudent(string logstat,
                string keyword,
                long terminalid,
                long areaid,
                long campusid,
                DateTime from,
                DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportTimeAndAttendanceStudent(logstat, keyword, terminalid, areaid, campusid, from, to);
        }

        public async Task<timeAndAttendanceVisitorPagedResult> GetAllTimeAttendanceVisitor(string logstat, string keyword, long terminalid, long areaid, long campusid, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllTimeAttendanceVisitor(logstat, keyword, terminalid, areaid, campusid, from, to, pageNo, pageSize);
        }

        public async Task<AlarmVisitorPagedResult> ExportAllAlarmVisitor(string logstat, string keyword, long terminalid, long areaid, long campusid, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportAllAlarmVisitor(logstat, keyword, terminalid, areaid, campusid, from, to);
        }

        public async Task<AlarmVisitorPagedResult> GetAllAlarmVisitor(string logstat, string keyword, long terminalid, long areaid, long campusid, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllAlarmVisitor(logstat, keyword, terminalid, areaid, campusid, from, to, pageNo, pageSize);
        }

        public async Task<ICollection<TimeAttendanceVisitorEntity>> GetAllTimeAttendanceVisitor(timeAttendanceVisitorFilter filter)
        {
            return await _unitOfWork.ReportRepository.GetAllTimeAttendanceVisitor(filter).ToListAsync();
        }

        public async Task<ICollection<AlarmEmployeeEntity>> GetAllAlarmEmployee(alarmEmployeeFilter filter)
        {
            //return await GetDataAlarmE(_unitOfWork.ReportRepository.GetAllAlarmEmployee(emp)).ToListAsync();
            return await _unitOfWork.ReportRepository.GetAllAlarmEmployee(filter).ToListAsync();
        }

        public async Task<ICollection<AlarmEntity>> GetAllAlarmStudent(alarmFilter filter)
        {
            //return await GetDataAlarm(_unitOfWork.ReportRepository.GetAllAlarmStudent(stud)).ToListAsync();
            return await _unitOfWork.ReportRepository.GetAllAlarmStudent(filter).ToListAsync();
        }

        public async Task<ICollection<AlarmVisitorEntity>> GetAllAlarmVisitor(alarmVisitorFilter filter)
        {
            //return await GetDataAlarm(_unitOfWork.ReportRepository.GetAllAlarmStudent(stud)).ToListAsync();
            return await _unitOfWork.ReportRepository.GetAllAlarmVisitor(filter).ToListAsync();
        }

        public async Task<cardReportPagedResult> ExportCardReport(string persontype, string keyword, string cardstatus, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportCardReport(persontype, keyword, cardstatus, from, to);
        }

        public async Task<cardReportPagedResult> GetAllCardReport(string persontype,
            string keyword,
            string cardstatus,
            DateTime from,
            DateTime to,
            int pageNo,
            int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetAllCardReport(persontype,
            keyword,
            cardstatus,
            from,
            to,
            pageNo,
            pageSize);
        }

        public async Task<ICollection<CardEntity>> GetAllCardReport(cardFilterVM filter)
        {
            return await _unitOfWork.ReportRepository.GetAllCardReport(filter).ToListAsync();
        }

        public async Task<totalHoursOfEmployeePagedResult> GetTotalHoursOfEmployee(string emptype, string keyword, string departmentid, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return await _unitOfWork.ReportRepository.GetTotalHoursOfEmployee(emptype, keyword, departmentid, from, to, pageNo, pageSize);
        }

        public async Task<totalHoursOfEmployeePagedResult> ExportTotalHoursOfEmployee(string emptype, string keyword, string departmentid, DateTime from, DateTime to)
        {
            return await _unitOfWork.ReportRepository.ExportTotalHoursOfEmployee(emptype, keyword, departmentid, from, to);
        }

        #region Queryables
        private IQueryable<AuditTrail> GetDataAudit(IQueryable<AuditTrail> query)
        {
            return !_user.MasterAccess ? query.Where(o => o.Campus_ID == _user.Campus) : query;
        }
        //private IQueryable<Time_Attendance> GetDataTimeAttendance(IQueryable<Time_Attendance> query)
        //{
        //    return !_user.MasterAccess ? query.Where(o => o.Campus_ID == _user.Campus) : query;
        //}
        //private IQueryable<Time_Attendance_EmployeeVM> GetDataTimeAttendanceE(IQueryable<Time_Attendance_EmployeeVM> query)
        //{
        //    return !_user.MasterAccess ? query.Where(o => o.CampusID == _user.Campus) : query;
        //}
        //private IQueryable<Alarm_Employee> GetDataAlarmE(IQueryable<Alarm_Employee> query)
        //{
        //    return !_user.MasterAccess ? query.Where(o => o.Campus_ID == _user.Campus) : query;
        //}
        //private IQueryable<Alarm_Student> GetDataAlarm(IQueryable<Alarm_Student> query)
        //{
        //    return !_user.MasterAccess ? query.Where(o => o.Campus_ID == _user.Campus) : query;
        //}
        private IQueryable<Card_Employee> GetDataCard(IQueryable<Card_Employee> query)
        {
            return !_user.MasterAccess ? query.Where(o => o.Campus_ID == _user.Campus) : query;
        }
        private IQueryable<Visitor_Report> GetDataVisitor(IQueryable<Visitor_Report> query)
        {
            return !_user.MasterAccess ? query.Where(o => o.Campus_ID == _user.Campus) : query;
        }
        private IQueryable<Card_Student> GetDataCardE(IQueryable<Card_Student> query)
        {
            return !_user.MasterAccess ? query.Where(o => o.Campus_ID == _user.Campus) : query;
        }
        #endregion

    }
}
