using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class schoolCalendarBatchUploadVM
    {
        public string SchoolYear { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Days { get; set; }
    }

    public class schoolCalendarBatchUploadHeaders
    {
        public string SchoolYear { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Days { get; set; }
    }
}
