using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyCampusV2.Common.ViewModels
{
    public class FilterStudent
    {
       public DateTime? from {get;set;}
       public DateTime? to {get;set;}
       public string filter {get;set;}
       public int? campus {get;set;}
       public int? area {get;set;}
       public int? terminal {get;set;}
       public int? educlevel {get;set;}
       public int? year {get;set;}
       public int? section {get;set;}
       public int? college {get;set;}
       public int? course {get;set;}
    }

    public class FilterStudentLogs
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
