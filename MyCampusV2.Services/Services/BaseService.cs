using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using System;

namespace MyCampusV2.Services
{
    public class BaseService
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IAuditTrailService _audit;
        public readonly ICurrentUser _user;

        public BaseService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user)
        {
            _unitOfWork = unitOfWork;
            _audit = audit;
            _user = user;
        }
        
        protected string CAMPUS_EXIST = "Campus already exists!";
        protected string EDUCATIONAL_LEVEL_EXIST = "Educational Level already exists!";
        protected string COLLEGE_EXIST = "College already exists!";
        protected string COURSE_EXIST = "Course already exists!";
        protected string YEAR_SECTION_EXIST = "Year Level already exists!";
        protected string STUDENT_SECTION_EXIST = "Student Section already exists!";
        protected string SECTION_SCHEDULE_EXIST = "Section Schedule already exists!";
        protected string SECTION_RANGE_SCHEDULE_EXIST = "Section Schedule for the selected date range already exists!";
        protected string SCHOOL_YEAR_EXIST = "School Year already exists!";
        protected string DEPARTMENT_EXIST = "Department already exists!";
        protected string POSITION_EXIST = "Position already exists!";
        protected string EMPLOYEE_TYPE_EXIST = "Employee Type already exists!";
        protected string EMPLOYEE_SUB_TYPE_EXIST = "Employee Sub Type already exists!";
        protected string OFFICE_EXIST = "Office already exists!";
        protected string GROUP_EXIST = "Group already exists!";
        protected string SCHEDULE_EXIST = "Schedule already exists!";
        protected string ASSIGNMENT_EXIST = "Assignment already exists!";
        protected string AREA_EXIST = "Area already exists!";
        protected string TERMINAL_EXIST = "Terminal already exists!";
        protected string GENERAL_NOTIFICATION_EXIST = "General Notification already exists!";
        protected string PERSONAL_NOTIFICATION_EXIST = "Personal Notification already exists!";
        protected string ROLE_EXIST = "Role already exists!";

        protected string EMERGENCY_LOGOUT_EXIST = "Emergency Logout already exists!";
        protected string STUDENT_EXCUSE_EXIST = "Student Excused Date already exists!";
        protected string STUDENT_EXCUSE_RANGE_EXIST = "Student Excused Dates for the selected range already exist!";
        protected string FETCHER_GROUP_STUDENT = "Student under Fetcher Group already exists!";
        protected string FETCHER_SCHEDULE_STUDENT = "Schedule under this Fetcher already exists!";
        protected string FETCHER_MAX_LIMIT = "Student has reached the maximum number of allowed fetchers!";

        protected string PERSON_OTHERACCESS_EXIST = "Other Access already exists!";
        protected string PERSON_FETCHER_EXIST = "Fetcher already exists!";
        protected string PERSON_VISITOR_EXIST = "Visitor already exists!";
        protected string PERSON_EMPLOYEE_EXIST = "Employee already exists!";
        protected string PERSON_STUDENT_EXIST = "Student already exists!";

        protected string ITEM_IN_USE = "Unable to deactivate. Item is in use.";
        protected string UNABLE_DELETE = "Unable to delete. Item is in use.";
        protected string UNABLE_EDIT = "Unable to edit. Item is in use.";

        protected string PERSON_FETCHER_NUMBER_EXIST = "Fetcher Contact Number Already Exist";

        protected string PAP_ACCOUNT_EXIST = "Account already exists!";

        protected enum Form
        {
            Card_Card = 5,
            Person_Student = 10,
            Campus_Campus = 12,
            Person_Employee = 13,
            User_Role = 14,
            Campus_EducationalLevel = 15,
            Campus_Year = 16,
            Campus_Section = 17,
            Campus_College = 18,
            Campus_Course = 19,
            Campus_Department = 21,
            Campus_Position = 22,
            Campus_Designation = 23,
            Campus_Area = 24,
            Device_Terminal = 33,
            Notification_General = 35,
            Notification_Personal = 36,
            Batch_Upload = 37,
            User_List = 37,
            Card_Assign = 38,
            Card_Update = 39,
            Card_Deactivate = 40,
            Card_Reassign = 41,
            Person_Visitor = 55,
            Person_Fetcher = 60,
            Person_OtherAccess = 61,
            Campus_Employee_Type = 62,
            Campus_Employee_Sub_Type = 63,
            Fetcher_Schedule = 67,
            Fetcher_Group = 68,
            Fetcher_Assign = 69,
            Fetcher_Emergency_Logout = 70,
            Campus_School_Calendar = 71,
            Deped_Report = 72,
            Visitor_Report = 73,
            Library_Attendance = 74,
            Library_Usage = 75,
            Campus_Office = 76,
            Mobile_App_Account = 76,
            Announcements = 77,
            Digital_References = 78,
            App_Calendar = 79
        }

        public eventLoggingEntity fillEventLogging(int userId, int formId, string source, string category, bool logLevel, string message, DateTime logDateTime)
        {
            eventLoggingEntity eventLogging = new eventLoggingEntity();
            eventLogging.User_ID = userId;
            eventLogging.Form_ID = formId;
            eventLogging.Source = source;
            eventLogging.Category = category;
            eventLogging.Log_Level = logLevel;
            eventLogging.Message = message;
            eventLogging.Log_Date_Time = logDateTime;
            return eventLogging;
        }

        public ResultModel CreateResult(string Code, string Message, bool isSuccess)
        {
            ResultModel result = new ResultModel();
            result.resultCode = Code;
            result.resultMessage = Message;
            result.isSuccess = isSuccess;
            return result;
        }

        public string UNABLE_ACTIVATE(string title, string name)
        {
            return "Unable to activate. " + title.Trim() + " " + name.Trim() + " is inactive."; 
        }

    }
}
