using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class papAccountVM
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string emailAddress { get; set; }
        public string mobileNumber { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string accountCode { get; set; }
        public string fullName { get; set; }
        public ICollection<students> linkedStudents { get; set; }
        public string lnkStudents { get; set; }
        public bool isPending { get; set; }
        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
        public DateTime dateTimeAdded { get; set; }
        public DateTime lastUpdated { get; set; }
        public string dateAdded { get; set; }
    }

    public class papAccountLinkedStudentsVM
    {
        public string idNumber { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string yearLevelName { get; set; }
        public string sectionName { get; set; }
        public string imageByte { get; set; }
    }

    public class studentsVM
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class papAccountPagedResult
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

        public IList<papAccountEntity> papAccounts { get; set; }
    }

    public class papAccountPagedResultVM
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

        public virtual ICollection<papAccountVM> papAccounts { get; set; }
    }
}

