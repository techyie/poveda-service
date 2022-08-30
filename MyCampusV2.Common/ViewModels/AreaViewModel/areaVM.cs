﻿using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class areaVM
    {
        //public int? Area_ID { get; set; }
        //public string Area_Name { get; set; }
        //public string Area_Desc { get; set; }
        //public int? Campus_ID { get; set; }
        //public string Campus_Name { get; set; }
        //public string Status { get; set; }

        public int? areaId { get; set; }
        public string areaName { get; set; }
        public string areaDescription { get; set; }
        public int? campusId { get; set; }
        public string campusName { get; set; }

        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
    }

    public class areaPagedResult
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

        public IList<areaEntity> areas { get; set; }
    }


    public class areaPagedResultVM
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

        public virtual ICollection<areaVM> areas { get; set; }
    }
}
