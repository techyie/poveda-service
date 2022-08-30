using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class batchUploadVM
    {
        public long ID { get; set; }

        public int Form_ID { get; set; }
        public int User_ID { get; set; }

        public string Path { get; set; }
        public string Filename { get; set; }

        public int ProcessCount { get; set; }
        public int TotalCount { get; set; }
        public string Status { get; set; }
    }
}
