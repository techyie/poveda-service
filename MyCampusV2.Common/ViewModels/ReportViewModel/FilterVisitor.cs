using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class FilterVisitor
    {
    }

    public class FilterVisitorLogs
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
