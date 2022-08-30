using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class personalNotificationVM
    {
        public int? notification_ID { get; set; }
        public string message { get; set; }
        public string date_From { get; set; }
        public string date_To { get; set; }
        public int person_ID { get; set; }
        public string id_Number { get; set; }
        public string first_Name { get; set; }
        public string last_Name { get; set; }
        public int? terminal_ID { get; set; }
        public string terminal_Name { get; set; }
        public int? area_ID { get; set; }
        public string area_Name { get; set; }
        public int? campus_ID { get; set; }
        public string campus_Name { get; set; }
        public string status { get; set; }

        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
    }

    public class personalNotificationPagedResult
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

        public IList<notificationEntity> notifications { get; set; }
    }


    public class personalNotificationPagedResultVM
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

        public virtual ICollection<personalNotificationVM> notifications { get; set; }
    }
}
