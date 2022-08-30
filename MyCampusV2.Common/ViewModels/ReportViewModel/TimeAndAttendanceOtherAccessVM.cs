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
    public class TimeAndAttendanceOtherAccessVM
    {
        [Key]
        public int ID { get; set; }
        public string ID_Number { get; set; }
        public string Name { get; set; }
        public string Office_Name { get; set; }
        public string Date { get; set; }
        public string Log_In { get; set; }
        public string Log_Out { get; set; }
    }

    public class TimeAndAttendanceOtherAccessResultVM
    {
        [Key]
        public int ID { get; set; }
        public string ID_Number { get; set; }
        public string Name { get; set; }
        public string Office_Name { get; set; }
        public string Date { get; set; }
        public string Log_In { get; set; }
        public string Log_Out { get; set; }
    }

    public class timeAndAttendanceOtherAccessPagedResult
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

        public IList<TimeAndAttendanceOtherAccessResultVM> dailylogs { get; set; }
    }


    public class timeAndAttendanceOtherAccessPagedResultVM
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

        public virtual ICollection<TimeAndAttendanceOtherAccessVM> dailylogs { get; set; }
    }
}
