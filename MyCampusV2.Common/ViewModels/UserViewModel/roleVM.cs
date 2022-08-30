using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    //public class roleVM
    //{
    //    public long? Role_ID { get; set; }
    //    public string Role_Name { get; set; }
    //    public string Role_Desc { get; set; }
    //    public string Status { get; set; }
    //}
    public class roleVM
    {
        public long? role_ID { get; set; }
        public string role_Name { get; set; }
        public string status { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
    }

    public class rolePagedResult
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

        public IList<roleEntity> roles { get; set; }
    }

    public class rolePagedResultVM
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

        public virtual ICollection<roleVM> roles { get; set; }
    }
}
