using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MyCampusV2.Common.ViewModels
{
    public class CardFetcherVM
    {
        [Key]
        public int ID { get; set; }
        public string ID_Number { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Date_Issued { get; set; }
        public string Date_Updated { get; set; }
    }

    public class CardFetcherResultVM
    {
        [Key]
        public int ID { get; set; }
        public string ID_Number { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Date_Issued { get; set; }
        public string Date_Updated { get; set; }
    }

    public class cardFetcherPagedResult
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

        public IList<CardFetcherResultVM> carddetails { get; set; }
    }


    public class cardFetcherPagedResultVM
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

        public virtual ICollection<CardFetcherVM> carddetails { get; set; }
    }
}