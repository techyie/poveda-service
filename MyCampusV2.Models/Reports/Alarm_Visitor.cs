using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Models
{
    public class AlarmVisitorEntity
    {
        public string IDNumber { get; set; }
        public string Name { get; set; }
        public string CampusName { get; set; }
        public string AreaName { get; set; }
        public string TerminalName { get; set; }
        public DateTime LogDate { get; set; }
        public string Status { get; set; }
    }

    //public class AlarmVisitorVM
    //{
    //    public string IDNumber { get; set; }
    //    public string Name { get; set; }
    //    public string CampusName { get; set; }
    //    public string AreaName { get; set; }
    //    public string TerminalName { get; set; }
    //    public string LogDate { get; set; }
    //    public string Status { get; set; }
    //}

    public class alarmVisitorFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string filter { get; set; }
        public int? campus_ID { get; set; }
        public int? area_ID { get; set; }
        public int? terminal_ID { get; set; }
        public string logstat { get; set; }
    }
}
