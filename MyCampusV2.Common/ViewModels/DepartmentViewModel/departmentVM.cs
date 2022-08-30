using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class departmentVM
    {
        //public long? Department_ID { get; set; }
        //public string Department_Name { get; set; }
        //public string Department_Desc { get; set; }
        //public long? Campus_ID { get; set; }
        //public string Campus_Name { get; set; }
        //public string Status { get; set; }

        public int? departmentId { get; set; }
        public string departmentName { get; set; }
        public int? campusId { get; set; }
        public string campusName { get; set; }

        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
    }

    public class departmentPagedResult
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

        public IList<departmentEntity> departments { get; set; }
    }


    public class departmentPagedResultVM
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

        public virtual ICollection<departmentVM> departments { get; set; }
    }
}
