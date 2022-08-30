using MyCampusV2.DAL.IRepositories;
using MyCampusV2.DAL.IRepositories.Report;
using System;

namespace MyCampusV2.DAL.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        bool isDisposed { get; }

        //-- Declare variable get for every new entity repository.
        IUserRepository UserRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        ICampusRepository CampusRepository { get; }
        IPersonRepository PersonRepository { get; }
        IDepartmentRepository DepartmentRepository { get; }
        IOfficeRepository OfficeRepository { get; }
        IPositionRepository PositionRepository { get;}
        //IFailedLogsRepository FailedLogsRepository { get; }
        IEducationLevelRepository EducationLevelRepository { get; }
        ICollegeRepository CollegeRepository { get; }
        ICourseRepository CourseRepository { get; }
        IFormRepository FormRepository { get; }
        IVisitorRepository VisitorRepository { get; }
        IAreaRepository AreaRepository { get; }
        IAuditTrailRepository AuditTrailRepository { get; }
        IRoleRepository RoleRepository { get; }
        ITerminalRepository TerminalRepository { get; }
        IReportRepository ReportRepository { get; }
        INotificationRepository NotificationRepository { get; }
        ITerminalConfigurationRepository TerminalConfigurationRepository { get; }

        ICardDetailsRepository CardDetailsRepository { get; }
        IDataSyncRepository DataSyncRepository { get; }
        ITerminalWhiteListRepository TerminalWhiteListRepository { get; }

        IRegionRepository RegionRepository { get; }
        IDivisionRepository DivisionRepository { get; }
        IBatchUploadRepository BatchUploadRepository { get; }

        ICampusReportRepository CampusReportRepository { get; }
        IEducationLevelReportRepository EducationLevelReportRepository { get; }
        ICollegeReportRepository CollegeReportRepository { get; }
        ICourseReportRepository CourseReportRepository { get; }
        IDepartmentReportRepository DepartmentReportRepository { get; }
        IOfficeReportRepository OfficeReportRepository { get; }
        IPositionReportRepository PositionReportRepository { get; }
        IAreaReportRepository AreaReportRepository { get; }
        IPersonReportRepository PersonReportRepository { get; }
        ICardDetailsReportRepository CardDetailsReportRepository { get; }
        ISectionReportRepository SectionReportRepository { get; }


        IScheduleRepository ScheduleRepository { get; }
        IEmployeeTypeRepository EmployeeTypeRepository { get; }
        IEmployeeSubTypeRepository EmployeeSubTypeRepository { get; }

        IYearSectionRepository YearSectionRepository { get; }
        IStudentSectionRepository StudentSectionRepository { get; }
        ISectionScheduleRepository SectionScheduleRepository { get; }
        ISchoolYearRepository SchoolYearRepository { get; }
        ISchoolCalendarRepository SchoolCalendarRepository { get; }

        IEventLoggingRepository EventLoggingRepository { get; }

        IGroupRepository GroupRepository { get; }
        IGroupDetailsRepository GroupDetailsRepository { get; }

        IEmergencyLogoutRepository EmergencyLogoutRepository { get; }
        IDepedReportRepository DepedReportRepository { get; }

        IDatasyncEmergencyLogoutRepository DatasyncEmergencyLogoutRepository { get; }
        IDatasyncFetcherRepository DatasyncFetcherRepository { get; }

        IFetcherScheduleDetailsRepository FetcherScheduleDetailsRepository { get; }
        IFetcherScheduleRepository FetcherScheduleRepository { get; }

        IPAPAccountRepository PAPAccountRepository { get; }
        IPAPAccountLinkedStudentsRepository PAPAccountLinkedStudentsRepository { get; }
        IAnnouncementsRepository AnnouncementsRepository { get; }
        IAnnouncementsRecipientsRepository AnnouncementsRecipientsRepository { get; }
        ICalendarRepository CalendarRepository { get; }
        ICalendarRecipientsRepository CalendarRecipientsRepository { get; }
        IDigitalReferencesRepository DigitalReferencesRepository { get; }
    }
}