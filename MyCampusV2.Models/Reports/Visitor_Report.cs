using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    public class Visitor_Report
    {
        public string Campus { get; set; }
        public int Campus_ID { get; set; }
        public string Area { get; set; }
        public string Terminal { get; set; }
        public string ID_Number { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Log_Message { get; set; }
    }

    public class VisitorReportEntity
    {
        public long VisitorInformationID { get; set; }
        public string TrackingCode { get; set; }
        public string VisitorName { get; set; }
        public string Gender { get; set; }
        public string PresentedID { get; set; }
        public string PresentedIDType { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public DateTime? DateTimeTagged { get; set; }
        public DateTime? DateTimeSurrendered { get; set; }
        public string VisitorCard { get; set; }
        public string VisitedPerson { get; set; }
        public string VisitPurpose { get; set; }
        public string ManualOutRemarks { get; set; }
        public long CardDetailsID { get; set; }
    }

    public class VisitorReportVM
    {
        public long VisitorInformationID { get; set; }
        public string TrackingCode { get; set; }
        public string VisitorName { get; set; }
        public string Gender { get; set; }
        public string PresentedID { get; set; }
        public string PresentedIDType { get; set; }
        public string ScheduleDate { get; set; }
        public string DateTimeTagged { get; set; }
        public string DateTimeSurrendered { get; set; }
        public string VisitorCard { get; set; }
        public string VisitedPerson { get; set; }
        public string VisitPurpose { get; set; }
        public string ManualOutRemarks { get; set; }
        public long CardDetailsID { get; set; }
    }

    public class visitorReportFilter
    {
        public string from { get; set; }
        public string to { get; set; }
        public string filter { get; set; }
    }
}
