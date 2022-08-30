using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System.ComponentModel.DataAnnotations;

namespace MyCampusV2.Common.ViewModels
{
    //public class VisitorVM
    //{
    //    public string Campus { get; set; }
    //    public string Area { get; set; }
    //    public string Terminal { get; set; }
    //    [JsonProperty(PropertyName = "ID Number")]
    //    public string ID_Number { get; set; }
    //    public string Name { get; set; }
    //    public string Date { get; set; }
    //    [JsonProperty(PropertyName = "Log Message")]
    //    public string Log_Message { get; set; }
    //}

    public class VisitorInformationVM
    {
        //public int id { get; set; }
        //public string trackingCode { get; set; }
        //public string visitorCard { get; set; }
        //public string scheduleDate { get; set; }
        //public string visitedPerson { get; set; }
        //public string visitPurpose { get; set; }
        //public string presentedIdNumber { get; set; }
        //public string presentedIdType { get; set; }
        //public string dateTimeTagged { get; set; }
        //public string dateTimeSurrendered { get; set; }
        //public bool manualOut { get; set; }
        //public string remarks { get; set; }
        //public string Email { get; set; }
        //public string Contact_Number { get; set; }
        //public string visitorName { get; set; }
        //public string Gender { get; set; }
        //public string NameOfEmployer { get; set; }

        [Key]
        public int ID { get; set; }
        public string ID_Number { get; set; }
        public string Visitor_Name { get; set; }
        public string Remarks { get; set; }
        public string Visited_Employee { get; set; }
        public string Scheduled_Date { get; set; }
        public string LogDateTime { get; set; }
        public string Area_Name { get; set; }
        public string LogIn { get; set; }
        public string LogOut { get; set; }
        public string Date_Time_Registered { get; set; }
        public string Date_Time_Surrendered { get; set; }
    }

    public class VisitorInformationResultVM
    {
        [Key]
        public int ID { get; set; }
        public string ID_Number { get; set; }
        public string Visitor_Name { get; set; }
        public string Remarks { get; set; }
        public string Visited_Employee { get; set; }
        public string Scheduled_Date { get; set; }
        public string LogDateTime { get; set; }
        public string Area_Name { get; set; }
        public string LogIn { get; set; }
        public string LogOut { get; set; }
        public string Date_Time_Registered { get; set; }
        public string Date_Time_Surrendered { get; set; }
    }

    public class visitorReportPagedResult
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }

        public int FirstRowOnPage
        {

            get { return (CurrentPage - 1) * PageSize + 1; }
        }

        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }

        public IList<VisitorInformationResultVM> visitorinformations { get; set; }
    }


    public class visitorReportPagedResultVM
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }

        public int FirstRowOnPage
        {

            get { return (CurrentPage - 1) * PageSize + 1; }
        }

        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }

        public virtual ICollection<VisitorInformationVM> visitorinformations { get; set; }
    }
}
