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
    public class TimeAndAttendanceAllVM
    {
        [Key]
        public int ID { get; set; }
        public string ID_Number { get; set; }
        public string Name { get; set; }
        public string Person_Type { get; set; }
        public string Date { get; set; }
        public string Log_In { get; set; }
        public string Log_Out { get; set; }
    }

    public class TimeAndAttendanceAllResultVM
    {
        [Key]
        public int ID { get; set; }
        public string ID_Number { get; set; }
        public string Name { get; set; }
        public string Person_Type { get; set; }
        public string Date { get; set; }
        public string Log_In { get; set; }
        public string Log_Out { get; set; }
    }

    public class timeAndAttendanceAllPagedResult
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

        public IList<TimeAndAttendanceAllResultVM> dailylogs { get; set; }
    }


    public class timeAndAttendanceAllPagedResultVM
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

        public virtual ICollection<TimeAndAttendanceAllVM> dailylogs { get; set; }
    }
}
