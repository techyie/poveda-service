using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class DepedReportVM
    {
        public int Day { get; set; }
        public string NameOfTheDay { get; set; }
    }
    public class DepedReportHeaderVM
    {
        public string Region { get; set; }
        public string Division { get; set; }
        public string SchoolID { get; set; }
        public string SchoolYear { get; set; }
        public string SchoolName { get; set; }
        public string Month { get; set; }
        public string GradeLevel { get; set; }
        public string Section { get; set; }
        public string Password { get; set; }
        public DateTime EnrollmentEnd { get; set; }
    }

    public class ScheduleVM
    {
        public DateTime Date { get; set; }
        public string NameOfTheDay { get; set; }
        public bool withClasses { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan HalfDay { get; set; }
        public int GracePeriod { get; set; }
        public bool isExcused { get; set; }
    }

    public class RecordsVM
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Dates { get; set; }
        public string Status { get; set; }
        public double Absent { get; set; }
        public double Tardy { get; set; }
        public bool TransferredIn { get; set; }
        public bool TransferredOut { get; set; }
        public string SchoolName { get; set; }
        public bool DropOut { get; set; }
        public string DropOutCode { get; set; }
        public DateTime DateEnrolled { get; set; }
    }
}
