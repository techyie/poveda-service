using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.DAL.IRepositories.Report;
using MyCampusV2.DAL.Repositories;
using MyCampusV2.DAL.Repositories.Report;
using System;

namespace MyCampusV2.DAL.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private MyCampusCardContext context;
        private MyCampusCardReportContext reportContext;

        #region === UnitOfWork Constructor ===

        public UnitOfWork(MyCampusCardContext Context, MyCampusCardReportContext ReportContext)
        {
            this.context = Context;
            this.reportContext = ReportContext;
        }

        #endregion

        #region === IUnitOfWork Methods ===
        public void Save()
        {
            context.SaveChanges();
        }

        public bool isDisposed
        {
            get { return disposed; }
        }

        #endregion

        #region === IDisposable Members ===

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            this.Dispose(false);
        }

        #endregion

        #region === IUnitOfWork Properties ===

        //-- Add new property below for every new entity repository.
        private IFetcherScheduleDetailsRepository fetcherScheduleDetailsRepository;
        public IFetcherScheduleDetailsRepository FetcherScheduleDetailsRepository
        {
            get
            {
                if (this.fetcherScheduleDetailsRepository == null)
                    this.fetcherScheduleDetailsRepository = new FetcherScheduleDetailsRepository(context);

                return fetcherScheduleDetailsRepository;
            }
        }

        private IFetcherScheduleRepository fetcherScheduleRepository;
        public IFetcherScheduleRepository FetcherScheduleRepository
        {
            get
            {
                if (this.fetcherScheduleRepository == null)
                    this.fetcherScheduleRepository = new FetcherScheduleRepository(context);

                return fetcherScheduleRepository;
            }
        }

        private IDatasyncFetcherRepository datasyncFetcherRepository;
        public IDatasyncFetcherRepository DatasyncFetcherRepository
        {
            get
            {
                if (this.datasyncFetcherRepository == null)
                    this.datasyncFetcherRepository = new DatasyncFetcherRepository(context);

                return datasyncFetcherRepository;
            }
        }

        private IDatasyncEmergencyLogoutRepository datasyncEmergencyLogoutRepository;
        public IDatasyncEmergencyLogoutRepository DatasyncEmergencyLogoutRepository
        {
            get
            {
                if (this.datasyncEmergencyLogoutRepository == null)
                    this.datasyncEmergencyLogoutRepository = new DatasyncEmergencyLogoutRepository(context);

                return datasyncEmergencyLogoutRepository;
            }
        }

        private INotificationRepository notificationRepository;
        public INotificationRepository NotificationRepository
        {
            get
            {
                if (this.notificationRepository == null)
                    this.notificationRepository = new NotificationRepository(context);

                return notificationRepository;
            }
        }

        private IReportRepository reportRepository;
        public IReportRepository ReportRepository
        {
            get
            {
                if (this.reportRepository == null)
                    this.reportRepository = new ReportRepository(reportContext);

                return reportRepository;
            }
        }

        private ITerminalRepository terminalRepository;
        public ITerminalRepository TerminalRepository
        {
            get
            {
                if (this.terminalRepository == null)
                    this.terminalRepository = new TerminalRepository(context);

                return terminalRepository;
            }
        }

        private ITerminalConfigurationRepository terminalConfigurationRepository;
        public ITerminalConfigurationRepository TerminalConfigurationRepository
        {
            get
            {
                if (this.terminalConfigurationRepository == null)
                    this.terminalConfigurationRepository = new TerminalConfigurationRepository(context);

                return terminalConfigurationRepository;
            }
        }

        private IRoleRepository roleRepository;
        public IRoleRepository RoleRepository
        {
            get
            {
                if (this.roleRepository == null)
                    this.roleRepository = new RoleRepository(context);

                return roleRepository;
            }
        }
        private IAreaRepository areaRepository;
        public IAreaRepository AreaRepository
        {
            get
            {
                if (this.areaRepository == null)
                    this.areaRepository = new AreaRepository(context);

                return areaRepository;
            }
        }

        private IUserRepository userRepository;
        public IUserRepository UserRepository
        {
            get
            {
                if (this.userRepository == null)
                    this.userRepository = new UserRepository(context);

                return userRepository;
            }
        }

        private IUserRoleRepository userRoleRepository;
        public IUserRoleRepository UserRoleRepository
        {
            get
            {
                if (this.userRoleRepository == null)
                    this.userRoleRepository = new UserRoleRepository(context);

                return userRoleRepository;
            }
        }

        private IAuditTrailRepository auditTrailRepository;
        public IAuditTrailRepository AuditTrailRepository
        {
            get
            {
                if (this.auditTrailRepository == null)
                    this.auditTrailRepository = new AuditTrailRepository(context);

                return auditTrailRepository;
            }
        }

        //private IFailedLogsRepository failedLogsRepository;
        //public IFailedLogsRepository FailedLogsRepository
        //{
        //    get
        //    {
        //        if (this.failedLogsRepository == null)
        //            this.failedLogsRepository = new FailedLogsRepository(context);

        //        return failedLogsRepository;
        //    }
        //}

        private ICampusRepository campusRepository;
        public ICampusRepository CampusRepository
        {
            get
            {
                if (this.campusRepository == null)
                    this.campusRepository = new CampusRepository(context);

                return campusRepository;
            }
        }

        private IPersonRepository personRepository;
        public IPersonRepository PersonRepository
        {
            get
            {
                if (this.personRepository == null)
                    this.personRepository = new PersonRepository(context);

                return personRepository;
            }
        }   

        private IDepartmentRepository departmentRepository;
        public IDepartmentRepository DepartmentRepository
        {
            get
            {
                if (this.departmentRepository == null)
                    this.departmentRepository = new DepartmentRepository(context);

                return departmentRepository;
            }
        }

        private IPositionRepository positionRepository;
        public IPositionRepository PositionRepository
        {
            get
            {
                if (this.positionRepository == null)
                    this.positionRepository = new PositionRepository(context);

                return positionRepository;
            }
        }

        private IEducationLevelRepository educationLevelRepository;
        public IEducationLevelRepository EducationLevelRepository
        {
            get
            {
                if (this.educationLevelRepository == null)
                    this.educationLevelRepository = new EducationLevelRepository(context);

                return educationLevelRepository;
            }
        }

        private ICollegeRepository collegeRepository;
        public ICollegeRepository CollegeRepository
        {
            get
            {
                if (this.collegeRepository == null)
                    this.collegeRepository = new CollegeRepository(context);

                return collegeRepository;
            }
        }

        private ICourseRepository courseRepository;
        public ICourseRepository CourseRepository
        {
            get
            {
                if (this.courseRepository == null)
                    this.courseRepository = new CourseRepository(context);

                return courseRepository;
            }
        }

        private IFormRepository formRepository;
        public IFormRepository FormRepository
        {
            get
            {
                if (this.formRepository == null)
                    this.formRepository = new FormRepository(context);

                return formRepository;
            }
        }

        private IVisitorRepository visitorRepository;
        public IVisitorRepository VisitorRepository
        {
            get
            {
                if (this.visitorRepository == null)
                    this.visitorRepository = new VisitorRepository(context);

                return visitorRepository;
            }
        }


        private IDataSyncRepository dataSyncRepository;
        public IDataSyncRepository DataSyncRepository
        {
            get
            {
                if (this.dataSyncRepository == null)
                    this.dataSyncRepository = new DataSyncRepository(context);

                return dataSyncRepository;
            }
        }

        private ITerminalWhiteListRepository terminalWhiteListRepository;
        public ITerminalWhiteListRepository TerminalWhiteListRepository
        {
            get
            {
                if (this.terminalWhiteListRepository == null)
                    this.terminalWhiteListRepository = new TerminalWhiteListRepository(context);

                return terminalWhiteListRepository;
            }
        }

        private ICardDetailsRepository cardDetailsRepository;
        public ICardDetailsRepository CardDetailsRepository
        {
            get
            {
                if (this.cardDetailsRepository == null)
                    this.cardDetailsRepository = new CardDetailsRepository(context);

                return cardDetailsRepository;
            }
        }

        private IRegionRepository regionRepository;
        public IRegionRepository RegionRepository
        {
            get
            {
                if (this.regionRepository == null)
                    this.regionRepository = new RegionRepository(context);

                return regionRepository;
            }
        }

        private IDivisionRepository divisionRepository;
        public IDivisionRepository DivisionRepository
        {
            get
            {
                if (this.divisionRepository == null)
                    this.divisionRepository = new DivisionRepository(context);

                return divisionRepository;
            }
        }

        private IBatchUploadRepository batchUploadRepository;
        public IBatchUploadRepository BatchUploadRepository
        {
            get
            {
                if (this.batchUploadRepository == null)
                    this.batchUploadRepository = new BatchUploadRepository(context);

                return batchUploadRepository;
            }
        }

        private ICampusReportRepository campusReportRepository;
        public ICampusReportRepository CampusReportRepository
        {
            get
            {
                if (this.campusReportRepository == null)
                    this.campusReportRepository = new CampusReportRepository(reportContext);

                return campusReportRepository;
            }
        }

        private IEducationLevelReportRepository educationLevelReportRepository;
        public IEducationLevelReportRepository EducationLevelReportRepository
        {
            get
            {
                if (this.educationLevelReportRepository == null)
                    this.educationLevelReportRepository = new EducationLevelReportRepository(reportContext);

                return educationLevelReportRepository;
            }
        }

        //private IYearLevelReportRepository yearLevelReportRepository;
        //public IYearLevelReportRepository YearLevelReportRepository
        //{
        //    get
        //    {
        //        if (this.yearLevelReportRepository == null)
        //            this.yearLevelReportRepository = new YearLevelReportRepository(reportContext);

        //        return yearLevelReportRepository;
        //    }
        //}

        private ISectionReportRepository sectionReportRepository;
        public ISectionReportRepository SectionReportRepository
        {
            get
            {
                if (this.sectionReportRepository == null)
                    this.sectionReportRepository = new SectionReportRepository(reportContext);

                return sectionReportRepository;
            }
        }

        private ICollegeReportRepository collegeReportRepository;
        public ICollegeReportRepository CollegeReportRepository
        {
            get
            {
                if (this.collegeReportRepository == null)
                    this.collegeReportRepository = new CollegeReportRepository(reportContext);

                return collegeReportRepository;
            }
        }

        private ICourseReportRepository courseReportRepository;
        public ICourseReportRepository CourseReportRepository
        {
            get
            {
                if (this.courseReportRepository == null)
                    this.courseReportRepository = new CourseReportRepository(reportContext);

                return courseReportRepository;
            }
        }

        private IDepartmentReportRepository departmentReportRepository;
        public IDepartmentReportRepository DepartmentReportRepository
        {
            get
            {
                if (this.departmentReportRepository == null)
                    this.departmentReportRepository = new DepartmentReportRepository(reportContext);

                return departmentReportRepository;
            }
        }

        private IOfficeReportRepository officeReportRepository;
        public IOfficeReportRepository OfficeReportRepository
        {
            get
            {
                if (this.officeReportRepository == null)
                    this.officeReportRepository = new OfficeReportRepository(reportContext);

                return officeReportRepository;
            }
        }

        private IPositionReportRepository positionReportRepository;
        public IPositionReportRepository PositionReportRepository
        {
            get
            {
                if (this.positionReportRepository == null)
                    this.positionReportRepository = new PositionReportRepository(reportContext);

                return positionReportRepository;
            }
        }

        private IAreaReportRepository areaReportRepository;
        public IAreaReportRepository AreaReportRepository
        {
            get
            {
                if (this.areaReportRepository == null)
                    this.areaReportRepository = new AreaReportRepository(reportContext);

                return areaReportRepository;
            }
        }

        private IPersonReportRepository personReportRepository;
        public IPersonReportRepository PersonReportRepository
        {
            get
            {
                if (this.personReportRepository == null)
                    this.personReportRepository = new PersonReportRepository(reportContext);

                return personReportRepository;
            }
        }

        private ICardDetailsReportRepository cardDetailsReportRepository;
        public ICardDetailsReportRepository CardDetailsReportRepository
        {
            get
            {
                if (this.cardDetailsReportRepository == null)
                    this.cardDetailsReportRepository = new CardDetailsReportRepository(reportContext);

                return cardDetailsReportRepository;
            }
        }

        private IScheduleRepository scheduleRepository;
        public IScheduleRepository ScheduleRepository
        {
            get
            {
                if (this.scheduleRepository == null)
                    this.scheduleRepository = new ScheduleRepository(context);

                return scheduleRepository;
            }
        }

        private IEmployeeTypeRepository employeeTypeRepository;
        public IEmployeeTypeRepository EmployeeTypeRepository
        {
            get
            {
                if (this.employeeTypeRepository == null)
                    this.employeeTypeRepository = new EmployeeTypeRepository(context);

                return employeeTypeRepository;
            }
        }

        private IEmployeeSubTypeRepository employeeSubTypeRepository;
        public IEmployeeSubTypeRepository EmployeeSubTypeRepository
        {
            get
            {
                if (this.employeeSubTypeRepository == null)
                    this.employeeSubTypeRepository = new EmployeeSubTypeRepository(context);

                return employeeSubTypeRepository;
            }
        }

        private IOfficeRepository officeRepository;
        public IOfficeRepository OfficeRepository
        {
            get
            {
                if (this.officeRepository == null)
                    this.officeRepository = new OfficeRepository(context);

                return officeRepository;
            }
        }

        private IYearSectionRepository yearSectionRepository;
        public IYearSectionRepository YearSectionRepository
        {
            get
            {
                if (this.yearSectionRepository == null)
                    this.yearSectionRepository = new YearSectionRepository(context);

                return yearSectionRepository;
            }
        }

        private IStudentSectionRepository studentSectionRepository;
        public IStudentSectionRepository StudentSectionRepository
        {
            get
            {
                if (this.studentSectionRepository == null)
                    this.studentSectionRepository = new StudentSectionRepository(context);

                return studentSectionRepository;
            }
        }

        private ISectionScheduleRepository sectionScheduleRepository;
        public ISectionScheduleRepository SectionScheduleRepository
        {
            get
            {
                if (this.sectionScheduleRepository == null)
                    this.sectionScheduleRepository = new SectionScheduleRepository(context);

                return sectionScheduleRepository;
            }
        }


        private ISchoolYearRepository schoolYearRepository;
        public ISchoolYearRepository SchoolYearRepository
        {
            get
            {
                if (this.schoolYearRepository == null)
                    this.schoolYearRepository = new SchoolYearRepository(context);

                return schoolYearRepository;
            }
        }

        private ISchoolCalendarRepository schoolCalendarRepository;
        public ISchoolCalendarRepository SchoolCalendarRepository
        {
            get
            {
                if (this.schoolCalendarRepository == null)
                    this.schoolCalendarRepository = new SchoolCalendarRepository(context);

                return schoolCalendarRepository;
            }
        }

        private IEventLoggingRepository eventLoggingRepository;
        public IEventLoggingRepository EventLoggingRepository
        {
            get
            {
                if (this.eventLoggingRepository == null)
                    this.eventLoggingRepository = new EventLoggingRepository(context);

                return eventLoggingRepository;
            }
        }

        private IGroupRepository groupRepository;
        public IGroupRepository GroupRepository
        {
            get
            {
                if (this.groupRepository == null)
                    this.groupRepository = new GroupRepository(context);

                return groupRepository;
            }
        }

        private IGroupDetailsRepository groupDetailsRepository;
        public IGroupDetailsRepository GroupDetailsRepository
        {
            get
            {
                if (this.groupDetailsRepository == null)
                    this.groupDetailsRepository = new GroupDetailsRepository(context);

                return groupDetailsRepository;
            }
        }

        private IEmergencyLogoutRepository emergencyLogoutRepository;
        public IEmergencyLogoutRepository EmergencyLogoutRepository
        {
            get
            {
                if (this.emergencyLogoutRepository == null)
                    this.emergencyLogoutRepository = new EmergencyLogoutRepository(context);

                return emergencyLogoutRepository;
            }
        }

        private IDepedReportRepository depedReportRepository;
        public IDepedReportRepository DepedReportRepository
        {
            get
            {
                if (this.depedReportRepository == null)
                    this.depedReportRepository = new DepedReportRepository(context);

                return depedReportRepository;
            }
        }

        private IPAPAccountRepository papAccountRepository;
        public IPAPAccountRepository PAPAccountRepository
        {
            get
            {
                if (this.papAccountRepository == null)
                    this.papAccountRepository = new PAPAccountRepository(context);

                return papAccountRepository;
            }
        }

        private IPAPAccountLinkedStudentsRepository papAccountLinkedStudentsRepository;
        public IPAPAccountLinkedStudentsRepository PAPAccountLinkedStudentsRepository
        {
            get
            {
                if (this.papAccountLinkedStudentsRepository == null)
                    this.papAccountLinkedStudentsRepository = new PAPAccountLinkedStudentsRepository(context);

                return papAccountLinkedStudentsRepository;
            }
        }

        private IAnnouncementsRepository announcementsRepository;
        public IAnnouncementsRepository AnnouncementsRepository
        {
            get
            {
                if (this.announcementsRepository == null)
                    this.announcementsRepository = new AnnouncementsRepository(context);

                return announcementsRepository;
            }
        }

        private IAnnouncementsRecipientsRepository announcementsRecipientsRepository;
        public IAnnouncementsRecipientsRepository AnnouncementsRecipientsRepository
        {
            get
            {
                if (this.announcementsRecipientsRepository == null)
                    this.announcementsRecipientsRepository = new AnnouncementsRecipientsRepository(context);

                return announcementsRecipientsRepository;
            }
        }

        private ICalendarRepository calendarRepository;
        public ICalendarRepository CalendarRepository
        {
            get
            {
                if (this.calendarRepository == null)
                    this.calendarRepository = new CalendarRepository(context);

                return calendarRepository;
            }
        }

        private ICalendarRecipientsRepository calendarRecipientsRepository;
        public ICalendarRecipientsRepository CalendarRecipientsRepository
        {
            get
            {
                if (this.calendarRecipientsRepository == null)
                    this.calendarRecipientsRepository = new CalendarRecipientsRepository(context);

                return calendarRecipientsRepository;
            }
        }

        private IDigitalReferencesRepository digitalReferencesRepository;
        public IDigitalReferencesRepository DigitalReferencesRepository
        {
            get
            {
                if (this.digitalReferencesRepository == null)
                    this.digitalReferencesRepository = new DigitalReferencesRepository(context);

                return digitalReferencesRepository;
            }
        }
        #endregion
    }
}