using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MyCampusV2.Common.ViewModels
{
    public class CardStudentVM
    {
        public string ID_Number { get; set; }
        public string Name { get; set; }
        public string Educ_Level { get; set; }
        public string Year_Level { get; set; }
        public string Section { get; set; }
        public string Status { get; set; }
        public string Date_Issued { get; set; }
        public string Date_Updated { get; set; }
    }

    public class CardStudentResultVM
    {
        [Key]
        public int ID { get; set; }
        public string ID_Number { get; set; }
        public string Name { get; set; }
        public string Educ_Level { get; set; }
        public string Year_Level { get; set; }
        public string Section { get; set; }
        public string Status { get; set; }
        public string Date_Issued { get; set; }
        public string Date_Updated { get; set; }
    }

    public class cardStudentPagedResult
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

        public IList<CardStudentResultVM> carddetails { get; set; }
    }


    public class cardStudentPagedResultVM
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

        public virtual ICollection<CardStudentVM> carddetails { get; set; }
    }
}

/*
public class CardStudentVM
{
    [JsonProperty(PropertyName = "ID Number")]
    public string ID_Number {get;set;}
    public string Name {get;set;}
    [JsonProperty(PropertyName = "Educ Level")]
    public string EducLevel {get;set;}
    [JsonProperty(PropertyName = "Year Section")]
    public string YearSection {get;set;}
    public string Status {get;set;}
    [JsonProperty(PropertyName = "Issued Date")]
    public string Issued_Date {get;set;}
    [JsonProperty(PropertyName = "Last Updated")]
    public string Last_Updated {get;set;}
}

public class cardStudentReportVM
{
    public string IDNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CardNumber { get; set; }
    public string IssuedDate { get; set; }
    public string IssuedDate2 { get; set; }
    public string CardStatus { get; set; }
    public string IsBlocked { get; set; }
    public string IsOnHold { get; set; }
    public string ExpiryDate { get; set; }
    public string Remarks { get; set; }
} 
*/
