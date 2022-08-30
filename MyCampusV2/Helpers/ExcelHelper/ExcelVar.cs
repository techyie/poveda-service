using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Helpers.ExcelHelper
{
    public static class ExcelVar
    {
        public static string Author = "MyCampusCard";
        public static string Company = "POVEDA";
        public static string dateFormat = "yyyy-MM-dd";

        // Headers with * are required fields
        public static string CampusTitle = "Campus List";
        public static string CampusTemplateTitle = "Campus Template";
        public static string[] CampusColHeader = new string[] { "CampusCode*", "CampusName*", "CampusStatus*", "CampusAddress*", "CampusContactNo*", "Region*", "Division*" };
        public static string[] CampusSampleData = new string[] { "POVEDA CAMPUS", "POVEDA CAMPUS NAME", "ACTIVE", "POVEDA ADDRESS", "9999999", "Region 1", "Division 1" };

        public static string EducLevelTitle = "Educational Level List";
        public static string EducLevelTemplateTitle = "Educational Level Template";
        public static string[] EducLevelColHeader = new string[] { "CampusName*", "EducationalLevelName*", "College*" };
        public static string[] EducLevelSampleData = new string[] { "POVEDA CAMPUS", "EDUCATIONAL LEVEL 1", "Yes or No" };
        
        public static string YearLevelTitle = "Year Level List";
        public static string YearLevelTemplateTitle = "Year Level Template";
        public static string[] YearLevelColHeader = new string[] { "CampusName*", "EducationalLevelName*", "YearLevelName*" };
        public static string[] YearLevelSampleData = new string[] { "POVEDA CAMPUS", "COLLEGE", "1ST YR COLLEGE" };

        public static string SectionTitle = "Section List";
        public static string[] SectionColHeader = new string[] { "CampusName*", "EducationalLevelName*", "YearLevelName*", "SectionName*", "StartTime*", "EndTime*", "HalfDay*", "GracePeriod*" };
        public static string[] SectionSampleData = new string[] { "POVEDA CAMPUS", "GRADE SCHOOL", "GRADE 1", "BLOCK A", "00:00", "00:00", "00:00", "0" };

        public static string CollegeTitle = "College List";
        public static string CollegeTemplateTitle = "College Template";
        public static string[] CollegeColHeader = new string[] { "CampusName*", "EducationalLevelName*", "CollegeName*" };
        public static string[] CollegeSampleData = new string[] { "POVEDA CAMPUS", "COLLEGE", "CIT" };

        public static string CourseTitle = "Course List";
        public static string CourseTemplateTitle = "Course Template";
        public static string[] CourseColHeader = new string[] { "CampusName*", "EducationalLevelName*", "CollegeName*", "CourseName*" };
        public static string[] CourseSampleData = new string[] { "POVEDA CAMPUS", "COLLEGE", "CIT", "COMSCI" };

        public static string DepartmentTitle = "Department List";
        public static string DepartmentTemplateTitle = "Department Template";
        public static string[] DepartmentColHeader = new string[] { "CampusName*", "DepartmentName*" };
        public static string[] DepartmentSampleData = new string[] { "POVEDA CAMPUS", "HR" };

        public static string OfficeTitle = "Office List";
        public static string OfficeTemplateTitle = "Office Template";
        public static string[] OfficeColHeader = new string[] { "CampusName*", "OfficeName*" };
        public static string[] OfficeSampleData = new string[] { "POVEDA CAMPUS", "OFFICE 1" };

        public static string PositionTitle = "Position List";
        public static string PositionTemplateTitle = "Position Template";
        public static string[] PositionColHeader = new string[] { "CampusName*", "DepartmentName*", "PositionName*" };
        public static string[] PositionSampleData = new string[] { "POVEDA CAMPUS", "HR", "HR SPECIALIST" };

        public static string AreaTitle = "Area List";
        public static string AreaTemplateTitle = "Area Template";
        public static string[] AreaColHeader = new string[] { "CampusName*", "AreaName*", "AreaDescription*" };
        public static string[] AreaSampleData = new string[] { "POVEDA CAMPUS", "GATE 1", "AREA GATE ONE" };

        public static string EmergencyLogoutStudentsTitle = "Emergency Logout Students List";
        public static string[] EmergencyLogoutStudentsColHeader = new string[] { "Student*", "Remarks*", "Date*" };
        
        public static string SchoolCalendarTitle = "School Calendar List";
        public static string SchoolCalendarTemplateTitle = "School Calendar Template";

        public static string DepedReportTitle = "DepEd Report";

        public static string StudentTemplateTitle = "Student Template";
        public static string StudentTitle = "Student List";

        public static string[] StudentColHeader = new string[] { "IDNumber*", "FirstName*", "MiddleName", "LastName*", "BirthDate*", "Gender*",
            "ContactNumber", "TelephoneNumber", "EmailAddress", "Address*", "CampusName*", "EducationalLevelName*", "YearLevelName*", "SectionName*", "DateEnrolled*",
            "EmergencyFullname*", "EmergencyAddress*", "EmergencyContactNo*", "EmergencyRelationship*"};
        public static string[] StudentSampleData = new string[] { "S-001", "JUAN", "BONIFACIO", "DELA CRUZ", "YYYY-MM-DD", "M", "09123456789", "1234567",
            "juandelacruz@gmail.com", "MANILA", "POVEDA CAMPUS", "EDUCATIONAL LEVEL 1", "1ST YR COLLEGE", "BLOCK A", "YYYY-MM-DD", "JUANITA DELA CRUZ",
            "MANILA", "09123456789", "MOTHER"};
        
        public static string[] StudentCollegeColHeader = new string[] { "IDNumber*", "FirstName*", "MiddleName", "LastName*", "BirthDate*", "Gender*",
            "ContactNumber", "TelephoneNumber", "EmailAddress", "Address*", "CampusName*", "EducationalLevelName*", "YearLevelName*", "SectionName*",
            "CollegeName*", "CourseName*",  "DateEnrolled*", "EmergencyFullname*", "EmergencyAddress*", "EmergencyContactNo*", "EmergencyRelationship*"};
        public static string[] StudentCollegeSampleData = new string[] { "S-001", "JUAN", "BONIFACIO", "DELA CRUZ", "YYYY-MM-DD", "M", "09123456789", "1234567",
            "juandelacruz@gmail.com", "MANILA", "POVEDA CAMPUS", "EDUCATIONAL LEVEL 1", "1ST YR COLLEGE", "BLOCK A", "CIT", "BSCS", "YYYY-MM-DD", "JUANITA DELA CRUZ",
            "MANILA", "09123456789", "MOTHER"};

        public static string EmployeeTemplateTitle = "Employee Template";
        public static string EmployeeTitle = "Employee List";
        public static string[] EmployeeColHeader = new string[] { "IDNumber*", "FirstName*", "MiddleName", "LastName*", "BirthDate*", "Gender*",
            "ContactNumber*", "TelephoneNumber", "EmailAddress*", "Address*", "CampusName*", "EmployeeTypeName*", "EmployeeSubTypeName*", "DepartmentName*", "PositionName*", "EmergencyFullname*",
            "EmergencyAddress*", "EmergencyMobileNo*", "EmergencyRelationship*", "SSS*", "TIN*", "PAGIBIG*", "Philhealth*"};
        public static string[] EmployeeSampleData = new string[] { "E-001", "JUAN", "BONIFACIO", "DELA CRUZ", "YYYY-MM-DD", "M", "09123456789", "0000-0000",
            "juandelacruz@gmail.com", "MANILA", "POVEDA CAMPUS", "NON-TEACHING", "NON-FACULTY", "HR", "HR STAFF", "JUANITA DELA CRUZ", "MANILA",
            "09123456789", "SPOUSE", "00-0000000-0", "000-000-000-000", "0000-0000-0000", "00-000000000-0" };

        public static string VisitorTemplateTitle = "Visitor Template";
        public static string VisitorTitle = "Visitor List";
        public static string[] VisitorColHeader = new string[] { "IDNumber*", "FirstName*", "MiddleName", "LastName*", "BirthDate*", "Gender*",
            "ContactNumber*", "EmailAddress*", "Address*" };
        public static string[] VisitorSampleData = new string[] { "V-001", "JUAN", "BONIFACIO", "DELA CRUZ", "YYYY-MM-DD", "M", "09123456789",
            "juandelacruz@gmail.com", "MANILA" };

        public static string FetcherTemplateTitle = "Fetcher Template";
        public static string FetcherTitle = "Fetcher List";
        public static string[] FetcherColHeader = new string[] { "IDNumber*", "FirstName*", "MiddleName", "LastName*", "BirthDate*", "Gender*",
            "ContactNumber*", "EmailAddress", "Address*", "Relationship", "CampusName*" };
        public static string[] FetcherSampleData = new string[] { "F-001", "JUAN", "BONIFACIO", "DELA CRUZ", "YYYY-MM-DD", "M", "09123456789",
            "juandelacruz@gmail.com", "MANILA", "MOTHER", "POVEDA CAMPUS"};

        public static string OtherAccessTemplateTitle = "Other Access Template";
        public static string OtherAccessTitle = "Other Access List";
        public static string[] OtherAccessColHeader = new string[] { "IDNumber*", "FirstName*", "MiddleName", "LastName*", "BirthDate*", "Gender*",
            "ContactNumber*", "TelephoneNumber", "EmailAddress", "Address*", "CampusName*", "DepartmentName*", "PositionName*", "OfficeName*", "EmergencyFullname*",
            "EmergencyAddress*", "EmergencyMobileNo*", "EmergencyRelationship*" };
        public static string[] OtherAccessSampleData = new string[] { "OA-001", "JUAN", "BONIFACIO", "DELA CRUZ", "YYYY-MM-DD", "M", "09123456789", "0000-0000",
            "juandelacruz@gmail.com", "MANILA", "POVEDA CAMPUS", "HR", "HR STAFF", "CONSULTANT", "JUANITA DELA CRUZ", "MANILA", "09123456789", "SPOUSE" };

        public static string UpdateTitle = "Card Update List";
        public static string[] UpdateColHeader = new string[] { "IDNumber*", "ExpiryDate*" };
        public static string[] UpdateSampleData = new string[] { "S-001", "YYYY-MM-DD" };

        public static string DeactivateTitle = "Card Deactivate List";
        public static string[] DeactivateColHeader = new string[] { "IDNumber*" };
        public static string[] DeactivateSampleData = new string[] { "S-001" };
        
        public static string GeneralTitle = "General Notification List";
        public static string[] GeneralColHeader = new string[] { "Message*", "DateFrom*", "DateTo*" };
        public static string[] GeneralSampleData = new string[] { "TEST MESSAGE", "YYYY-MM-DD", "YYYY-MM-DD" };

        public static string PersonalTitle = "Personal Notification List";
        public static string[] PersonalColHeader = new string[] { "Message*", "DateFrom*", "DateTo*", "IDNumber*" };
        public static string[] PersonalSampleData = new string[] { "TEST MESSAGE", "YYYY-MM-DD", "YYYY-MM-DD", "S-001" };
        
        public static string[] TimeAndAttendanceColHeader = new string[] { "ID Number", "Full Name", "Log Date", "Log In", "Log Out" };

        public static string TAATitle = "Time and Attendance Report";

        /* ------------------------------------------------------------------ */

        public static string[] AlarmStudentColHeader = new string[] { "ID Number", "Name", "Educational Level", "Course / Year / Grade", "Section", "Log Message", "Date and Time Log" };
        public static string[] AlarmOtherAccessColHeader = new string[] { "ID Number", "Name", "Department", "Position", "Log Message", "Date and Time Log" };
        public static string[] AlarmEmployeeColHeader = new string[] { "ID Number", "Name", "Employee Sub-Type", "Department", "Position", "Log Message", "Date and Time Log" };
        public static string[] AlarmFetcherColHeader = new string[] { "ID Number", "Name", "Fetcher Relationship", "Log Message", "Date and Time Log" };
        public static string[] AlarmAllColHeader = new string[] { "ID Number", "Name", "Employee Sub-Type", "Department / Educational Level", "Position / Course / Year / Grade", "Student Section", "Log Message", "Date and Time Log" };

        public static string ATitle = "Alarm Report";

        /* ------------------------------------------------------------------ */

        public static string[] LibraryAttendanceAllColHeader = new string[] { "ID Number", "Full Name", "Department / Educational Level", "Position / Course / Year / Grade Level", "Section", "Log Date and Time", "Log Message" };
        public static string[] LibraryAttendanceStudentColHeader = new string[] { "ID Number", "Full Name", "Educational Level", "Course / Year / Grade Level", "Section", "Log Date and Time", "Log Message" };
        public static string[] LibraryAttendanceEmployeeColHeader = new string[] { "ID Number", "Full Name", "Department", "Position", "Log Date and Time", "Log Message" };

        public static string LATitle = "Library Attendance Report";

        /* ------------------------------------------------------------------ */

        public static string[] LibraryUsageAllColHeader = new string[] { "ID Number", "Full Name", "Department / Educational Level", "Position / Course / Year", "Section", "Date", "Usage Count" };
        public static string[] LibraryUsageStudentColHeader = new string[] { "ID Number", "Full Name", "Educational Level", "Course / Year", "Section", "Date", "Usage Count" };
        public static string[] LibraryUsageEmployeeColHeader = new string[] { "ID Number", "Full Name", "Department", "Position", "Date", "Usage Count" };

        public static string LUTitle = "Library Usage Report";

        /* ------------------------------------------------------------------ */

        public static string[] CardStudentColHeader = new string[] { "ID Number", "Full Name", "Educational Level", "Course / Year / Grade", "Date and Time Updated" };
        public static string[] CardEmployeeColHeader = new string[] { "ID Number", "Full Name", "Department", "Position", "Date and Time Updated" };
        public static string[] CardOtherAccessColHeader = new string[] { "ID Number", "Full Name", "Department", "Position", "Date and Time Updated" };
        public static string[] CardFetcherColHeader = new string[] { "ID Number", "Full Name", "Date and Time Updated" };
        public static string[] CardAllColHeader = new string[] { "ID Number", "Full Name", "Person Type", "Department / Educational Level", "Position / Course / Year / Grade", "Date and Time Updated" };

        public static string CTitle = "Card Report";

        /* ------------------------------------------------------------------ */

        public static string[] AuditTrailColHeader = new string[] { "User", "Source", "Category", "Status", "Message", "Date and Time" };
        public static string AuditTrailTitle = "Audit Trail Report";
        
        public static string[] CardColHeader = new string[] { "IDNumber", "Name", "CardNumber", "IssuedDate", "ExpiryDate", "CardStatus", "Remarks" };
        public static string CardTitle = "Card Report";

        public static string[] VisitorReportColHeader = new string[] { "ID Number", "Visitor Name", "Remarks", "Visited Employee", "Area", "Scheduled Date", "Date and Time Registered",
            "Log Date and Time", "Date and Time Surrendered" };
        public static string VisitorReportTitle = "Visitor Report";
        
        public static string[] TotalHoursColHeader = new string[] { "ID Number", "Name", "Department", "Employee Type", "Total Hours" };
        public static string TotalHoursTitle = "Total Hours of Employee Report";


        /* ------------------------------------------------------------------ */

        public static string AccountTitle = "Account List";
        public static string AccountTemplateTitle = "Account Template";
        public static string[] AccountColHeader = new string[] { "FirstName", "MiddleName", "LastName", "EmailAddress", "MobileNumber", "Username", "LinkedStudents", "Status" };
        public static string[] AccountColHeaderSample = new string[] { "FirstName*", "MiddleName", "LastName*", "EmailAddress*", "MobileNumber*", "LinkedStudents*" };
        public static string[] AccountSampleData = new string[] { "Juan", "Gatmaitan", "Dela Cruz", "jgdelacruz@allcard.com.ph", "09661722222", "10-0001;10-0002;10-0003;10-0004;10-0005" };

        public static string AnnouncementsTitle = "Announcements List";
        public static string AnnouncementsTemplateTitle = "Announcements Template";
        public static string[] AnnouncementsColHeader = new string[] { "Title", "Body", "DateSent", "Recipients" };
        public static string[] AnnouncementsSampleData = new string[] { "", "", "", "" };

        public static string CalendarTitle = "Calendar List";
        public static string CalendarTemplateTitle = "Calendar Template";
        public static string[] CalendarColHeader = new string[] { "Title", "Body", "PostDate", "Recipients" };
        public static string[] CalendarSampleData = new string[] { "", "", "", "" };

        public static string DigitalReferencesTitle = "Digital References List";
        public static string DigitalReferencesTemplateTitle = "Digital References Template";
        public static string[] DigitalReferencesColHeader = new string[] { "Title", "FileTypeandSize", "DateUploaded" };
        public static string[] DigitalReferencesSampleData = new string[] { "", "", "" };
    }
}
