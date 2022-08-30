using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class eventLoggingVM
    {
        public int? eventLogId { get; set; }
        public int? userId { get; set; }
        public string userName { get; set; }
        public int? formId { get; set; }
        public string formName { get; set; }
        public string source { get; set; }
        public string category { get; set; }
        public Boolean logLevel { get; set; }
        public string message { get; set; }
        public DateTime Log_Date_Time { get; set; }
    }

    public class eventLoggingPagedResult
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

        public IList<eventLoggingEntity> eventLoggingList { get; set; }
    }

    public class eventLoggingPagedResultVM
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

        public virtual ICollection<eventLoggingVM> eventLoggingList { get; set; }
    }
}
