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
    public class CardEmployeeVM
    {
        [Key]
        public int ID { get; set; }
        public string ID_Number { get; set; }
        public string Name { get; set; }
        public string Department_Name { get; set; }
        public string Position_Name { get; set; }
        public string Status { get; set; }
        public string Date_Issued { get; set; }
        public string Date_Updated { get; set; }
    }

    public class CardEmployeeResultVM
    {
        [Key]
        public int ID { get; set; }
        public string ID_Number { get; set; }
        public string Name { get; set; }
        public string Department_Name { get; set; }
        public string Position_Name { get; set; }
        public string Status { get; set; }
        public string Date_Issued { get; set; }
        public string Date_Updated { get; set; }
    }

    public class cardEmployeePagedResult
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

        public IList<CardEmployeeResultVM> carddetails { get; set; }
    }


    public class cardEmployeePagedResultVM
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

        public virtual ICollection<CardEmployeeVM> carddetails { get; set; }
    }
}

/*
public class CardEmployeeVM
{
    [JsonProperty(PropertyName = "ID Number")]
    public string ID_Number {get;set;}
    public string Name {get;set;}
    public string Department {get;set;}
    public string Position {get;set;}
    public string Status {get;set;}
    [JsonProperty(PropertyName = "Issued Date")]
    public string Issued_Date {get;set;}
    [JsonProperty(PropertyName = "Last Updated")]
    public string Last_Updated {get;set;}
} 
*/
