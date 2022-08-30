using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class officeVM
    {
        public int? officeId { get; set; }
        public string officeName { get; set; }
        public string officeStatus { get; set; }
        public int? campusId { get; set; }
        public string campusName { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
        public Boolean isActive { get; set; }
        public Boolean toDisplay { get; set; }
    }

    public class officePagedResult
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

        public IList<officeEntity> offices { get; set; }
    }


    public class officePagedResultVM
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

        public virtual ICollection<officeVM> offices { get; set; }
    }
}
