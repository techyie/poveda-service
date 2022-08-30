using MyCampusV2.Models.V2.entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class fetcherVM
    {
        public int? personId { get; set; }
        public string idNumber { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string birthdate { get; set; }
        public string gender { get; set; }
        public int? campusId { get; set; }
        public string campusName { get; set; }
        public string emailAddress { get; set; }
        public string contactNumber { get; set; }
        public string address { get; set; }
        public string fetcherRelationship { get; set; }

        public IFormFile filePhoto { get; set; }
        public IFormFile fileSignature { get; set; }

        public bool isChangePhoto { get; set; }
        public bool isChangeSignature { get; set; }

        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
		public bool isActive { get; set; }
        public bool toDisplay { get; set; }
    }

    public class personFetcherVM
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
        public string fetcherRelationship { get; set; }
        public int? campusId { get; set; }
        public string campusName { get; set; }
    }

    public class personFetcherTableVM
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
        public string fetcherRelationship { get; set; }
        public int? campusId { get; set; }
        public string campusName { get; set; }
    }

    public class fetcherPagedResult
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

        public IList<personEntity> personfetchers { get; set; }
    }

    public class fetcherPagedResultVM
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

        public virtual ICollection<personFetcherVM> personfetchers { get; set; }
    }
}
