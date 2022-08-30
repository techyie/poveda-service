using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class studentSectionVM
    {
        public int? studSecId { get; set; }
        public string description { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string halfDay { get; set; }
        public string gracePeriod { get; set; }
        public string password { get; set; }
        public int? yearSecId { get; set; }
        public string yearSecName { get; set; }
        public int? educLevelId { get; set; }
        public string educLevelName { get; set; }
        public int? campusId { get; set; }
        public string campusName { get; set; }

        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }

    }

    public class studentSecPagedResult
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

        public IList<studentSectionEntity> studentSections { get; set; }
    }


    public class studentSecPagedResultVM
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

        public virtual ICollection<studentSectionVM> studentSections { get; set; }
    }


    public class sectionScheduleVM
    {
        public int? schedId { get; set; }
        public int schedStudSecId { get; set; }
        public string schedDate { get; set; }
        public string schedStartDate { get; set; }
        public string schedEndDate { get; set; }
        public string schedStartTime { get; set; }
        public string schedEndTime { get; set; }
        public string schedHalfDay { get; set; }
        public string schedGracePeriod { get; set; }
        public bool schedIsExcused { get; set; }
        public string schedRemarks { get; set; }

        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }

    }

    public class sectionScheduleFilterVM
    {
        public string Keyword { get; set; }
        public long? SectionID { get; set; }
    }

    public class sectionSchedulePagedResult
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

        public IList<sectionScheduleEntity> schedules { get; set; }
    }

    public class sectionSchedulePagedResultVM
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

        public ICollection<sectionScheduleVM> schedules { get; set; }
    }
}
