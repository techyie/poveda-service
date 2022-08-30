using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    public class alarmStudentFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public int educLevelId { get; set; }
        public int yearSecId { get; set; }
        public int studSecId { get; set; }
        public string persontype { get; set; }
        public int personId { get; set; }
        public string logMessage { get; set; }
    }

    public class alarmEmployeesFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public string persontype { get; set; }
        public int personId { get; set; }
        public string logMessage { get; set; }
    }

    public class alarmOtherAccessFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public string persontype { get; set; }
        public int personId { get; set; }
        public string logMessage { get; set; }
    }

    public class alarmFetcherFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public string persontype { get; set; }
        public int personId { get; set; }
        public string logMessage { get; set; }
    }

    public class alarmAllFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public string persontype { get; set; }
        public int personId { get; set; }
        public string logMessage { get; set; }
    }
}
