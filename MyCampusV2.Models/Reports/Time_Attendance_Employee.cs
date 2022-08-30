using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    public class TimeAttendanceEmployeeEntity
    {
        //public long ID { get; set; }
        public DateTime LogDate { get; set; }
        public string Status { get; set; }

        public long Campus_ID { get; set; }
        public long Department_ID { get; set; }
        public long Area_ID { get; set; }
        public long Terminal_ID { get; set; }
        public long Person_ID { get; set; }
        public long Card_Details_ID { get; set; }

        //[ForeignKey("Card_Details_ID")]
        //public virtual tbl_card_details tbl_card_details { get; set; }

        //[ForeignKey("Person_ID")]
        //public virtual tbl_person tbl_person { get; set; }

        //[ForeignKey("Campus_ID")]
        //public virtual tblref_campus tbl_campus { get; set; }

        //[ForeignKey("Area_ID")]
        //public virtual tbl_area tbl_area { get; set; }

        //[ForeignKey("Terminal_ID")]
        //public virtual tbl_terminal tbl_terminal { get; set; }
    }

    public class TimeAttendanceEmployeeVM
    {
        public string IDNumber { get; set; }
        public string Name { get; set; }
        public string CampusName { get; set; }
        public string DepartmentName { get; set; }
        public string AreaName { get; set; }
        public string TerminalName { get; set; }
        public string LogDate { get; set; }
        public string Status { get; set; }
    }

    public class timeAttendanceEmployeeFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string filter { get; set; }
        public long campus_ID { get; set; }
        public long department_ID { get; set; }
        public long area_ID { get; set; }
        public long terminal_ID { get; set; }
        public string logstat { get; set; }
        public string keyword { get; set; }
    }
}
