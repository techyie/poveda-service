using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    public class TimeAttendanceEntity
    {
        public string IDNumber { get; set; }
        public string Name { get; set; }
        public string CampusName { get; set; }
        public string AreaName { get; set; }
        public string TerminalName { get; set; }
        public DateTime LogDate { get; set; }
        public string Status { get; set; }
    }

    public class TimeAttendanceVM
    {
        public string IDNumber { get; set; }
        public string Name { get; set; }
        public string CampusName { get; set; }
        public string AreaName { get; set; }
        public string TerminalName { get; set; }
        public string LogDate { get; set; }
        public string Status { get; set; }
    }

    public class timeAttendanceFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string filter { get; set; }
        public int? campus_ID { get; set; }
        public int? area_ID { get; set; }
        public int? terminal_ID { get; set; }
        public string logstat { get; set; }
    }

    public class timeAttendanceStudentFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public int educLevelId { get; set; }
        public int yearSecId { get; set; }
        public int studSecId { get; set; }
        public string persontype { get; set; }
        public int personId { get; set; }
        public string reportType { get; set; }
    }

    public class timeAttendanceEmployeesFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public int employeeTypeId { get; set; }
        public int employeeSubTypeId { get; set; }
        public string persontype { get; set; }
        public int personId { get; set; }
        public string reportType { get; set; }
    }

    public class timeAttendanceOtherAccessFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public int officeId { get; set; }
        public string persontype { get; set; }
        public int personId { get; set; }
        public string reportType { get; set; }
    }

    public class timeAttendanceFetcherFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public string persontype { get; set; }
        public int personId { get; set; }
        public string reportType { get; set; }
    }

    public class timeAttendanceAllFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public string persontype { get; set; }
        public int personId { get; set; }
        public string reportType { get; set; }
    }
}
