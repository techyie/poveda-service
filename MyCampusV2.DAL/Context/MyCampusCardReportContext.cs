using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Common.ViewModels.ReportViewModel;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Models.Reports;

namespace MyCampusV2.DAL.Context
{
    public class MyCampusCardReportContext : DbContext
    {
        public MyCampusCardReportContext(DbContextOptions<MyCampusCardReportContext> options)
            : base(options)
        {

        }

        #region Others
            public DbSet<divisionEntity> DivisionEntity { get; set; }
            public DbSet<regionEntity> RegionEntity { get; set; }
            public DbSet<auditTrailEntity> AuditTrailEntity { get; set; }

            public DbSet<eventLoggingEntity> EventLoggingEntity { get; set; }
        #endregion

        #region Campus Management
        public DbSet<campusEntity> CampusEntity { get; set; }
            public DbSet<areaEntity> areaEntity { get; set; }
            public DbSet<departmentEntity> DepartmentEntity { get; set; }
            public DbSet<positionEntity> PositionEntity { get; set; }
            public DbSet<officeEntity> OfficeEntity { get; set; }
            public DbSet<educationalLevelEntity> EducationalLevelEntity { get; set; }
            public DbSet<collegeEntity> CollegeEntity { get; set; }
            public DbSet<courseEntity> CourseEntity { get; set; }
            public DbSet<studentSectionEntity> StudentSectionEntity { get; set; }
            public DbSet<yearSectionEntity> YearSectionEntity { get; set; }
            public DbSet<employeeSubTypeEntity> EmployeeSubTypeEntity { get; set; }
            public DbSet<empTypeEntity> EmpTypeEntity { get; set; }
        #endregion

        #region Person Management
            public DbSet<personEntity> PersonEntity { get; set; }
            public DbSet<emergencyContactEntity> EmergencyContactEntity { get; set; }
            public DbSet<govIdsEntity> GovIdsEntity { get; set; }
        #endregion
        
        public DbSet<emergencyContactEntity> emergencyContactEntity { get; set; }
        public DbSet<govIdsEntity> govIdsEntity { get; set; }
        public DbSet<formEntity> FormEntity { get; set; }

        //public DbSet<Visitor> visitor { get; set; }
        public DbSet<visitorInformationEntity> visitorInformation { get; set; }
        public DbSet<terminalEntity> tbl_terminal { get; set; }
        //public DbSet<tbl_terminal_category> tbl_terminal_category { get; set; }
        public DbSet<notificationEntity> notificationEntity { get; set; }
        public DbSet<cardDetailsEntity> CardDetailsEntity { get; set; }
        public DbSet<dailyLogsEntity> DailyLogsEntity { get; set; }

        public DbSet<datasyncEntity> tbl_data_sync { get; set; }
        public DbSet<terminalWhitelistEntity> tbl_terminal_whitelist { get; set; }
        public DbSet<terminalConfigurationEntity> tbl_terminal_configuration { get; set; }
        public DbSet<batchUploadEntity> tbl_batch_upload { get; set; }

        #region User Management
            public DbSet<userEntity> UserEntity { get; set; }
            public DbSet<roleEntity> RoleEntity { get; set; }
            public DbSet<userRoleEntity> UserRoleEntity { get; set; }
            public DbSet<rolePermissionEntity> RolePermissionEntity { get; set; }
            public DbSet<userAccessEntity> UserAccessEntity { get; set; }
        #endregion

        #region Terminal Whitelist
        public DbSet<terminalWhitelistVM> terminalWhitelistVM { get; set; }
        public DbSet<terminalWhitelistCount> tbl_terminal_whitelist_count { get; set; }
        #endregion

        #region Reports_DBSet
        public DbSet<TotalHoursOfEmployeeVM> totalHoursEmployeeVM { get; set; }
        public DbSet<AuditTrailVM> auditTrailVM { get; set; }
        public DbSet<Audit_Trail_Report_Count> Report_Audit_Trail_Count { get; set; }
        public DbSet<Time_Attendance_Report_Count> Report_Time_Attendance_Count { get; set; }
        public DbSet<Alarm_Report_Count> Report_Alarm_Count { get; set; }
        public DbSet<Report_Count> Report_Count { get; set; }

        public DbSet<DetailsPerPersonResultVM> detailsPerPersonVM { get; set; }

        public DbSet<AuditTrailResultVM> auditTrailResultVM { get; set; }

        public DbSet<TimeAndAttendanceStudentResultVM> timeAndAttendanceStudentResultVM { get; set; }
        public DbSet<TimeAndAttendanceEmployeeResultVM> timeAndAttendanceEmployeeResultVM { get; set; }
        public DbSet<TimeAndAttendanceOtherAccessResultVM> timeAndAttendanceOtherAccessResultVM { get; set; }
        public DbSet<TimeAndAttendanceFetcherResultVM> timeAndAttendanceFetcherResultVM { get; set; }
        public DbSet<TimeAndAttendanceAllResultVM> timeAndAttendanceAllResultVM { get; set; }

        public DbSet<AlarmStudentResultVM> alarmStudentResultVM { get; set; }
        public DbSet<AlarmEmployeeResultVM> alarmEmployeeResultVM { get; set; }
        public DbSet<AlarmOtherAccessResultVM> alarmOtherAccessResultVM { get; set; }
        public DbSet<AlarmFetcherResultVM> alarmFetcherResultVM { get; set; }
        public DbSet<AlarmAllResultVM> alarmAllResultVM { get; set; }

        public DbSet<VisitorInformationResultVM> visitorInformationResultVM { get; set; }

        public DbSet<LibraryAttendanceAllResultVM> libraryAttendanceAllResultVM { get; set; }
        public DbSet<LibraryAttendanceStudentResultVM> libraryAttendanceStudentResultVM { get; set; }
        public DbSet<LibraryAttendanceEmployeeResultVM> libraryAttendanceEmployeeResultVM { get; set; }

        public DbSet<LibraryUsageAllResultVM> libraryUsageAllResultVM { get; set; }
        public DbSet<LibraryUsageStudentResultVM> libraryUsageStudentResultVM { get; set; }
        public DbSet<LibraryUsageEmployeeResultVM> libraryUsageEmployeeResultVM { get; set; }

        public DbSet<CardStudentResultVM> cardStudentResultVM { get; set; }
        public DbSet<CardEmployeeResultVM> cardEmployeeResultVM { get; set; }
        public DbSet<CardOtherAccessResultVM> cardOtherAccessResultVM { get; set; }
        public DbSet<CardFetcherResultVM> cardFetcherResultVM { get; set; }
        public DbSet<CardAllResultVM> cardAllResultVM { get; set; }
        #endregion

        #region Reports
        public DbQuery<TimeAttendanceEmployeeEntity> Report_Time_Attendance_Employee { get; set; }
        public DbQuery<TimeAttendanceVisitorEntity> Report_Time_Attendance_Visitor { get; set; }
        public DbQuery<TimeAttendanceEntity> Report_Time_Attendance_Student { get; set; }
        public DbQuery<AuditTrail> Report_Audit_Trail { get; set; }
        //public DbQuery<Audit_Trail_Report_Count> Report_Audit_Trail_Count { get; set; }
        public DbQuery<AlarmEntity> Report_Alarm_Student { get; set; }
        public DbQuery<AlarmEmployeeEntity> Report_Alarm_Employee { get; set; }
        public DbQuery<AlarmVisitorEntity> Report_Alarm_Visitor { get; set; }
        public DbQuery<Card_Student> Report_Card_Student { get; set; }
        public DbQuery<Card_Employee> Report_Card_Employee { get; set; }
        public DbQuery<CardEntity> Report_Card { get; set; }
        public DbQuery<Total_Hours_Employee> Report_Total_Hours_Employee { get; set; }
        public DbQuery<VisitorReportEntity> Report_Visitor { get; set; }
        #endregion


        #region Fetcher Management

        public DbSet<scheduleEntity> tbl_schedule { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<campusEntity>().HasKey(g => new { g.Campus_ID });


            /*modelBuilder.Entity<tbl_user_role_access>()
            .HasOne(s => s.tbl_form)
            .WithMany(c => c.user_role_access)
            .HasForeignKey(s => s.Form_Code)
            .HasPrincipalKey(c => c.Form_Code);*/
        }

    }
}
