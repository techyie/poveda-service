using MyCampusV2.Models.V2.entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class fetcherScheduleVM
    {
        public int? fetcherSchedId { get; set; }
        public int? fetcherId { get; set; }
        public int? scheduleId { get; set; }
        public int? userId { get; set; }
        public string scheduleName { get; set; }
        public string scheduleDays { get; set; }
        public TimeSpan scheduleTimeFrom { get; set; }
        public TimeSpan scheduleTimeTo { get; set; }

        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
    }

    public class fetcherSchedulePagedResult
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

        public IList<fetcherScheduleEntity> fetcherSchedules { get; set; }
    }

    public class fetcherSchedulePagedResultVM
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

        public virtual ICollection<fetcherScheduleVM> fetcherSchedules { get; set; }
    }
}

