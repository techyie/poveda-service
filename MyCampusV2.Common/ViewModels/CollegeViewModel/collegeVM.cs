using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class collegeVM
    {
        //public long? College_ID { get; set; }
        //public string College_Name { get; set; }
        //public string College_Desc { get; set; }
        //public long Educ_Level_ID { get; set; }
        //public string Educ_Level_Name { get; set; }
        //public long Campus_ID { get; set; }
        //public string Campus_Name { get; set; }
        //public string Status { get; set; }

        public int? collegeId { get; set; }
        public string collegeName { get; set; }
        public string collegeStatus { get; set; }
        public int? educLevelId { get; set; }
        public string educLevelName { get; set; }
        public int? campusId { get; set; }
        public string campusName { get; set; }

        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
    }

    public class collegePagedResult
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

        public IList<collegeEntity> colleges { get; set; }
    }


    public class collegePagedResultVM
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

        public virtual ICollection<collegeVM> colleges { get; set; }
    }
}
