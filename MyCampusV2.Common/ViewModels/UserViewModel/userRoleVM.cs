using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class userRoleVM
    {
        //public int? user_role_ID { get; set; }
        public int user_ID { get; set; }
        public int role_ID { get; set; }
        public string role_Name { get; set; }
        public string user_Name { get; set; }
        public string id_Number { get; set; }
        public string full_Name { get; set; }
        public string position_Name { get; set; }
        public string department_Name { get; set; }
        public string campus_Name { get; set; }
    }

    public class userPagedResult
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

        public IList<userEntity> users { get; set; }
    }

    public class userPagedResultVM
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

        public virtual ICollection<userRoleVM> users { get; set; }
    }
}
