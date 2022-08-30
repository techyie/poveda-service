using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    public class Card_Employee
    {
        public string ID_Number {get;set;}
        public string Name {get;set;}
        public string Department {get;set;}
        public int Campus_ID { get; set; }
        public string Position {get;set;}
        public bool IsActive {get;set;}
        public DateTime Issued_Date {get;set;}
        public DateTime Last_Updated {get;set;}
    }
}
