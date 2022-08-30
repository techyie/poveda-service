using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class cardReportFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string cardstatus { get; set; }
        public string keyword { get; set; }
        public string persontype { get; set; }
    }

    public class CardReportVM
    {
        public string IDNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CardNumber { get; set; }
        public string IssuedDate { get; set; }
        public string IssuedDate2 { get; set; }
        public string CardStatus { get; set; }
        public string IsBlocked { get; set; }
        public string IsOnHold { get; set; }
        public string ExpiryDate { get; set; }
        public string Remarks { get; set; }
        public string Name { get; set; }
    }

    public class cardReportPagedResult
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

        public IList<cardDetailsEntity> carddetails { get; set; }
    }


    public class cardReportPagedResultVM
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

        public virtual ICollection<CardReportVM> carddetails { get; set; }
    }


}
