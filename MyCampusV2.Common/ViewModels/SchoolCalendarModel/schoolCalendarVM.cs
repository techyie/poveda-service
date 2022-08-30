using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class schoolCalendarVM
    {
        public string date { get; set; }
        public string className { get; set; }
    }
    public class schoolCalendarDatesVM
    {
        public DateTime date { get; set; }
        public int dateValue { get; set; }
    }

    public class schoolCalendarResult
    {
        public ICollection<schoolCalendarVM> schoolcalendar { get; set; }
    }

    //public class studentSecPagedResult
    //{
    //    public int CurrentPage { get; set; }
    //    public int PageCount { get; set; }
    //    public int PageSize { get; set; }
    //    public int RowCount { get; set; }

    //    public int FirstRowOnPage
    //    {

    //        get { return (CurrentPage - 1) * PageSize + 1; }
    //    }

    //    public int LastRowOnPage
    //    {
    //        get { return Math.Min(CurrentPage * PageSize, RowCount); }
    //    }

    //    public IList<studentSectionEntity> studentSections { get; set; }
    //}


    //public class studentSecPagedResultVM
    //{
    //    public int CurrentPage { get; set; }
    //    public int PageCount { get; set; }
    //    public int PageSize { get; set; }
    //    public int RowCount { get; set; }

    //    public int FirstRowOnPage
    //    {

    //        get { return (CurrentPage - 1) * PageSize + 1; }
    //    }

    //    public int LastRowOnPage
    //    {
    //        get { return Math.Min(CurrentPage * PageSize, RowCount); }
    //    }

    //    public virtual ICollection<studentSectionVM> studentSections { get; set; }
    //}


    public class schoolYearVM
    {
        public int? schoolYearId { get; set; }
        public string schoolYear { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }

        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }

    }

    public class schoolYearFilterVM
    {
        public string Keyword { get; set; }
    }

    public class schoolYearPagedResult
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

        public IList<schoolYearEntity> schoolyear { get; set; }
    }

    public class schoolYearPagedResultVM
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

        public ICollection<schoolYearVM> schoolyear { get; set; }
    }
}
