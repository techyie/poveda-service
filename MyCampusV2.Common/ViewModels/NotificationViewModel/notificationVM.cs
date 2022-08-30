using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class notificationVM
    {
        public int? notificationId { get; set; }
        public int? personId { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string idNumber { get; set; }
        public string notificationMessage { get; set; }
        public string dateToDisplayFrom { get; set; }
        public string dateToDisplayTo { get; set; }
        public string guid { get; set; }
        public int campusId { get; set; }
        public string campusName { get; set; }
        public int areaId { get; set; }
        public string areaName { get; set; }
        public int terminalId { get; set; }
        public string terminalName { get; set; }
        public bool isDeleted { get; set; }
        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
    }

    public class notificationPagedResult
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


    public class notificationPagedResultVM
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

        public virtual ICollection<notificationVM> notifications { get; set; }
    }
}
