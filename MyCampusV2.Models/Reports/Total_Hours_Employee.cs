using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.Reports
{
    public class Total_Hours_Employee
    {
        public string ID_Number { get; set; }
        public string Full_Name { get; set; }
        public string Department_Name { get; set; }
        public string Employee_Type { get; set; }
        public string Total_Hours { get; set; }
    }

    public class TotalHoursEmployeeVM
    {
        public string IDNumber { get; set; }
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeType { get; set; }
    }

    public class totalHoursEmployeeFilter
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string department_ID { get; set; }
        public string emptype { get; set; }
        public string keyword { get; set; }
    }
}
