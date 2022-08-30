using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Models
{
    //public class Time_Attendance_Visitor
    //{
    //    public string IDNumber { get; set; }
    //    public string Name { get; set; }
    //    public string CampusName { get; set; }
    //    public string AreaName { get; set; }
    //    public string TerminalName { get; set; }
    //    public string LogDate { get; set; }
    //    public string Status { get; set; }
    //}

    //public class Time_Attendance_VisitorVM
    //{
    //    public string IDNumber { get; set; }
    //    public string Name { get; set; }
    //    public string CampusName { get; set; }
    //    public string AreaName { get; set; }
    //    public string TerminalName { get; set; }
    //    public DateTime LogDate { get; set; }
    //    public string Status { get; set; }
    //}

    public class TimeAttendanceVisitorEntity
    {
        public string IDNumber { get; set; }
        public string Name { get; set; }
        public string CampusName { get; set; }
        public string AreaName { get; set; }
        public string TerminalName { get; set; }
        public DateTime LogDate { get; set; }
        public string Status { get; set; }
    }

    public class TimeAttendanceVisitorVM
    {
        public string IDNumber { get; set; }
        public string Name { get; set; }
        public string CampusName { get; set; }
        public string AreaName { get; set; }
        public string TerminalName { get; set; }
        public string LogDate { get; set; }
        public string Status { get; set; }
    }

    public class timeAttendanceVisitorFilter
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
