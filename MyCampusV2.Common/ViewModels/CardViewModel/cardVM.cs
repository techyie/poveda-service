using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;

namespace MyCampusV2.Common.ViewModels
{
    public class cardVM
    {
        //public int? Card_ID { get; set; }
        //public string UID { get; set; }
        //public string Card_Serial { get; set; }
        //public DateTime Issued_Date { get; set; }
        //public DateTime Expiry_Date { get; set; }
        //public long Person_ID { get; set; }
        //public bool On_Hold { get; set; }
        //public bool Blocked { get; set; }
        //public string ID_Number { get; set; }
        //public string remarks { get; set; }

        public int? cardId { get; set; }
        public string cardSerial { get; set; }
        public int? personId { get; set; }
        public string cardPersonType { get; set; }

        public DateTime issuedDate { get; set; }
        public DateTime expiryDate { get; set; }
        public string pan { get; set; }
        public string remarks { get; set; }
        public string uid { get; set; }
        public bool isActive { get; set; }
        public bool? isSeparated { get; set; }
        public DateTime? separatedDate { get; set; }
    }

    public class cardPagedResult
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

        public IList<cardDetailsEntity> cards { get; set; }
    }

    public class cardPagedResultVM
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

        public virtual ICollection<cardVM> cards { get; set; }
    }
}
