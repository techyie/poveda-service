using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class AlarmVisitorVM
    {
        public string ID_Number { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Campus_Name { get; set; }
        public string Area_Name { get; set; }
        public string Terminal_Name { get; set; }
        public string Log_Date { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
    }

    public class AlarmVisitorPagedResult
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

        public IList<dailyLogsEntity> dailylogs { get; set; }
    }


    public class AlarmVisitorPagedResultVM
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

        public virtual ICollection<AlarmVisitorVM> dailylogs { get; set; }
    }
}
