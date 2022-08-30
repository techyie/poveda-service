using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Models;
using Newtonsoft.Json;

namespace MyCampusV2.Common.ViewModels.ReportViewModel
{
    public class TotalHoursOfEmployeeVM
    {
        [Key]
        public string ID_Number { get; set; }
        public string Full_Name { get; set; }
        public string Department_Name { get; set; }
        public string Employee_Type { get; set; }
        public string Total_Hours { get; set; }
    }

    public class totalHoursOfEmployeePagedResult
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

        //public IList<tbl_daily_logs> dailylogs { get; set; }
        public IList<TotalHoursOfEmployeeVM> dailylogs { get; set; }
    }

    public class totalHoursOfEmployeePagedResultVM
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

        public virtual ICollection<TotalHoursOfEmployeeVM> dailylogs { get; set; }
    }
}
