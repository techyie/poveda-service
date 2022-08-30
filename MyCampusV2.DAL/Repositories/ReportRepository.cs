using Microsoft.EntityFrameworkCore;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Text;
using System.Threading.Tasks;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Common.ViewModels.ReportViewModel;
using MyCampusV2.Common.Helpers;
using System.Collections.Generic;
using System.Data;

namespace MyCampusV2.DAL.Repositories
{
    public class ReportRepository : BaseReportRepository<formEntity>, IReportRepository
    {
        private readonly MyCampusCardReportContext context;

        public ReportRepository(MyCampusCardReportContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<detailsPerPersonPagedResult> GetDetailsPerPersonByID(string idNumber, string date)
        {
            try
            {
                var detailsPerPersonEntity = new List<DetailsPerPersonResultVM>();

                var initialResult = await _context.detailsPerPersonVM.FromSql
                    ("call get_details_per_person(@idNumber,@date)",
                    new MySqlParameter("@idNumber", idNumber),
                    new MySqlParameter("@date", date)).ToListAsync();

                foreach (var actualResult in initialResult)
                {
                    detailsPerPersonEntity.Add(new DetailsPerPersonResultVM()
                    {
                        ID = actualResult.ID,
                        idNumber = actualResult.idNumber,
                        fullName = actualResult.fullName,
                        logDate = actualResult.logDate,
                        logMessage = actualResult.logMessage,
                        terminalName = Encryption.Decrypt(actualResult.terminalName)
                    });
                }

                detailsPerPersonPagedResult result = new detailsPerPersonPagedResult();
                result.detailsPerPerson = detailsPerPersonEntity;

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region Time and Attendance List

        public async Task<timeAndAttendanceEmployeePagedResult> GetAllTimeAttendanceEmployee(int campusId, int employeeTypeId, int employeeSubTypeId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new timeAndAttendanceEmployeePagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Time_Attendance_Count.FromSql
                    ("call get_timeattendance_employee_report_count(@from,@to,@campusId,@employeeTypeId,@employeeSubTypeId,@personId,@reportType)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@employeeTypeId", employeeTypeId),
                        new MySqlParameter("@employeeSubTypeId", employeeSubTypeId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@reportType", reportType)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.dailylogs = await _context.timeAndAttendanceEmployeeResultVM.FromSql
                    ("call get_timeattendance_employee_report_list(@skip,@pageSize,@from,@to,@campusId,@employeeTypeId,@employeeSubTypeId,@personId,@reportType)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@employeeTypeId", employeeTypeId),
                    new MySqlParameter("@employeeSubTypeId", employeeSubTypeId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@reportType", reportType)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<timeAndAttendanceOtherAccessPagedResult> GetAllTimeAttendanceOtherAccess(int campusId, int officeId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new timeAndAttendanceOtherAccessPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Time_Attendance_Count.FromSql
                    ("call get_timeattendance_otheraccess_report_count(@from,@to,@campusId,@officeId,@personId,@reportType)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@officeId", officeId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@reportType", reportType)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.dailylogs = await _context.timeAndAttendanceOtherAccessResultVM.FromSql
                    ("call get_timeattendance_otheraccess_report_list(@skip,@pageSize,@from,@to,@campusId,@officeId,@personId,@reportType)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@officeId", officeId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@reportType", reportType)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<timeAndAttendanceFetcherPagedResult> GetAllTimeAttendanceFetcher(int campusId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new timeAndAttendanceFetcherPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Time_Attendance_Count.FromSql
                    ("call get_timeattendance_fetcher_report_count(@from,@to,@campusId,@personId,@reportType)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@reportType", reportType)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.dailylogs = await _context.timeAndAttendanceFetcherResultVM.FromSql
                    ("call get_timeattendance_fetcher_report_list(@skip,@pageSize,@from,@to,@campusId,@personId,@reportType)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@reportType", reportType)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<timeAndAttendanceAllPagedResult> GetAllTimeAttendanceAll(int campusId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new timeAndAttendanceAllPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Time_Attendance_Count.FromSql
                    ("call get_timeattendance_all_report_count(@from,@to,@campusId,@personId,@reportType)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@reportType", reportType)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.dailylogs = await _context.timeAndAttendanceAllResultVM.FromSql
                    ("call get_timeattendance_all_report_list(@skip,@pageSize,@from,@to,@campusId,@personId,@reportType)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@reportType", reportType)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<timeAndAttendanceStudentPagedResult> GetAllTimeAttendanceStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string reportType, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new timeAndAttendanceStudentPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Time_Attendance_Count.FromSql
                    ("call get_timeattendance_student_report_count(@from,@to,@campusId,@educLevelId,@yearSecId,@studSecId,@personId,@reportType)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@educLevelId", educLevelId),
                        new MySqlParameter("@yearSecId", yearSecId),
                        new MySqlParameter("@studSecId", studSecId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@reportType", reportType)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.dailylogs = await _context.timeAndAttendanceStudentResultVM.FromSql
                    ("call get_timeattendance_student_report_list(@skip,@pageSize,@from,@to,@campusId,@educLevelId,@yearSecId,@studSecId,@personId,@reportType)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@educLevelId", educLevelId),
                    new MySqlParameter("@yearSecId", yearSecId),
                    new MySqlParameter("@studSecId", studSecId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@reportType", reportType)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Time and Attendance Export

        public async Task<timeAndAttendanceAllPagedResult> ExportTimeAttendanceAll(int campusId, int personId, string reportType, DateTime from, DateTime to)
        {
            try
            {
                var result = new timeAndAttendanceAllPagedResult();

                result.dailylogs = await _context.timeAndAttendanceAllResultVM.FromSql
                    ("call get_timeattendance_all_report_export(@from,@to,@campusId,@personId,@reportType)",
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@reportType", reportType)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<timeAndAttendanceStudentPagedResult> ExportTimeAttendanceStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string reportType, DateTime from, DateTime to)
        {
            try
            {
                var result = new timeAndAttendanceStudentPagedResult();

                result.dailylogs = await _context.timeAndAttendanceStudentResultVM.FromSql
                    ("call get_timeattendance_student_report_export(@from,@to,@campusId,@educLevelId,@yearSecId,@studSecId,@personId,@reportType)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@educLevelId", educLevelId),
                        new MySqlParameter("@yearSecId", yearSecId),
                        new MySqlParameter("@studSecId", studSecId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@reportType", reportType)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<timeAndAttendanceEmployeePagedResult> ExportTimeAttendanceEmployee(int campusId, int employeeTypeId, int employeeSubTypeId, int personId, string reportType, DateTime from, DateTime to)
        {
            try
            {
                var result = new timeAndAttendanceEmployeePagedResult();

                result.dailylogs = await _context.timeAndAttendanceEmployeeResultVM.FromSql
                    ("call get_timeattendance_employee_report_export(@from,@to,@campusId,@employeeTypeId,@employeeSubTypeId,@personId,@reportType)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@employeeTypeId", employeeTypeId),
                        new MySqlParameter("@employeeSubTypeId", employeeSubTypeId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@reportType", reportType)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<timeAndAttendanceOtherAccessPagedResult> ExportTimeAttendanceOtherAccess(int campusId, int officeId, int personId, string reportType, DateTime from, DateTime to)
        {
            try
            {
                var result = new timeAndAttendanceOtherAccessPagedResult();

                result.dailylogs = await _context.timeAndAttendanceOtherAccessResultVM.FromSql
                    ("call get_timeattendance_otheraccess_report_export(@from,@to,@campusId,@officeId,@personId,@reportType)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@officeId", officeId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@reportType", reportType)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<timeAndAttendanceFetcherPagedResult> ExportTimeAttendanceFetcher(int campusId, int personId, string reportType, DateTime from, DateTime to)
        {
            try
            {
                var result = new timeAndAttendanceFetcherPagedResult();

                result.dailylogs = await _context.timeAndAttendanceFetcherResultVM.FromSql
                    ("call get_timeattendance_fetcher_report_export(@from,@to,@campusId,@personId,@reportType)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@reportType", reportType)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Alarm List

        public async Task<alarmEmployeePagedResult> GetAllAlarmEmployee(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new alarmEmployeePagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Alarm_Count.FromSql
                    ("call get_alarm_employee_report_count(@from,@to,@campusId,@personId,@logMessage)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@logMessage", logMessage)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.dailylogs = await _context.alarmEmployeeResultVM.FromSql
                    ("call get_alarm_employee_report_list(@skip,@pageSize,@from,@to,@campusId,@personId,@logMessage)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@logMessage", logMessage)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<alarmOtherAccessPagedResult> GetAllAlarmOtherAccess(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new alarmOtherAccessPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Alarm_Count.FromSql
                    ("call get_alarm_otheraccess_report_count(@from,@to,@campusId,@personId,@logMessage)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@logMessage", logMessage)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.dailylogs = await _context.alarmOtherAccessResultVM.FromSql
                    ("call get_alarm_otheraccess_report_list(@skip,@pageSize,@from,@to,@campusId,@personId,@logMessage)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@logMessage", logMessage)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<alarmFetcherPagedResult> GetAllAlarmFetcher(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new alarmFetcherPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Alarm_Count.FromSql
                    ("call get_alarm_fetcher_report_count(@from,@to,@campusId,@personId,@logMessage)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@logMessage", logMessage)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.dailylogs = await _context.alarmFetcherResultVM.FromSql
                    ("call get_alarm_fetcher_report_list(@skip,@pageSize,@from,@to,@campusId,@personId,@logMessage)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@logMessage", logMessage)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<alarmAllPagedResult> GetAllAlarmAll(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new alarmAllPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Alarm_Count.FromSql
                    ("call get_alarm_all_report_count(@from,@to,@campusId,@personId,@logMessage)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@logMessage", logMessage)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.dailylogs = await _context.alarmAllResultVM.FromSql
                    ("call get_alarm_all_report_list(@skip,@pageSize,@from,@to,@campusId,@personId,@logMessage)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@logMessage", logMessage)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<alarmStudentPagedResult> GetAllAlarmStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string persontype, string logMessage, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new alarmStudentPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Alarm_Count.FromSql
                    ("call get_alarm_student_report_count(@from,@to,@campusId,@educLevelId,@yearSecId,@studSecId,@personId,@logMessage)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@educLevelId", educLevelId),
                        new MySqlParameter("@yearSecId", yearSecId),
                        new MySqlParameter("@studSecId", studSecId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@logMessage", logMessage)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.dailylogs = await _context.alarmStudentResultVM.FromSql
                    ("call get_alarm_student_report_list(@skip,@pageSize,@from,@to,@campusId,@educLevelId,@yearSecId,@studSecId,@personId,@logMessage)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@educLevelId", educLevelId),
                    new MySqlParameter("@yearSecId", yearSecId),
                    new MySqlParameter("@studSecId", studSecId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@logMessage", logMessage)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Alarm Export

        public async Task<alarmAllPagedResult> ExportAlarmAll(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to)
        {
            try
            {
                var result = new alarmAllPagedResult();

                result.dailylogs = await _context.alarmAllResultVM.FromSql
                    ("call get_alarm_all_report_export(@from,@to,@campusId,@personId,@logMessage)",
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@logMessage", logMessage)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<alarmStudentPagedResult> ExportAlarmStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string persontype, string logMessage, DateTime from, DateTime to)
        {
            try
            {
                var result = new alarmStudentPagedResult();

                result.dailylogs = await _context.alarmStudentResultVM.FromSql
                    ("call get_alarm_student_report_export(@from,@to,@campusId,@educLevelId,@yearSecId,@studSecId,@personId,@logMessage)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@educLevelId", educLevelId),
                        new MySqlParameter("@yearSecId", yearSecId),
                        new MySqlParameter("@studSecId", studSecId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@logMessage", logMessage)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<alarmEmployeePagedResult> ExportAlarmEmployee(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to)
        {
            try
            {
                var result = new alarmEmployeePagedResult();

                result.dailylogs = await _context.alarmEmployeeResultVM.FromSql
                    ("call get_alarm_employee_report_export(@from,@to,@campusId,@personId,@logMessage)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@logMessage", logMessage)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<alarmOtherAccessPagedResult> ExportAlarmOtherAccess(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to)
        {
            try
            {
                var result = new alarmOtherAccessPagedResult();

                result.dailylogs = await _context.alarmOtherAccessResultVM.FromSql
                    ("call get_alarm_otheraccess_report_export(@from,@to,@campusId,@personId,@logMessage)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@logMessage", logMessage)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<alarmFetcherPagedResult> ExportAlarmFetcher(int campusId, int personId, string persontype, string logMessage, DateTime from, DateTime to)
        {
            try
            {
                var result = new alarmFetcherPagedResult();

                result.dailylogs = await _context.alarmFetcherResultVM.FromSql
                    ("call get_alarm_fetcher_report_export(@from,@to,@campusId,@personId,@logMessage)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@logMessage", logMessage)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Visitor

        public async Task<visitorReportPagedResult> GetAllVisitorReport(int personId, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new visitorReportPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Count.FromSql
                    ("call get_visitor_report_count(@from,@to,@personId)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@personId", personId)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.visitorinformations = await _context.visitorInformationResultVM.FromSql
                    ("call get_visitor_report_list(@skip,@pageSize,@from,@to,@personId)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@personId", personId)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<visitorReportPagedResult> ExportAllVisitorReport(int personId, DateTime from, DateTime to)
        {
            try
            {
                var result = new visitorReportPagedResult();

                result.visitorinformations = await _context.visitorInformationResultVM.FromSql
                    ("call get_visitor_report_export(@from,@to,@personId)",
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@personId", personId)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Library Attendance List

        public async Task<libraryAttendanceAllPagedResult> GetAllLibraryAttendanceAll(int campusId, int personId, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new libraryAttendanceAllPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Count.FromSql
                    ("call get_library_attendance_all_report_count(@from,@to,@campusId,@personId)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.librarylogs = await _context.libraryAttendanceAllResultVM.FromSql
                    ("call get_library_attendance_all_report_list(@skip,@pageSize,@from,@to,@campusId,@personId)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@personId", personId)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<libraryAttendanceStudentPagedResult> GetAllLibraryAttendanceStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new libraryAttendanceStudentPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Count.FromSql
                    ("call get_library_attendance_student_report_count(@from,@to,@campusId,@educLevelId,@yearSecId,@studSecId,@personId)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@educLevelId", educLevelId),
                        new MySqlParameter("@yearSecId", yearSecId),
                        new MySqlParameter("@studSecId", studSecId),
                        new MySqlParameter("@personId", personId)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.librarylogs = await _context.libraryAttendanceStudentResultVM.FromSql
                    ("call get_library_attendance_student_report_list(@skip,@pageSize,@from,@to,@campusId,@educLevelId,@yearSecId,@studSecId,@personId)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@educLevelId", educLevelId),
                    new MySqlParameter("@yearSecId", yearSecId),
                    new MySqlParameter("@studSecId", studSecId),
                    new MySqlParameter("@personId", personId)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<libraryAttendanceEmployeePagedResult> GetAllLibraryAttendanceEmployee(int campusId, int departmentId, int positionId, int personId, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new libraryAttendanceEmployeePagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Count.FromSql
                    ("call get_library_attendance_employee_report_count(@from,@to,@campusId,@departmentId,@positionId,@personId)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@departmentId", departmentId),
                        new MySqlParameter("@positionId", positionId),
                        new MySqlParameter("@personId", personId)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.librarylogs = await _context.libraryAttendanceEmployeeResultVM.FromSql
                    ("call get_library_attendance_employee_report_list(@skip,@pageSize,@from,@to,@campusId,@departmentId,@positionId,@personId)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@departmentId", departmentId),
                    new MySqlParameter("@positionId", positionId),
                    new MySqlParameter("@personId", personId)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Library Attendance Export

        public async Task<libraryAttendanceAllPagedResult> ExportLibraryAttendanceAll(int campusId, int personId, DateTime from, DateTime to)
        {
            try
            {
                var result = new libraryAttendanceAllPagedResult();

                result.librarylogs = await _context.libraryAttendanceAllResultVM.FromSql
                    ("call get_library_attendance_all_report_export(@from,@to,@campusId,@personId)",
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@personId", personId)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<libraryAttendanceStudentPagedResult> ExportLibraryAttendanceStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, DateTime from, DateTime to)
        {
            try
            {
                var result = new libraryAttendanceStudentPagedResult();

                result.librarylogs = await _context.libraryAttendanceStudentResultVM.FromSql
                    ("call get_library_attendance_student_report_export(@from,@to,@campusId,@educLevelId,@yearSecId,@studSecId,@personId)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@educLevelId", educLevelId),
                        new MySqlParameter("@yearSecId", yearSecId),
                        new MySqlParameter("@studSecId", studSecId),
                        new MySqlParameter("@personId", personId)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<libraryAttendanceEmployeePagedResult> ExportLibraryAttendanceEmployee(int campusId, int departmentId, int positionId, int personId, DateTime from, DateTime to)
        {
            try
            {
                var result = new libraryAttendanceEmployeePagedResult();

                result.librarylogs = await _context.libraryAttendanceEmployeeResultVM.FromSql
                    ("call get_library_attendance_employee_report_export(@from,@to,@campusId,@departmentId,@positionId,@personId)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@departmentId", departmentId),
                        new MySqlParameter("@positionId", positionId),
                        new MySqlParameter("@personId", personId)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Library Usage List

        public async Task<libraryUsageAllPagedResult> GetAllLibraryUsageAll(int campusId, string persontype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new libraryUsageAllPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Count.FromSql
                    ("call get_library_usage_all_report_count(@from,@to,@campusId,@persontype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@persontype", persontype)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.librarylogs = await _context.libraryUsageAllResultVM.FromSql
                    ("call get_library_usage_all_report_list(@skip,@pageSize,@from,@to,@campusId,@persontype)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@persontype", persontype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<libraryUsageStudentPagedResult> GetAllLibraryUsageStudent(int campusId, string persontype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new libraryUsageStudentPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Count.FromSql
                    ("call get_library_usage_all_report_count(@from,@to,@campusId,@persontype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@persontype", persontype)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.librarylogs = await _context.libraryUsageStudentResultVM.FromSql
                    ("call get_library_usage_all_report_list(@skip,@pageSize,@from,@to,@campusId,@persontype)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@persontype", persontype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<libraryUsageEmployeePagedResult> GetAllLibraryUsageEmployee(int campusId, string persontype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new libraryUsageEmployeePagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Count.FromSql
                    ("call get_library_usage_all_report_count(@from,@to,@campusId,@persontype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@persontype", persontype)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.librarylogs = await _context.libraryUsageEmployeeResultVM.FromSql
                    ("call get_library_usage_all_report_list(@skip,@pageSize,@from,@to,@campusId,@persontype)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@persontype", persontype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Library Usage Export

        public async Task<libraryUsageAllPagedResult> ExportLibraryUsageAll(int campusId, string persontype, DateTime from, DateTime to)
        {
            try
            {
                var result = new libraryUsageAllPagedResult();

                result.librarylogs = await _context.libraryUsageAllResultVM.FromSql
                    ("call get_library_usage_all_report_export(@from,@to,@campusId,@persontype)",
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@persontype", persontype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<libraryUsageStudentPagedResult> ExportLibraryUsageStudent(int campusId, string persontype, DateTime from, DateTime to)
        {
            try
            {
                var result = new libraryUsageStudentPagedResult();

                result.librarylogs = await _context.libraryUsageStudentResultVM.FromSql
                    ("call get_library_usage_all_report_export(@from,@to,@campusId,@persontype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@persontype", persontype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<libraryUsageEmployeePagedResult> ExportLibraryUsageEmployee(int campusId, string persontype, DateTime from, DateTime to)
        {
            try
            {
                var result = new libraryUsageEmployeePagedResult();

                result.librarylogs = await _context.libraryUsageEmployeeResultVM.FromSql
                    ("call get_library_usage_all_report_export(@from,@to,@campusId,@persontype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@persontype", persontype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Card List

        public async Task<cardEmployeePagedResult> GetAllCardEmployee(int campusId, int departmentId, int positionId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new cardEmployeePagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Count.FromSql
                    ("call get_card_employee_report_count(@from,@to,@campusId,@departmentId,@positionId,@personId,@statustype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@departmentId", departmentId),
                        new MySqlParameter("@positionId", positionId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@statustype", statustype)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.carddetails = await _context.cardEmployeeResultVM.FromSql
                    ("call get_card_employee_report_list(@skip,@pageSize,@from,@to,@campusId,@departmentId,@positionId,@personId,@statustype)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@departmentId", departmentId),
                    new MySqlParameter("@positionId", positionId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@statustype", statustype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<cardOtherAccessPagedResult> GetAllCardOtherAccess(int campusId, int departmentId, int positionId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new cardOtherAccessPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Count.FromSql
                    ("call get_card_otheraccess_report_count(@from,@to,@campusId,@departmentId,@positionId,@personId,@statustype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@departmentId", departmentId),
                        new MySqlParameter("@positionId", positionId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@statustype", statustype)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.carddetails = await _context.cardOtherAccessResultVM.FromSql
                    ("call get_card_otheraccess_report_list(@skip,@pageSize,@from,@to,@campusId,@departmentId,@positionId,@personId,@statustype)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@departmentId", departmentId),
                    new MySqlParameter("@positionId", positionId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@statustype", statustype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<cardFetcherPagedResult> GetAllCardFetcher(int campusId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new cardFetcherPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Count.FromSql
                    ("call get_card_fetcher_report_count(@from,@to,@campusId,@personId,@statustype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@statustype", statustype)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.carddetails = await _context.cardFetcherResultVM.FromSql
                    ("call get_card_fetcher_report_list(@skip,@pageSize,@from,@to,@campusId,@personId,@statustype)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@statustype", statustype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<cardAllPagedResult> GetAllCardAll(int campusId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new cardAllPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Count.FromSql
                    ("call get_card_all_report_count(@from,@to,@campusId,@personId,@statustype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@statustype", statustype)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.carddetails = await _context.cardAllResultVM.FromSql
                    ("call get_card_all_report_list(@skip,@pageSize,@from,@to,@campusId,@personId,@statustype)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@statustype", statustype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<cardStudentPagedResult> GetAllCardStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string statustype, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new cardStudentPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try
            {
                result.RowCount = Convert.ToInt32(_context.Report_Count.FromSql
                    ("call get_card_student_report_count(@from,@to,@campusId,@educLevelId,@yearSecId,@studSecId,@personId,@statustype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@educLevelId", educLevelId),
                        new MySqlParameter("@yearSecId", yearSecId),
                        new MySqlParameter("@studSecId", studSecId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@statustype", statustype)).AsNoTracking().FirstOrDefault().Count);

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.carddetails = await _context.cardStudentResultVM.FromSql
                    ("call get_card_student_report_list(@skip,@pageSize,@from,@to,@campusId,@educLevelId,@yearSecId,@studSecId,@personId,@statustype)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@educLevelId", educLevelId),
                    new MySqlParameter("@yearSecId", yearSecId),
                    new MySqlParameter("@studSecId", studSecId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@statustype", statustype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Card Export

        public async Task<cardAllPagedResult> ExportCardAll(int campusId, int personId, string statustype, DateTime from, DateTime to)
        {
            try
            {
                var result = new cardAllPagedResult();

                result.carddetails = await _context.cardAllResultVM.FromSql
                    ("call get_card_all_report_export(@from,@to,@campusId,@personId,@statustype)",
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@campusId", campusId),
                    new MySqlParameter("@personId", personId),
                    new MySqlParameter("@statustype", statustype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<cardStudentPagedResult> ExportCardStudent(int campusId, int educLevelId, int yearSecId, int studSecId, int personId, string statustype, DateTime from, DateTime to)
        {
            try
            {
                var result = new cardStudentPagedResult();

                result.carddetails = await _context.cardStudentResultVM.FromSql
                    ("call get_card_student_report_export(@from,@to,@campusId,@educLevelId,@yearSecId,@studSecId,@personId,@statustype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@educLevelId", educLevelId),
                        new MySqlParameter("@yearSecId", yearSecId),
                        new MySqlParameter("@studSecId", studSecId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@statustype", statustype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<cardEmployeePagedResult> ExportCardEmployee(int campusId, int departmentId, int positionId, int personId, string statustype, DateTime from, DateTime to)
        {
            try
            {
                var result = new cardEmployeePagedResult();

                result.carddetails = await _context.cardEmployeeResultVM.FromSql
                    ("call get_card_employee_report_export(@from,@to,@campusId,@departmentId,@positionId,@personId,@statustype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@departmentId", departmentId),
                        new MySqlParameter("@positionId", positionId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@statustype", statustype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<cardOtherAccessPagedResult> ExportCardOtherAccess(int campusId, int departmentId, int positionId, int personId, string statustype, DateTime from, DateTime to)
        {
            try
            {
                var result = new cardOtherAccessPagedResult();

                result.carddetails = await _context.cardOtherAccessResultVM.FromSql
                    ("call get_card_otheraccess_report_export(@from,@to,@campusId,@departmentId,@positionId,@personId,@statustype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@departmentId", departmentId),
                        new MySqlParameter("@positionId", positionId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@statustype", statustype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<cardFetcherPagedResult> ExportCardFetcher(int campusId, int personId, string statustype, DateTime from, DateTime to)
        {
            try
            {
                var result = new cardFetcherPagedResult();

                result.carddetails = await _context.cardFetcherResultVM.FromSql
                    ("call get_card_fetcher_report_export(@from,@to,@campusId,@personId,@statustype)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@campusId", campusId),
                        new MySqlParameter("@personId", personId),
                        new MySqlParameter("@statustype", statustype)).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Audit Trail

        public async Task<auditTrailPagedResult> GetAllAuditTrail(int user, string status, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            var result = new auditTrailPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            result.RowCount = Convert.ToInt32(_context.Report_Audit_Trail_Count.FromSql
                ("call get_audittrail_report_count(@from,@to,@userid,@status)",
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@userid", user),
                    new MySqlParameter("@status", status == "" || status == null || status == "All" ? "All" : status.Equals("Successful") ? "Successful" : "Failed")).AsNoTracking().FirstOrDefault().Count);

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (pageNo - 1) * pageSize;

            result.audittrails = await _context.auditTrailResultVM.FromSql
                ("call get_audittrail_report_list(@skip,@pageSize,@from,@to,@userid,@status)",
                    new MySqlParameter("@skip", skip),
                    new MySqlParameter("@pageSize", pageSize),
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@userid", user),
                    new MySqlParameter("@status", status == "" || status == null || status == "All" ? "All" : status.Equals("Successful") ? "Successful" : "Failed")).ToListAsync();

            return result;
        }

        public async Task<auditTrailPagedResult> ExportAuditTrail(int user, string status, DateTime from, DateTime to)
        {
            try
            {
                var result = new auditTrailPagedResult();

                result.audittrails = await _context.auditTrailResultVM.FromSql
                    ("call get_audittrail_report_export(@from,@to,@userid,@status)",
                        new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                        new MySqlParameter("@userid", user),
                        new MySqlParameter("@status", status == "" || status == null || status == "All" ? "All" : status.Equals("Successful") ? "Successful" : "Failed")).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        /* ------------------------------------------------------------------------------------------------------------- */

        public async Task<timeAndAttendanceStudentPagedResult> ExportTimeAndAttendanceStudent(string logstat,
                string keyword,
                long terminalid,
                long areaid,
                long campusid,
                DateTime from,
                DateTime to)
        {
            var result = new timeAndAttendanceStudentPagedResult();
            //keyword.ToUpper();

            //result.dailylogs = await _context.DailyLogsEntity
            //    .Include(x => x.CardDetailsEntity)
            //    .ThenInclude(x => x.PersonEntity)
            //    .Include(x => x.TerminalEntity)
            //    .ThenInclude(x => x.AreaEntity)
            //    .ThenInclude(x => x.CampusEntity)
            //    .Where(q => (q.Log_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
            //        && q.Log_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
            //        && (keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.ID_Number.Equals(keyword) : q.CardDetailsEntity.PersonEntity.ID_Number.Contains(keyword)
            //                || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.First_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.First_Name.ToUpper().Contains(keyword)
            //                || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.Last_Name.ToUpper().Contains(keyword)
            //                || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) :
            //                    (RemoveSpecialChar(q.CardDetailsEntity.PersonEntity.Last_Name + q.CardDetailsEntity.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
            //        && (logstat.Equals("") ? q.Log_Message.Equals("LOG IN") || q.Log_Message.Equals("LOG OUT") : q.Log_Message.Equals(logstat))
            //        && (terminalid.Equals(0) ? q.Terminal_ID != 0 : q.Terminal_ID == terminalid)
            //        && (areaid.Equals(0) ? q.TerminalEntity.Area_ID != 0 : q.TerminalEntity.Area_ID == areaid)
            //        && (campusid.Equals(0) ? q.TerminalEntity.AreaEntity.Campus_ID != 0 : q.TerminalEntity.AreaEntity.Campus_ID == campusid)
            //        && (q.CardDetailsEntity.PersonEntity.Person_Type == "S")).ToListAsync();

            return result;
        }

        private string RemoveSpecialChar(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().ToUpper();
        }

        public async Task<timeAndAttendanceVisitorPagedResult> ExportTimeAndAttendanceVisitor(string logstat,
                string keyword,
                long terminalid,
                long areaid,
                long campusid,
                DateTime from,
                DateTime to)
        {
            var result = new timeAndAttendanceVisitorPagedResult();
            keyword.ToUpper();

            result.dailylogs = await _context.DailyLogsEntity
                .Include(x => x.CardDetailsEntity)
                .ThenInclude(x => x.PersonEntity)
                .Include(x => x.TerminalEntity)
                .ThenInclude(x => x.AreaEntity)
                .ThenInclude(x => x.CampusEntity)
                .Where(q => (q.Log_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                    && q.Log_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                    && (keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.ID_Number.Equals(keyword) : q.CardDetailsEntity.PersonEntity.ID_Number.Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.First_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.First_Name.ToUpper().Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) :
                                (RemoveSpecialChar(q.CardDetailsEntity.PersonEntity.Last_Name + q.CardDetailsEntity.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                    && (logstat.Equals("") ? q.Log_Message.Equals("LOG IN") || q.Log_Message.Equals("LOG OUT") : q.Log_Message.Equals(logstat))
                    && (terminalid.Equals(0) ? q.Terminal_ID != 0 : q.Terminal_ID == terminalid)
                    && (areaid.Equals(0) ? q.TerminalEntity.Area_ID != 0 : q.TerminalEntity.Area_ID == areaid)
                    && (campusid.Equals(0) ? q.TerminalEntity.AreaEntity.Campus_ID != 0 : q.TerminalEntity.AreaEntity.Campus_ID == campusid)
                    && (q.CardDetailsEntity.PersonEntity.Person_Type == "O")).ToListAsync();

            return result;
        }

        public async Task<timeAndAttendanceVisitorPagedResult> GetAllTimeAttendanceVisitor(
            string logstat,
            string keyword,
            long terminalid,
            long areaid,
            long campusid,
            DateTime from,
            DateTime to,
            int pageNo,
            int pageSize
            )
        {
            try
            {
                var result = new timeAndAttendanceVisitorPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;
                keyword.ToUpper();

                result.RowCount = _context.DailyLogsEntity
                    .Include(x => x.CardDetailsEntity)
                    .ThenInclude(x => x.PersonEntity)
                    .Include(x => x.TerminalEntity)
                    .ThenInclude(x => x.AreaEntity)
                    .ThenInclude(x => x.CampusEntity)
                    .Where(q => (q.Log_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                        && q.Log_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                        && (keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.ID_Number.Equals(keyword) : q.CardDetailsEntity.PersonEntity.ID_Number.Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.First_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.First_Name.ToUpper().Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) :
                                (RemoveSpecialChar(q.CardDetailsEntity.PersonEntity.Last_Name + q.CardDetailsEntity.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                        && (logstat.Equals("") ? q.Log_Message.Equals("LOG IN") || q.Log_Message.Equals("LOG OUT") : q.Log_Message.Equals(logstat))
                        && (terminalid.Equals(0) ? q.Terminal_ID != 0 : q.Terminal_ID == terminalid)
                        && (areaid.Equals(0) ? q.TerminalEntity.Area_ID != 0 : q.TerminalEntity.Area_ID == areaid)
                        && (campusid.Equals(0) ? q.TerminalEntity.AreaEntity.Campus_ID != 0 : q.TerminalEntity.AreaEntity.Campus_ID == campusid)
                        && (q.CardDetailsEntity.PersonEntity.Person_Type == "O")).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.dailylogs = await _context.DailyLogsEntity
                    .Include(x => x.CardDetailsEntity)
                    .ThenInclude(x => x.PersonEntity)
                    .Include(x => x.TerminalEntity)
                    .ThenInclude(x => x.AreaEntity)
                    .ThenInclude(x => x.CampusEntity)
                    .Where(q => (q.Log_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                        && q.Log_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                        && (keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.ID_Number.Equals(keyword) : q.CardDetailsEntity.PersonEntity.ID_Number.Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.First_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.First_Name.ToUpper().Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) :
                                (RemoveSpecialChar(q.CardDetailsEntity.PersonEntity.Last_Name + q.CardDetailsEntity.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                        && (logstat.Equals("") ? q.Log_Message.Equals("LOG IN") || q.Log_Message.Equals("LOG OUT") : q.Log_Message.Equals(logstat))
                        && (terminalid.Equals(0) ? q.Terminal_ID != 0 : q.Terminal_ID == terminalid)
                        && (areaid.Equals(0) ? q.TerminalEntity.Area_ID != 0 : q.TerminalEntity.Area_ID == areaid)
                        && (campusid.Equals(0) ? q.TerminalEntity.AreaEntity.Campus_ID != 0 : q.TerminalEntity.AreaEntity.Campus_ID == campusid)
                        && (q.CardDetailsEntity.PersonEntity.Person_Type == "O"))
                    .OrderBy(c => c.CardHolderID)
                    .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<AlarmVisitorPagedResult> GetAllAlarmVisitor(string logstat, string keyword, long terminalid, long areaid, long campusid, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            try
            {
                var result = new AlarmVisitorPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;
                keyword.ToUpper();
                if (logstat == "BLACKLISTED")
                    logstat = "BLACKLIST";

                result.RowCount = _context.DailyLogsEntity
                    .Include(x => x.CardDetailsEntity)
                    .ThenInclude(x => x.PersonEntity)
                    .Include(x => x.TerminalEntity)
                    .ThenInclude(x => x.AreaEntity)
                    .ThenInclude(x => x.CampusEntity)
                    .Where(q => (q.Log_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                        && q.Log_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                        && (keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.ID_Number.Equals(keyword) : q.CardDetailsEntity.PersonEntity.ID_Number.Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.First_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.First_Name.ToUpper().Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) :
                                (RemoveSpecialChar(q.CardDetailsEntity.PersonEntity.Last_Name + q.CardDetailsEntity.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                        && (logstat.Equals("") ? q.Log_Status == false : RemoveSpecialChar(q.Log_Message).Contains(RemoveSpecialChar(logstat)))
                        && (terminalid.Equals(0) ? q.Terminal_ID != 0 : q.Terminal_ID == terminalid)
                        && (areaid.Equals(0) ? q.TerminalEntity.Area_ID != 0 : q.TerminalEntity.Area_ID == areaid)
                        && (campusid.Equals(0) ? q.TerminalEntity.AreaEntity.Campus_ID != 0 : q.TerminalEntity.AreaEntity.Campus_ID == campusid)
                        && (q.CardDetailsEntity.PersonEntity.Person_Type == "O")).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.dailylogs = await _context.DailyLogsEntity
                    .Include(x => x.CardDetailsEntity)
                    .ThenInclude(x => x.PersonEntity)
                    .Include(x => x.TerminalEntity)
                    .ThenInclude(x => x.AreaEntity)
                    .ThenInclude(x => x.CampusEntity)
                    .Where(q => (q.Log_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                        && q.Log_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                        && (keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.ID_Number.Equals(keyword) : q.CardDetailsEntity.PersonEntity.ID_Number.Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.First_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.First_Name.ToUpper().Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) :
                                (RemoveSpecialChar(q.CardDetailsEntity.PersonEntity.Last_Name + q.CardDetailsEntity.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                        && (logstat.Equals("") ? q.Log_Status == false : RemoveSpecialChar(q.Log_Message).Contains(RemoveSpecialChar(logstat)))
                        && (terminalid.Equals(0) ? q.Terminal_ID != 0 : q.Terminal_ID == terminalid)
                        && (areaid.Equals(0) ? q.TerminalEntity.Area_ID != 0 : q.TerminalEntity.Area_ID == areaid)
                        && (campusid.Equals(0) ? q.TerminalEntity.AreaEntity.Campus_ID != 0 : q.TerminalEntity.AreaEntity.Campus_ID == campusid)
                        && (q.CardDetailsEntity.PersonEntity.Person_Type == "O"))
                    .OrderBy(c => c.CardHolderID)
                    .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<AlarmVisitorPagedResult> ExportAllAlarmVisitor(string logstat, string keyword, long terminalid, long areaid, long campusid, DateTime from, DateTime to)
        {
            try
            {
                var result = new AlarmVisitorPagedResult();
                keyword.ToUpper();
                if (logstat == "BLACKLISTED")
                    logstat = "BLACKLIST";

                result.dailylogs = await _context.DailyLogsEntity
                    .Include(x => x.CardDetailsEntity)
                    .ThenInclude(x => x.PersonEntity)
                    .Include(x => x.TerminalEntity)
                    .ThenInclude(x => x.AreaEntity)
                    .ThenInclude(x => x.CampusEntity)
                    .Where(q => (q.Log_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                        && q.Log_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                        && (keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.ID_Number.Equals(keyword) : q.CardDetailsEntity.PersonEntity.ID_Number.Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.First_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.First_Name.ToUpper().Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) : q.CardDetailsEntity.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                            || keyword.Equals("") ? !q.CardDetailsEntity.PersonEntity.Last_Name.Equals(keyword) :
                                (RemoveSpecialChar(q.CardDetailsEntity.PersonEntity.Last_Name + q.CardDetailsEntity.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                        && (logstat.Equals("") ? q.Log_Status == false : RemoveSpecialChar(q.Log_Message).Contains(RemoveSpecialChar(logstat)))
                        && (terminalid.Equals(0) ? q.Terminal_ID != 0 : q.Terminal_ID == terminalid)
                        && (areaid.Equals(0) ? q.TerminalEntity.Area_ID != 0 : q.TerminalEntity.Area_ID == areaid)
                        && (campusid.Equals(0) ? q.TerminalEntity.AreaEntity.Campus_ID != 0 : q.TerminalEntity.AreaEntity.Campus_ID == campusid)
                        && (q.CardDetailsEntity.PersonEntity.Person_Type == "O")).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IQueryable<TimeAttendanceVisitorEntity> GetAllTimeAttendanceVisitor(timeAttendanceVisitorFilter filter)
        {
            return context.Report_Time_Attendance_Visitor.FromSql<TimeAttendanceVisitorEntity>
                ("call spReport_TimeAndAttendanceVisitor(@from, @to,@filter,@campus,@area,@terminal,@logstat)",
                new MySqlParameter("@from", filter.from),
                new MySqlParameter("@to", filter.to),
                new MySqlParameter("@terminal", filter.terminal_ID),
                new MySqlParameter("@filter", filter.filter == null || filter.filter == "" ? null : filter.filter),
                new MySqlParameter("@campus", filter.campus_ID),
                new MySqlParameter("@area", filter.area_ID),
                new MySqlParameter("@logstat", filter.logstat == null ? "" : filter.logstat)
                ).AsNoTracking();
        }

        public IQueryable<AlarmEntity> GetAllAlarmStudent(alarmFilter filter)
        {
            return context.Report_Alarm_Student.FromSql<AlarmEntity>
                ("call spReport_AlarmStudent(@from, @to,@filter,@campus,@area,@terminal,@logstat)",
                new MySqlParameter("@from", filter.from),
                new MySqlParameter("@to", filter.to),
                new MySqlParameter("@terminal", filter.terminal_ID),
                new MySqlParameter("@filter", filter.filter == null || filter.filter == "" ? null : filter.filter),
                new MySqlParameter("@campus", filter.campus_ID),
                new MySqlParameter("@area", filter.area_ID),
                new MySqlParameter("@logstat", filter.logstat == null ? "" : filter.logstat)
                ).AsNoTracking();
        }

        public IQueryable<AlarmEmployeeEntity> GetAllAlarmEmployee(alarmEmployeeFilter filter)
        {
            return context.Report_Alarm_Employee.FromSql<AlarmEmployeeEntity>
                ("call spReport_AlarmEmployee(@from, @to,@filter,@campus,@area,@terminal,@logstat)",
                new MySqlParameter("@from", filter.from),
                new MySqlParameter("@to", filter.to),
                new MySqlParameter("@terminal", filter.terminal_ID),
                new MySqlParameter("@filter", filter.filter == null || filter.filter == "" ? null : filter.filter),
                new MySqlParameter("@campus", filter.campus_ID),
                new MySqlParameter("@area", filter.area_ID),
                new MySqlParameter("@logstat", filter.logstat == null ? "" : filter.logstat)
                ).AsNoTracking();
        }

        public IQueryable<AlarmVisitorEntity> GetAllAlarmVisitor(alarmVisitorFilter filter)
        {
            return context.Report_Alarm_Visitor.FromSql<AlarmVisitorEntity>
                ("call spReport_AlarmVisitor(@from, @to,@filter,@campus,@area,@terminal,@logstat)",
                new MySqlParameter("@from", filter.from),
                new MySqlParameter("@to", filter.to),
                new MySqlParameter("@terminal", filter.terminal_ID),
                new MySqlParameter("@filter", filter.filter == null || filter.filter == "" ? null : filter.filter),
                new MySqlParameter("@campus", filter.campus_ID),
                new MySqlParameter("@area", filter.area_ID),
                new MySqlParameter("@logstat", filter.logstat == null ? "" : filter.logstat)
                ).AsNoTracking();
        }

        public async Task<cardReportPagedResult> GetAllCardReport(string persontype, string keyword, string cardstatus, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            try
            {
                var result = new cardReportPagedResult();
                //result.CurrentPage = pageNo;
                //result.PageSize = pageSize;
                //keyword.ToUpper();

                //var pageCount = 0.00;
                //var skip = 0;

                //if (cardstatus.Equals("") || cardstatus.Equals("undefined"))
                //{
                //    result.RowCount = _context.CardDetailsEntity
                //        .Include(x => x.PersonEntity)
                //        .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
                //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                //            && (q.PersonEntity.Person_Type == persontype)
                //            ).Count();

                //    pageCount = (double)result.RowCount / pageSize;
                //    result.PageCount = (int)Math.Ceiling(pageCount);

                //    skip = (pageNo - 1) * pageSize;

                //    result.carddetails = await _context.CardDetailsEntity
                //        .Include(x => x.PersonEntity)
                //            .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
                //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                //            && (q.PersonEntity.Person_Type == persontype))
                //        .OrderBy(c => c.CardHolderID)
                //        .Skip(skip).Take(pageSize).ToListAsync();
                //}
                //else if (cardstatus.Equals("ACTIVE"))
                //{
                //    result.RowCount = _context.CardDetailsEntity
                //        .Include(x => x.PersonEntity)
                //        .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
                //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                //            && (q.PersonEntity.Person_Type == persontype)
                //            && (q.IsActive == true)
                //            && (q.Blocked == false)
                //            && (q.On_Hold == false)
                //            ).Count();

                //    pageCount = (double)result.RowCount / pageSize;
                //    result.PageCount = (int)Math.Ceiling(pageCount);

                //    skip = (pageNo - 1) * pageSize;

                //    result.carddetails = await _context.CardDetailsEntity
                //        .Include(x => x.PersonEntity)
                //            .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
                //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                //            && (q.PersonEntity.Person_Type == persontype)
                //            && (q.IsActive == true)
                //            && (q.Blocked == false)
                //            && (q.On_Hold == false))
                //        .OrderBy(c => c.ID)
                //        .Skip(skip).Take(pageSize).ToListAsync();
                //}
                //else if (cardstatus.Equals("NOT VALIDATED"))
                //{
                //    result.RowCount = _context.CardDetailsEntity
                //        .Include(x => x.PersonEntity)
                //        .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
                //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                //            && (q.PersonEntity.Person_Type == persontype)
                //            && (q.IsActive == false)
                //            ).Count();

                //    pageCount = (double)result.RowCount / pageSize;
                //    result.PageCount = (int)Math.Ceiling(pageCount);

                //    skip = (pageNo - 1) * pageSize;

                //    result.carddetails = await _context.CardDetailsEntity
                //        .Include(x => x.PersonEntity)
                //            .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
                //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                //            && (q.PersonEntity.Person_Type == persontype)
                //            && (q.IsActive == false))
                //        .OrderBy(c => c.Cardholder_ID)
                //        .Skip(skip).Take(pageSize).ToListAsync();
                //}
                //else if (cardstatus.Contains("ON") && cardstatus.Contains("HOLD"))
                //{
                //    result.RowCount = _context.CardDetailsEntity
                //        .Include(x => x.PersonEntity)
                //        .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
                //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                //            && (q.PersonEntity.Person_Type == persontype)
                //            && (q.IsActive == true)
                //            && (q.Blocked == false)
                //            && (q.On_Hold == true)
                //            ).Count();

                //    pageCount = (double)result.RowCount / pageSize;
                //    result.PageCount = (int)Math.Ceiling(pageCount);

                //    skip = (pageNo - 1) * pageSize;

                //    result.carddetails = await _context.CardDetailsEntity
                //        .Include(x => x.PersonEntity)
                //            .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
                //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                //            && (q.PersonEntity.Person_Type == persontype)
                //            && (q.IsActive == true)
                //            && (q.Blocked == false)
                //            && (q.On_Hold == true))
                //        .OrderBy(c => c.ID)
                //        .Skip(skip).Take(pageSize).ToListAsync();
                //}
                //else if (cardstatus.Contains("BLACKLISTED"))
                //{
                //    result.RowCount = _context.CardDetailsEntity
                //        .Include(x => x.PersonEntity)
                //        .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
                //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                //            && (q.PersonEntity.Person_Type == persontype)
                //            && (q.IsActive == true)
                //            && (q.Blocked == true)
                //            && (q.On_Hold == false)
                //            ).Count();

                //    pageCount = (double)result.RowCount / pageSize;
                //    result.PageCount = (int)Math.Ceiling(pageCount);

                //    skip = (pageNo - 1) * pageSize;

                //    result.carddetails = await _context.CardDetailsEntity
                //        .Include(x => x.PersonEntity)
                //            .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
                //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
                //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
                //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
                //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
                //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
                //            && (q.PersonEntity.Person_Type == persontype)
                //            && (q.IsActive == true)
                //            && (q.Blocked == true)
                //            && (q.On_Hold == false))
                //        .OrderBy(c => c.ID)
                //        .Skip(skip).Take(pageSize).ToListAsync();
                //}

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<cardReportPagedResult> ExportCardReport(string persontype, string keyword, string cardstatus, DateTime from, DateTime to)
        {
            var result = new cardReportPagedResult();
            //keyword.ToUpper();

            //if (cardstatus.Equals("") || cardstatus.Equals("undefined"))
            //{
            //    result.carddetails = await _context.CardDetailsEntity
            //        .Include(x => x.PersonEntity)
            //            .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
            //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
            //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
            //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
            //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
            //            && (q.PersonEntity.Person_Type == persontype)).ToListAsync();
            //}
            //else if (cardstatus.Equals("ACTIVE"))
            //{
            //    result.carddetails = await _context.CardDetailsEntity
            //        .Include(x => x.PersonEntity)
            //            .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
            //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
            //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
            //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
            //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
            //            && (q.PersonEntity.Person_Type == persontype)
            //            && (q.IsActive == true)
            //            && (q.Blocked == false)
            //            && (q.On_Hold == false)).ToListAsync();
            //}
            //else if (cardstatus.Equals("NOT VALIDATED"))
            //{
            //    result.carddetails = await _context.CardDetailsEntity
            //        .Include(x => x.PersonEntity)
            //            .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
            //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
            //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
            //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
            //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
            //            && (q.PersonEntity.Person_Type == persontype)
            //            && (q.IsActive == false)).ToListAsync();
            //}
            //else if (cardstatus.Contains("ON") && cardstatus.Contains("HOLD"))
            //{
            //    result.carddetails = await _context.CardDetailsEntity
            //        .Include(x => x.PersonEntity)
            //            .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
            //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
            //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
            //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
            //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
            //            && (q.PersonEntity.Person_Type == persontype)
            //            && (q.IsActive == true)
            //            && (q.Blocked == false)
            //            && (q.On_Hold == true)).ToListAsync();
            //}
            //else if (cardstatus.Contains("BLACKLISTED"))
            //{
            //    result.carddetails = await _context.CardDetailsEntity
            //        .Include(x => x.PersonEntity)
            //            .Where(q => (q.Issued_Date >= Convert.ToDateTime(from.ToString("yyyy-MM-dd 00:00:00"))
            //            && q.Issued_Date <= Convert.ToDateTime(to.ToString("yyyy-MM-dd 23:59:59")))
            //            && (keyword.Equals("") ? !q.PersonEntity.ID_Number.Equals(keyword) : q.PersonEntity.ID_Number.Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.First_Name.Equals(keyword) : q.PersonEntity.First_Name.ToUpper().Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) : q.PersonEntity.Last_Name.ToUpper().Contains(keyword)
            //                || keyword.Equals("") ? !q.Remarks.Equals(keyword) : q.Remarks.Contains(keyword)
            //                || keyword.Equals("") ? !q.PersonEntity.Last_Name.Equals(keyword) :
            //                    (RemoveSpecialChar(q.PersonEntity.Last_Name + q.PersonEntity.First_Name)).Contains(RemoveSpecialChar(keyword)))
            //            && (q.PersonEntity.Person_Type == persontype)
            //            && (q.IsActive == true)
            //            && (q.Blocked == true)
            //            && (q.On_Hold == false)).ToListAsync();
            //}

            return result;
        }

        public IQueryable<CardEntity> GetAllCardReport(cardFilterVM filter)
        {
            return context.Report_Card.FromSql<CardEntity>
                ("call spReport_Card(@from,@to,@filter,@cardstatus,@persontype)",
                new MySqlParameter("@from", Convert.ToDateTime(filter.from.ToString("yyyy-MM-dd HH:mm:ss"))),
                new MySqlParameter("@to", Convert.ToDateTime(filter.to.ToString("yyyy-MM-dd HH:mm:ss"))),
                new MySqlParameter("@filter", filter.filter == null ? "" : filter.filter),
                new MySqlParameter("@cardstatus", filter.cardStatus == null ? "" : filter.cardStatus),
                new MySqlParameter("@persontype", filter.personType == null ? "" : filter.personType)
                ).AsNoTracking();
        }

        public IQueryable<formEntity> GetAllForms()
        {
            return context.FormEntity;
        }

        public async Task<totalHoursOfEmployeePagedResult> GetTotalHoursOfEmployee(
            string emptype,
            string keyword,
            string departmentid,
            DateTime from,
            DateTime to,
            int pageNo,
            int pageSize
            )
        {
            var result = new totalHoursOfEmployeePagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            try { 
                result.RowCount = _context.Report_Total_Hours_Employee.FromSql
                    ("call spReport_Total_Hours_Employee(@from,@to,@departmentid,@emptype,@keyword)",
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@departmentid", departmentid == null ? "" : departmentid),
                    new MySqlParameter("@emptype", emptype == null ? "" : emptype),
                    new MySqlParameter("@keyword", keyword)
                    ).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                result.dailylogs = await _context.totalHoursEmployeeVM.FromSql
                    ("call spReport_Total_Hours_Employee(@from,@to,@departmentid,@emptype,@keyword)",
                    new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                    new MySqlParameter("@departmentid", departmentid == null ? "" : departmentid),
                    new MySqlParameter("@emptype", emptype == null ? "" : emptype),
                    new MySqlParameter("@keyword", keyword)
                    ).Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<totalHoursOfEmployeePagedResult> ExportTotalHoursOfEmployee(string emptype,
            string keyword,
            string departmentid,
            DateTime from,
            DateTime to)
        {
            var result = new totalHoursOfEmployeePagedResult();

            result.dailylogs = await _context.totalHoursEmployeeVM.FromSql
                   ("call spReport_Total_Hours_Employee(@from,@to,@departmentid,@emptype,@keyword)",
                   new MySqlParameter("@from", Convert.ToDateTime(from.ToString("yyyy-MM-dd HH:mm:ss"))),
                   new MySqlParameter("@to", Convert.ToDateTime(to.ToString("yyyy-MM-dd HH:mm:ss"))),
                   new MySqlParameter("@departmentid", departmentid == null ? "" : departmentid),
                   new MySqlParameter("@emptype", emptype == null ? "" : emptype),
                   new MySqlParameter("@keyword", keyword)
                   ).ToListAsync();

            return result;
        }
    }
}
