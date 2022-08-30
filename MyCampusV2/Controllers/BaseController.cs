using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.IServices;
using Microsoft.Extensions.DependencyInjection;
using MyCampusV2.Common.ViewModels;

namespace MyCampusV2.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected enum Form
        {
            People = 4,
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
            Notification = 34,
            Notification_General = 35,
            Notification_Personal = 36,
            Batch_Upload = 37,
            User_List = 37,
            Card_Assign = 38,
            Card_Update = 39,
            Card_Deactivate = 40,
            Card_Reassign = 41,
            Campus_Card = 51,
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

        protected const string SuccessMessageAdd = " has been successfully added!";
        protected const string SuccessMessageUpdate = " has been successfully updated!";
        protected const string SuccessMessageDelete = " has been successfully deleted!";
        protected const string SuccessMessagePermanentDelete = " has been successfully deleted permanently!";
        protected const string SuccessMessageTemporaryDelete = " has been successfully deactivated!";
        protected const string SuccessMessageRetrieve = " has been successfully activated!";
        //protected const string SuccessMessageTemporaryDelete = " has been successfully deleted temporarily!";
        //protected const string SuccessMessageRetrieve = " has been successfully retrieved!";

        protected int GetUserId()
        {
            var currentUser = HttpContext.User;
            int userID = 0;

            if (currentUser.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                userID = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            }
            return userID;
        }

        protected int GetCampusId()
        {
            var currentUser = HttpContext.User;
            int campus = 0;

            if (currentUser.HasClaim(c => c.Type == ClaimTypes.PrimarySid))
            {
                campus = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid).Value);
            }
            return campus;
        }

        protected int GetRoleId()
        {
            var currentUser = HttpContext.User;
            int roleID = 0;

            if (currentUser.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                roleID = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value);
            }
            return roleID;
        }

        protected ResultModel CreateResult(string Code, string Message, bool isSuccess)
        {
            ResultModel result = new ResultModel();
            result.resultCode = Code;
            result.resultMessage = Message;
            result.isSuccess = isSuccess;
            return result;
        }
    }
}