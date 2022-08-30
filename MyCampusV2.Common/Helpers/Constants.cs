using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.Helpers
{
    public static class Constants
    {
        public static string SuccessMessageAdd = " has been successfully added!";
        public static string SuccessMessageUpdate = " has been successfully updated!";
        public static string SuccessMessageDelete = " has been successfully deleted!";
        public static string SuccessMessagePermanentDelete = " has been successfully deleted permanently!";
        public static string SuccessMessageTemporaryDelete = " has been successfully deactivated!";
        public static string SuccessMessageRetrieve = " has been successfully activated!";
        public static string FailedMessageUpdate = "Failed to update ";
        public static string FailedMessageCreate = "Failed to add ";
        public static string FailedMessageDelete = "Failed to delete ";
        public static string SuccessEmailSend = "Email sent!";
        public static string SuccessPasswordResetEmailSend = "A password reset link has been sent to";
        public static string FailedEmailSend = "Failed to send!";
        public static string FailedActivation = "Failed to activate account!";
        public static string SuccessActivation = "You have successfully updated your password.\nThank you.!";
        public static string SuccessDigitalFileUpload = "The file has been uploaded and will be visible in the app.";
        public static string FailedDigitalFileUpload = "Failed to upload.";
        public static string SuccessArchivedDigitalFileUpload = "The file has been removed from the list and app.";
        public static string FailedArchivedDigitalFileUpload = "Failed to archive.";
        public static string SuccessUpdateDigitalFileUpload = "The file has been successfully updated.";
        public static string FailedUpdateDigitalFileUpload = "Failed to update.";
        public static string SuccessArchivingAnnouncement = "The message has been removed from the list and inbox of its recipients.";
        public static string FailedArchivingAnnouncement = "Failed to archive.";
        public static string SuccessAnnouncementAdd = "The message has been posted and is now on queue for sending.";
        public static string SuccessArchivingCalendar = "The post has been removed in the app.";
        public static string SuccessCalendarAdd = "This post has been added on the app’s calendar module.";
        public static string FailedCalendarAdd = "Failed to post.";
        public static string SuccessUpdateAccount = "The account’s information has been successfully updated.";
        //public static string SuccessMessageUpdateAccount = "The account’s information has been successfully updated.";
        //public static string SuccessMessageTemporaryDelete = " has been successfully deleted temporarily!";
        //public static string SuccessMessageRetrieve = " has been successfully recovered!";

        public const string REQUIRED_HEADER_AREA_NAME = "AreaName header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_AREA_DESC = "AreaDescription header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_CAMPUS_CODE = "CampusCode header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_CAMPUS_NAME = "CampusName header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_EDUCATIONAL_LEVEL_NAME = "EducationalLevelName header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_EDUCATIONAL_LEVEL_DESC = "EducationalLevelDescription header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_EDUCATIONAL_LEVEL_IS_COLLEGE = "isCollege header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_COLLEGE_NAME = "CollegeName header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_COLLEGE_DESC = "CollegeDescription header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_COURSE_NAME = "CourseName header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_COURSE_DESC = "CourseDescription header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_COURSE_CODE = "CourseCode header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_YEAR_LEVEL_NAME = "YearLevelName header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_YEAR_LEVEL_DESC = "YearLevelDescription header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_SECTION_NAME = "SectionName header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_SECTION_DESC = "SectionDescription header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_DEPARTMENT_NAME = "DepartmentName header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_DEPARTMENT_DESCRIPTION = "DepartmentDescription header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_POSITION_NAME = "PositionName header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_POSITION_DESC = "PositionDescription header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_ID_NUMBER = "IDNumber header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_FIRST_NAME = "FirstName header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_MIDDLE_NAME = "MiddleName header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_LAST_NAME = "LastName header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_BIRTHDATE = "Birthdate header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_GENDER = "Gender header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_ADDRESS = "Address header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_CONTACT_NUMBER = "ContactNumber header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_EMAIL_ADDRESS = "EmailAddress header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_MESSAGE = "Message header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_DATE_FROM = "DateFrom header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_DATE_TO = "DateTo header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_TERMINAL_NAME = "TerminalName header is missing from the uploaded file.";

        public const string REQUIRED_HEADER_EMERGENCY_FULLNAME = "EmergencyFullname header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_EMERGENCY_CONTACT = "EmergencyContact header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_EMERGENCY_RELATIONSHIP = "EmergencyRelationship header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_EMERGENCY_ADDRESS = "EmergencyAddress header is missing from the uploaded file.";

        public const string REQUIRED_HEADER_SSS = "SSS header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_PAGIBIG = "PAGIBIG header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_TIN = "TIN header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_PHILHEALTH = "PhilHealth header is missing from the uploaded file.";

        public const string REQUIRED_HEADER_EXPIRATION = "Expiry_Date header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_ONHOLD = "On_Hold header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_BLOCKED = "Blocked header is missing from the uploaded file.";
        public const string REQUIRED_HEADER_REMARKS = "Remarks  is missing from the uploaded file.";

        public const string REQUIRED_HEADER_REGION = "Region is missing from the uploaded file.";
        public const string REQUIRED_HEADER_DIVISION = "Division is missing from the uploaded file.";

        public const string BATCH_UPLOAD_STATUS_UPLOAD = "UPLOAD";
        public const string BATCH_UPLOAD_STATUS_ONPROCESS = "ON PROCESS";
        public const string BATCH_UPLOAD_STATUS_SUCCESS = "SUCCESS";
        public const string BATCH_UPLOAD_STATUS_FAILED = "FAILED";
        public const string BATCH_UPLOAD_STATUS_CANCELLED = "CANCELLED";

        public const string LOGIN_INVALID_EMAIL = "Invalid Email Account.";
        public const string LOGIN_INVALID_AD = "Invalid Active Directory Account.";
        public const string LOGIN_INVALID_ACCOUNT = "Invalid Account.";
        public const string LOGIN_INCORRECT_INPUT = "Username or Password is incorrect.";
        public const string LOGIN_BAD_REQUEST = "Bad Request Error";
    }
}
