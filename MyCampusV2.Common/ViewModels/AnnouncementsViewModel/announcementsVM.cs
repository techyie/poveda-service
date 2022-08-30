using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class announcementsVM
    {
        public int id { get; set; }

        public string announcementsCode { get; set; }

        public string title { get; set; }

        public string body { get; set; }

        public string dateSent { get; set; }

        public bool isAll { get; set; }

        public bool isSent { get; set; }

        public string recipientsDisplay { get; set; }

        public ICollection<recipientsVM> recipients { get; set; }
        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
        public DateTime dateTimeAdded { get; set; }
        public DateTime lastUpdated { get; set; }
        public string dateAdded { get; set; }
    }

    public class recipientsVM
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class announcementsPagedResult
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

        public IList<announcementsEntity> announcements { get; set; }
    }

    public class announcementsRecipientsPagedResult
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

        public IList<announcementsRecipientsEntity> announcementsRecipients { get; set; }
    }

    public class announcementsPagedResultVM
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

        public virtual ICollection<announcementsVM> announcements { get; set; }
    }
}
