using MyCampusV2.Models.V2.entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class otherAccessVM
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
        public int? officeId { get; set; }
        public string officeName { get; set; }
        public int? departmentId { get; set; }
        public string departmentName { get; set; }
        public int? positionId { get; set; }
        public string positionName { get; set; }
        public string emailAddress { get; set; }
        public string contactNumber { get; set; }
        public string telephoneNumber { get; set; }
        public string address { get; set; }

        public string emergencyFullname { get; set; }
        public string emergencyContact { get; set; }
        public string emergencyRelationship { get; set; }
        public string emergencyAddress { get; set; }

        public bool isChangePhoto { get; set; }
        public bool isChangeSignature { get; set; }

        public IFormFile filePhoto { get; set; }
        public IFormFile fileSignature { get; set; }

        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
    }

    public class personOtherAccessVM
    {
        public int? personId { get; set; }
        public string idNumber { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string birthdate { get; set; }
        public string gender { get; set; }
        public string contactNumber { get; set; }
        public string telephoneNumber { get; set; }
        public string emailAddress { get; set; }
        public string address { get; set; }
        public string status { get; set; }

        public string emergencyFullname { get; set; }
        public string emergencyContact { get; set; }
        public string emergencyRelationship { get; set; }
        public string emergencyAddress { get; set; }

        public int? campusId { get; set; }
        public string campusName { get; set; }
        public int? officeId { get; set; }
        public string officeName { get; set; }
        public int? departmentId { get; set; }
        public string departmentName { get; set; }
        public int? positionId { get; set; }
        public string positionName { get; set; }
    }

    public class personOtherAccessTableVM
    {
        public int? personId { get; set; }
        public string idNumber { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string birthdate { get; set; }
        public string gender { get; set; }
        public string contactNumber { get; set; }
        public string telephoneNumber { get; set; }
        public string emailAddress { get; set; }
        public string address { get; set; }
        public string status { get; set; }

        public int? campusId { get; set; }
        public string campusName { get; set; }
        public int? officeId { get; set; }
        public string officeName { get; set; }
        public int? departmentId { get; set; }
        public string departmentName { get; set; }
        public int? positionId { get; set; }
        public string positionName { get; set; }

        public string emergencyFullname { get; set; }
        public string emergencyContact { get; set; }
        public string emergencyRelationship { get; set; }
        public string emergencyAddress { get; set; }
    }

    public class otherAccessPagedResult
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

        public IList<personEntity> personotheraccess { get; set; }
    }

    public class otherAccessPagedResultVM
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

        public virtual ICollection<personOtherAccessVM> personotheraccess { get; set; }
    }
}
