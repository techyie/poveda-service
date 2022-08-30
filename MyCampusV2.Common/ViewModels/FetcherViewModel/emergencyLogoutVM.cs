using MyCampusV2.Models.V2.entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class emergencyLogoutVM
    {
        public int? emergencyLogoutId { get; set; }
        public string remarks { get; set; }
        public int? personId { get; set; }
        public string studentName { get; set; }
        public string idNumber { get; set; }
        public string effectivityDate { get; set; }
        public string isCancelled { get; set; }
        public int? userId { get; set; }
        public string status { get; set; }

        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
    }

    public class emergencyLogoutStudentsVM
    {
        public int? personId { get; set; }
        public string studentName { get; set; }
    }

    public class emergencyLogoutStudentsFilter
    {
        public int campusId { get; set; }
        public int educLevelId { get; set; }
        public int yearSecId { get; set; }
        public int studSecId { get; set; }
        public string studentRemarks { get; set; }
        public string studentEffectivityDate { get; set; }
    }

    public class emergencyLogoutPagedResult
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

        public IList<emergencyLogoutEntity> emergencyLogouts { get; set; }
    }

    public class emergencyLogoutPagedResultVM
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

        public virtual ICollection<emergencyLogoutVM> emergencyLogouts { get; set; }
    }
}
