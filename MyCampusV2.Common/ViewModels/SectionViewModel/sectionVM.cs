using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class sectionVM
    {
        public long? Section_ID { get; set; }

        public string Section_Name { get; set; }

        public string Section_Desc { get; set; }

        public long Year_Level_ID { get; set; }

        public string Year_Level_Name { get; set; }

        public long Campus_ID { get; set; }

        public string Campus_Name { get; set; }

        public long Educ_Level_ID { get; set; }

        public string Educ_Level_Name { get; set; }

        public string Status { get; set; }
    }

    public class studentSecPagedResult
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

        public IList<sectionEntity> sections { get; set; }
    }


    public class sectionPagedResultVM
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

        public virtual ICollection<sectionVM> sections { get; set; }
    }
}
