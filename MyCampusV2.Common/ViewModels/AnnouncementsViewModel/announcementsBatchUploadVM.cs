using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels.AnnouncementsViewModel
{
    public class announcementsBatchUploadVM
    {
        public string announcementsCode { get; set; }

        public string title { get; set; }

        public string body { get; set; }

        public string dateSent { get; set; }

        public bool isAll { get; set; }

        public bool isSent { get; set; }

        public string recipientsDisplay { get; set; }
    }

    public class announcementsBatchUploadHeaders
    {
        public string announcementsCode { get; set; }

        public string title { get; set; }

        public string body { get; set; }

        public string dateSent { get; set; }

        public bool isAll { get; set; }

        public bool isSent { get; set; }

        public string recipientsDisplay { get; set; }
    }
}
