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
    public class detailsPerPersonVM
    {
        [Key]
        public int ID { get; set; }
        public string idNumber { get; set; }
        public string fullName { get; set; }
        public string logDate { get; set; }
        public string logMessage { get; set; }
        public string terminalName { get; set; }
    }

    public class DetailsPerPersonResultVM
    {
        [Key]
        public int ID { get; set; }
        public string idNumber { get; set; }
        public string fullName { get; set; }
        public string logDate { get; set; }
        public string logMessage { get; set; }
        public string terminalName { get; set; }
    }

    public class detailsPerPersonPagedResult
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

        public IList<DetailsPerPersonResultVM> detailsPerPerson { get; set; }
    }

    public class detailsPerPersonPagedResultVM
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

        public virtual ICollection<detailsPerPersonVM> detailsPerPerson { get; set; }
    }
}
