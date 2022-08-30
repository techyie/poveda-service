using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using Newtonsoft.Json;

namespace MyCampusV2.Common.ViewModels
{
    public class TimeAndAttendanceVM
    {
       [JsonProperty(PropertyName = "ID Number")]
       public string ID_Number { get; set; }
       public string Name { get; set; }
       public string Campus { get; set;}
       public string Educational_Level { get; set;}
       public string Year { get; set;}
       public string Section { get; set;}
       public string Date { get; set;}
       public string In { get; set; }
       public string Out { get; set; }
    }

    //public class TimeAndAttendanceStudentVM
    //{
    //    public string ID_Number { get; set; }
    //    public string First_Name { get; set; }
    //    public string Last_Name { get; set; }
    //    public string Campus_Name { get; set; }
    //    public string Area_Name { get; set; }
    //    public string Terminal_Name { get; set; }
    //    public string Log_Date { get; set; }
    //    public string Log_Date2 { get; set; }
    //    public string Status { get; set; }
    //    public string Name { get; set; }
    //}

    //public class timeAndAttendanceStudentPagedResult
    //{
    //    public int CurrentPage { get; set; }
    //    public int PageCount { get; set; }
    //    public int PageSize { get; set; }
    //    public int RowCount { get; set; }

    //    public int FirstRowOnPage
    //    {

    //        get { return (CurrentPage - 1) * PageSize + 1; }
    //    }

    //    public int LastRowOnPage
    //    {
    //        get { return Math.Min(CurrentPage * PageSize, RowCount); }
    //    }

    //    public IList<dailyLogsEntity> dailylogs { get; set; }
    //}


    //public class timeAndAttendanceStudentPagedResultVM
    //{
    //    public int CurrentPage { get; set; }
    //    public int PageCount { get; set; }
    //    public int PageSize { get; set; }
    //    public int RowCount { get; set; }

    //    public int FirstRowOnPage
    //    {

    //        get { return (CurrentPage - 1) * PageSize + 1; }
    //    }

    //    public int LastRowOnPage
    //    {
    //        get { return Math.Min(CurrentPage * PageSize, RowCount); }
    //    }

    //    public virtual ICollection<TimeAndAttendanceStudentVM> dailylogs { get; set; }
    //}
}
