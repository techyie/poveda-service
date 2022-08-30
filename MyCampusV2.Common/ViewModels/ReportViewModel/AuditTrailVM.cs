using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using Newtonsoft.Json;

namespace MyCampusV2.Common.ViewModels
{
    public class AuditTrailVM
    {
        public int ID { get; set; }
        public int Form_ID { get; set; }
        public int User_ID { get; set; }
        public string Action { get; set; }
        public string Date { get; set; }
        public string Date2 { get; set; }
        public string Status { get; set; }

        [ForeignKey("User_ID")]
        public virtual userEntity UserEntity { get; set; }
    }

    public class AuditTrailResultVM
    {
        public int ID { get; set; }
        public string User_Name { get; set; }
        public string Source { get; set; }
        public string Category { get; set; }
        public string Log_Level { get; set; }
        public string Message { get; set; }
        public string Log_Date_Time { get; set; }
    }

    public class formAuditTrailVM
    {
        public int? form_ID { get; set; }
        public string form_Name { get; set; }
    }

    public class AuditTrailReportVM
    {
        public int ID { get; set; }
        public string User_Name { get; set; }
        public string Source { get; set; }
        public string Category { get; set; }
        public string Log_Level { get; set; }
        public string Message { get; set; }
        public string Log_Date_Time { get; set; }

        //public int ID { get; set; }
        //public int User_ID { get; set; }
        //public string User_Name { get; set; }
        //public int Form_ID { get; set; }
        //public string Form_Name { get; set; }
        //public string Source { get; set; }
        //public string Category { get; set; }
        //public string Log_Level { get; set; }
        //public string Message { get; set; }
        //public string Action { get; set; }
        //public string Date { get; set; }
        //public string Date2 { get; set; }
        //public string Status { get; set; }
    }

    public class auditTrailPagedResult
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

        public IList<AuditTrailResultVM> audittrails { get; set; }
    }


    public class auditTrailPagedResultVM
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

        public virtual ICollection<AuditTrailReportVM> audittrails { get; set; }
    }
}
