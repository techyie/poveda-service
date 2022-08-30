using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class courseVM
    {
        public int? courseId { get; set; }
        public string courseName { get; set; }
        public int? collegeId { get; set; }
        public string collegeName { get; set; }
        public int? educLevelId { get; set; }
        public string educLevelName { get; set; }
        public int? campusId { get; set; }
        public string campusName { get; set; }

        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }

        public string lastUpdated { get; set; }
    }

    public class coursePagedResult
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

        public IList<courseEntity> courses { get; set; }
    }


    public class coursePagedResultVM
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

        public virtual ICollection<courseVM> courses { get; set; }
    }
}
