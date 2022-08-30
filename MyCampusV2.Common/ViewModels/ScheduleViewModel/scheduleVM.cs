using MyCampusV2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class scheduleVM
    {
        public int? scheduleId { get; set; }
        public string scheduleName { get; set; }
        public string scheduleDays { get; set; }
        public TimeSpan scheduleTimeFrom { get; set; }
        public TimeSpan scheduleTimeTo { get; set; }
        public string scheduleStatus { get; set; }
        public string dateTimeAdded { get; set; }
        public string lastUpdated { get; set; }
        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
    }

    public class schedulePagedResult
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

        public IList<scheduleEntity> schedules { get; set; }
    }

    public class schedulePagedResultVM
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

        public virtual ICollection<scheduleVM> schedules { get; set; }
    }
}
