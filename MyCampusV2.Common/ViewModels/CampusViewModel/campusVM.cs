using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Common.ViewModels
{
    public class campusVM
    {
        //public int? Campus_ID { get; set; }
        //public string Campus_Name { get; set; }
        //public string Campus_Status { get; set; }
        //public string Campus_Code { get; set; }
        //public string Campus_Address { get; set; }
        //public string Campus_ContactNo { get; set; }
        //public string Region_Name { get; set; }
        //public string Division_Name { get; set; }
        //public int Region_ID { get; set; }
        //public int Division_ID { get; set; }
        //public string Status { get; set; }

        public int? campusId { get; set; }
        public string campusName { get; set; }
        public string campusStatus { get; set; }
        public string campusCode { get; set; }
        public string campusAddress { get; set; }
        public string campusContactNo { get; set; }
        public int? divisionId { get; set; }
        public string divisionName { get; set; }
        public int? regionId { get; set; }
        public string regionName { get; set; }

        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
    }

    public class campusPagedResult
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

        public IList<campusEntity> campuses { get; set; }
    }


    public class campusPagedResultVM
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

        public virtual ICollection<campusVM> campuses { get; set; }
    }

}
