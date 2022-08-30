using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class personalNotificationBatchUploadVM
    {
        ////public long? notification_Id { get; set; }
        //public string message { get; set; }
        //public string date_From { get; set; }
        //public string date_To { get; set; }
        ////public long person_Id { get; set; }
        //public string id_Number { get; set; }
        //public string first_Name { get; set; }
        //public string last_Name { get; set; }
        ////public long terminal_Id { get; set; }
        //public string terminal_Name { get; set; }
        ////public long area_Id { get; set; }
        //public string area_Name { get; set; }
        ////public long campus_Id { get; set; }
        //public string campus_Name { get; set; }
        //public string status { get; set; }
        public string Message { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string IDNumber { get; set; }

        //public string CampusName { get; set; }
        //public string AreaName { get; set; }
        //public string TerminalName { get; set; }

    }

    public class personalNotificationBatchUploadHeaders
    {
        public string Message { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string IDNumber { get; set; }
        public string CampusName { get; set; }
        public string AreaName { get; set; }
        public string TerminalName { get; set; }
    }
}
