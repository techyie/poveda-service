using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    public class AuditTrail
    {
        public string User_Name { get;set;}
        public string Source { get;set;}
        public string Category { get;set;}
        public string Log_Level { get; set; }
        public DateTime Log_Date_Time { get;set;}
        public string Message { get; set; }
        public int Campus_ID { get; set; }
    }
}
