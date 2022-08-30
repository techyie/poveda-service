using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class visitorVM
    {
       public int? Visitor_ID {get;set;}
       public string IDNumber {get;set;}
       public string FirstName {get;set;}
       public string MiddleName {get;set;}
       public string LastName {get;set;}
       public int? Campus_ID {get;set;}
       public string Campus_Name {get;set;}
       public string Email {get;set;}
       public string ContactNumber {get;set;}
       public string Address {get;set;}
    }

    public class personVisitorVM
    {
        public int? personId { get; set; }
        public string idNumber { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string birthdate { get; set; }
        public string gender { get; set; }
        public string contactNumber { get; set; }
        public string emailAddress { get; set; }
        public string address { get; set; }
        public string status { get; set; }
    }

    public class personVisitorTableVM
    {
        public int? personId { get; set; }
        public string idNumber { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string birthdate { get; set; }
        public string gender { get; set; }
        public string contactNumber { get; set; }
        public string emailAddress { get; set; }
        public string address { get; set; }
        public string status { get; set; }
    }

    public class visitorPagedResult
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

        public IList<personEntity> personvisitors { get; set; }
    }

    public class visitorPagedResultVM
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

        public virtual ICollection<personVisitorVM> personvisitors { get; set; }
    }
}
