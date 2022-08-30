using MyCampusV2.Models.V2.entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class studentVM
    {
        public int? personId { get; set; }
        public string idNumber { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string birthdate { get; set; }
        public string gender { get; set; }
        public string address { get; set; }
        public string contactNumber { get; set; }
        public string telephoneNumber { get; set; }
        public string emailAddress { get; set; }
        public string educLevelName { get; set; }
        public int educLevelId { get; set; }
        public int yearSecId { get; set; }
        public string yearSecName { get; set; }
        public int studSecId { get; set; }
        public string studSecName { get; set; }
        public int? courseId { get; set; }
        public string courseName { get; set; }
        public int? collegeId { get; set; }
        public string collegeName { get; set; }
        public string description { get; set; }
        public string campusName { get; set; }
        public int? campusId { get; set; }
        public string dateEnrolled { get; set; }
        public string status { get; set; }
        public string emergencyFullname { get; set; }
        public string emergencyContact { get; set; }
        public string emergencyRelationship { get; set; }
        public string emergencyAddress { get; set; }

        public string isDropOut { get; set; }
        public string dropOutCode { get; set; }
        public string isTransferred { get; set; }
        public string transferredSchoolName { get; set; }
        public string dropOutOtherRemark { get; set; }
        public string isTransferredIn { get; set; }
        public string transferredInSchoolName { get; set; }

        public string educStatus { get; set; }
        public string dropOutType { get; set; }
        public string dropOutDesc { get; set; }
        public string dropOutRemarks { get; set; }
        public string schoolName { get; set; }

        public bool isChangePhoto { get; set; }
        public bool isChangeSignature { get; set; }

        public string encodedstr { get; set; }
        public IFormFile filePhoto { get; set; }
        public IFormFile fileSignature { get; set; }
    }

    public class studentPagedResult
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

        public IList<personEntity> students { get; set; }
    }

    public class studentPagedResultVM
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

        public virtual ICollection<studentVM> students { get; set; }
    }
}
