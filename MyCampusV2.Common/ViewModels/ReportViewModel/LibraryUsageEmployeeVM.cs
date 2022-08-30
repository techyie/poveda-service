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
    public class LibraryUsageEmployeeVM
    {
        [Key]
        public int ID { get; set; }
        public string ID_Number { get; set; }
        public string Name { get; set; }
        public string Person_Type { get; set; }
        public string Department_Name { get; set; }
        public string Position_Name { get; set; }
        public string Date { get; set; }
        public string Usage_Count { get; set; }
    }

    public class LibraryUsageEmployeeResultVM
    {
        [Key]
        public int ID { get; set; }
        public string ID_Number { get; set; }
        public string Name { get; set; }
        public string Person_Type { get; set; }
        public string Department_Name { get; set; }
        public string Position_Name { get; set; }
        public string Date { get; set; }
        public string Usage_Count { get; set; }
    }

    public class libraryUsageEmployeePagedResult
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

        public IList<LibraryUsageEmployeeResultVM> librarylogs { get; set; }
    }


    public class libraryUsageEmployeePagedResultVM
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

        public virtual ICollection<LibraryUsageEmployeeVM> librarylogs { get; set; }
    }
}
