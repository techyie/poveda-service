using MyCampusV2.Models.V2.entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class fetcherScheduleDetailsVM
    {
        public int? fetcherSchedDtlId { get; set; }
        public int? fetcherSchedId { get; set; }
        public int? fetcherGroupId { get; set; }
        public int? personId { get; set; }
        public int? userId { get; set; }
        public string studentName { get; set; }
        public string groupName { get; set; }
        public string scheduleName { get; set; }
        public string scheduleDays { get; set; }
        public TimeSpan scheduleTimeFrom { get; set; }
        public TimeSpan scheduleTimeTo { get; set; }

        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
    }

    public class fetcherScheduleDetailsPagedResult
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

        public IList<fetcherScheduleDetailsEntity> fetcherStudents { get; set; }
    }

    public class fetcherScheduleDetailsPagedResultVM
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

        public virtual ICollection<fetcherScheduleDetailsVM> fetcherStudents { get; set; }
    }
}

