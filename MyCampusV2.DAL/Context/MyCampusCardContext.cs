using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Common.ViewModels.ReportViewModel;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Models.Reports;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace MyCampusV2.DAL.Context
{
    public class MyCampusCardContext : DbContext
    {

        public MyCampusCardContext(DbContextOptions<MyCampusCardContext> options) 
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
            public DbSet<areaEntity> AreaEntity { get; set; }
            public DbSet<departmentEntity> DepartmentEntity { get; set; }
            public DbSet<positionEntity> PositionEntity { get; set; }
            public DbSet<officeEntity> OfficeEntity { get; set; }
            //public DbSet<designationEntity> DesignationEntity { get; set; }
            public DbSet<educationalLevelEntity> EducationalLevelEntity { get; set; }
            public DbSet<collegeEntity> CollegeEntity { get; set; }
            public DbSet<courseEntity> CourseEntity { get; set; }
            //public DbSet<sectionEntity> SectionEntity { get; set; }
            //public DbSet<yearLevelEntity> YearLevelEntity { get; set; }
            public DbSet<studentSectionEntity> StudentSectionEntity { get; set; }
            public DbSet<sectionScheduleEntity> SectionScheduleEntity { get; set; }
            public DbSet<yearSectionEntity> YearSectionEntity { get; set; }
            public DbSet<employeeSubTypeEntity> EmployeeSubTypeEntity { get; set; }
            public DbSet<empTypeEntity> EmpTypeEntity { get; set; }
            public DbSet<schoolYearEntity> SchoolYearEntity { get; set; }
            public DbSet<schoolCalendarEntity> SchoolCalendarEntity { get; set; }
        #endregion

        #region Person Management
        public DbSet<personEntity> PersonEntity { get; set; }
            public DbSet<emergencyContactEntity> EmergencyContactEntity { get; set; }
            public DbSet<govIdsEntity> GovIdsEntity { get; set; }
            public DbSet<excusedStudentEntity> ExcusedStudentEntity { get; set; }
            public DbSet<dropoutCodeEntity> DropoutCodeEntity { get; set; }
        #endregion

        #region User Management
        public DbSet<userEntity> UserEntity { get; set; }
            public DbSet<roleEntity> RoleEntity { get; set; }
            public DbSet<userRoleEntity> UserRoleEntity { get; set; }
            public DbSet<userAccessEntity> UserAccessEntity { get; set; }
            public DbSet<rolePermissionEntity> RolePermissionEntity { get; set; }
            public DbSet<formEntity> FormEntity { get; set; }
        #endregion

        #region PAP
        public DbSet<papAccountEntity> PapAccountEntity { get; set; }
        public DbSet<papAccountLinkedStudentsEntity> PapAccountLinkedStudentsEntity { get; set; }

        public DbSet<announcementsEntity> AnnouncementsEntity { get; set; }
        public DbSet<announcementsRecipientsEntity> AnnouncementsRecipientsEntity { get; set; }

        public DbSet<calendarEntity> CalendarEntity { get; set; }
        public DbSet<calendarRecipientsEntity> CalendarRecipientsEntity { get; set; }

        public DbSet<digitalReferencesEntity> DigitalReferencesEntity { get; set; }

        #endregion

        #region Card Management
        public DbSet<cardDetailsEntity> CardDetailsEntity { get; set; }
        #endregion

        public DbSet<visitorInformationEntity> visitorInformation { get; set; }
        public DbSet<terminalEntity> TerminalEntity { get; set; }
        public DbSet<notificationEntity> NotificationEntity { get; set; }
        public DbSet<dailyLogsEntity> DailyLogsEntity { get; set; }

        public DbSet<datasyncEntity> DatasyncEntity { get; set; }
        public DbSet<datasyncFetcherEntity> DatasyncFetcherEntity { get; set; }
        public DbSet<datasyncEmergencyEntity> DataSyncEmergencyEntity { get; set; }
        public DbSet<terminalWhitelistEntity> TerminalWhitelistEntity { get; set; }
        public DbSet<terminalConfigurationEntity> TerminalConfigurationEntity { get; set; }
        public DbSet<batchUploadEntity> tbl_batch_upload { get; set; }

        #region Terminal Whitelist
        public DbSet<terminalWhitelistVM> terminalWhitelistVM { get; set; }
        public DbSet<terminalWhitelistCount> tbl_terminal_whitelist_count { get; set; }
        #endregion

        #region Reports_DBSet
        public DbSet<TotalHoursOfEmployeeVM> totalHoursEmployeeVM { get; set; }
        #endregion

        #region Reports
        public DbSet<reportAttendanceLogsEntity> ReportAttendanceLogsEntity { get; set; }
        public DbQuery<TimeAttendanceEmployeeEntity> Report_Time_Attendance_Employee { get; set; }
        public DbQuery<TimeAttendanceVisitorEntity> Report_Time_Attendance_Visitor { get; set; }
        public DbQuery<TimeAttendanceEntity> Report_Time_Attendance_Student { get; set; }
        public DbQuery<AuditTrail> Report_Audit_Trail { get; set; }
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

        public DbSet<scheduleEntity> ScheduleEntity { get; set; }
        public DbSet<fetcherScheduleEntity> FetcherScheduleEntity { get; set; }
        public DbSet<fetcherScheduleDetailsEntity> FetcherScheduleDetailsEntity { get; set; }
        public DbSet<fetcherGroupEntity> FetcherGroupEntity { get; set; }
        public DbSet<fetcherGroupDetailsEntity> FetcherGroupDetailsEntity { get; set; }

        public DbSet<emergencyLogoutEntity> EmergencyLogoutEntity { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<campusEntity>().HasKey(g => new { g.Campus_ID });

            modelBuilder.Entity<courseEntity>().HasIndex(u => new { u.Course_ID }).IsUnique();

            modelBuilder.Entity<reportAttendanceLogsEntity>().Ignore(t => t.Added_By);
            modelBuilder.Entity<reportAttendanceLogsEntity>().Ignore(t => t.Updated_By);
            modelBuilder.Entity<reportAttendanceLogsEntity>().Ignore(t => t.IsActive);
            modelBuilder.Entity<reportAttendanceLogsEntity>().Ignore(t => t.ToDisplay);
            modelBuilder.Entity<reportAttendanceLogsEntity>().Ignore(t => t.Date_Time_Added);
            modelBuilder.Entity<reportAttendanceLogsEntity>().Ignore(t => t.Last_Updated);

            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<courseEntity>().HasKey(g => new { g.Course_ID });

            /*modelBuilder.Entity<tbl_user_role_access>()
            .HasOne(s => s.tbl_form)
            .WithMany(c => c.user_role_access)
            .HasForeignKey(s => s.Form_Code)
            .HasPrincipalKey(c => c.Form_Code);*/
        }
    }
}

