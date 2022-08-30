using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    public class AlarmEmployeeEntity
    {
        public string IDNumber { get; set; }
        public string Name { get; set; }
        public string CampusName { get; set; }
        public string AreaName { get; set; }
        public string TerminalName { get; set; }
        public DateTime LogDate { get; set; }
        public string Status { get; set; }
    }

    //public class AlarmEmployeeVM
    //{
    //    public string IDNumber { get; set; }
    //    public string Name { get; set; }
    //    public string CampusName { get; set; }
    //    public string AreaName { get; set; }
    //    public string TerminalName { get; set; }
    //    public string LogDate { get; set; }
    //    public string Status { get; set; }
    //}

    public class alarmEmployeeFilter
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
