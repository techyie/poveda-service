using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class notificationBatchUploadVM
    {
        //public string message { get; set; }
        //public string date_From { get; set; }
        //public string date_To { get; set; }
        //public string terminal_Name { get; set; }
        //public string area_Name { get; set; }
        //public string campus_Name { get; set; }
        //public string status { get; set; }
        public string Message { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        //public string IDNumber { get; set; }
        //public string CampusName { get; set; }
        //public string AreaName { get; set; }
        //public string TerminalName { get; set; }
    }

    public class notificationBatchUploadHeaders
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
