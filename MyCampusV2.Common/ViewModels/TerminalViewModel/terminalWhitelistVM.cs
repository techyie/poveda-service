using MyCampusV2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Common.ViewModels
{
    public class terminalWhitelistVM
    { 
        //public long? Card_Details_ID { get; set; }
        //public string ID_Number { get; set; }
        //public string Full_Name { get; set; }
        //public string Card_Serial { get; set; }
        //public string Status { get; set; }
        //public int? Terminal_ID { get; set; }

        [Key]
        public int personId { get; set; }
        public string idNumber { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string cardSerial { get; set; }
        public int? status { get; set; }
        public int? whitelistId { get; set; }
        public int? datasyncId { get; set; }
        public int? cardStatus { get; set; }
    }

    public class terminalWhitelistFilterVM
    {
        public string Keyword { get; set; }
        public long? Terminal_ID { get; set; }
    }

    public class terminalWhitelistPagedResult
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

        //public IList<tbl_terminal_whitelist> whitelist { get; set; }
        public IList<terminalWhitelistVM> whitelist { get; set; }
    }

    public class terminalWhitelistPagedResultVM
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

        //public IList<terminalWhitelistVM> whitelist { get; set; }
        public IList<terminalWhitelistVM> whitelist { get; set; }
    }
}
