using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyCampusV2.Common.ViewModels
{
    public class FilterEmployee
    {
       public DateTime? from {get;set;}
       public DateTime? to {get;set;}
       public string filter {get;set;}
       public int? campus {get;set;}
       public int? area {get;set;}
       public int? terminal {get;set;}
       public int? department {get;set;}
       public int? position {get;set;}
    }

    public class FilterEmployeeLogs
    {
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
        public string filter { get; set; }
        public int? campus_ID { get; set; }
        public int? area_ID { get; set; }
        public int? terminal_ID { get; set; }
        public string logstat { get; set; }
    }
}
