using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class educlevelVM
    {
        //public long? Educ_Level_ID { get; set; }
        //public string Educ_Level_Name { get; set; }
        //public string Educ_Level_Desc { get; set; }
        //public bool? IsCollege { get; set; }
        //public long? Campus_ID { get; set; }
        //public string Campus_Name { get; set; }
        //public string Status { get; set; }

        public long? educLevelId { get; set; }
        public string educLevelName { get; set; }
        public bool hasCourse { get; set; }
        public long? campusId { get; set; }
        public string campusName { get; set; }
        public string educLevelStatus { get; set; }

        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
    }

    public class educlevelPagedResult
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

        public IList<educationalLevelEntity> educlevels { get; set; }
    }


    public class educlevelPagedResultVM
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

        public virtual ICollection<educlevelVM> educlevels { get; set; }
    }
}
