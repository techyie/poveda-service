using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class fetcherGroupDetailsVM
    {
        public int? groupDetailId { get; set; }
        public int? fetcherGroupID { get; set; }
        public string groupName { get; set; }
        public int? personId { get; set; }
        public string idNumber { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string dateTimeAdded { get; set; }
        public string lastUpdated { get; set; }
        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
    }

    public class fetcherStudentsVM
    {
        public int? personId { get; set; }
        public string fetcherStudents { get; set; }
    }

    public class fetcherGroupDetailsPagedResult
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

        public IList<fetcherGroupDetailsEntity> groupsDetails { get; set; }
    }

    public class fetcherGroupDetailsPagedResultVM
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

        public virtual ICollection<fetcherGroupDetailsVM> groupsDetails { get; set; }
    }
}
