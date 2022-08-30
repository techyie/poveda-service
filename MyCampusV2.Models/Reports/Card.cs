using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    public class cardStudentFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public int educLevelId { get; set; }
        public int yearSecId { get; set; }
        public int studSecId { get; set; }
        public int personId { get; set; }
        public string statustype { get; set; }
    }

    public class cardEmployeesFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public int departmentId { get; set; }
        public int positionId { get; set; }
        public int personId { get; set; }
        public string statustype { get; set; }
    }

    public class cardOtherAccessFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public int departmentId { get; set; }
        public int positionId { get; set; }
        public int personId { get; set; }
        public string statustype { get; set; }
    }

    public class cardFetcherFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public int personId { get; set; }
        public string statustype { get; set; }
    }

    public class cardAllFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int campusId { get; set; }
        public int personId { get; set; }
        public string statustype { get; set; }
    }
}
