using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class terminalConfigVM
    {
        public int? configId { get; set; }
        public int? terminalId { get; set; }
        public string terminalName { get; set; }
        public int? areaId { get; set; }
        public string areaName { get; set; }
        public int? campusId { get; set; }
        public string campusName { get; set; }
        public string terminalSchedule { get; set; }
        public string schoolName { get; set; }
        public string terminalCode { get; set; }
        public string hostIPAddress1 { get; set; }
        public string hostPort1 { get; set; }
        public string hostIPAddress2 { get; set; }
        public string hostPort2 { get; set; }
        public string viewerAddress { get; set; }
        public string viewerPort { get; set; }
        public string readerName1 { get; set; }
        public string readerDirection1 { get; set; }
        public bool enableAntipassback1 { get; set; }
        public string readerName2 { get; set; }
        public string readerDirection2 { get; set; }
        public bool enableAntipassback2 { get; set; }
        public int? loopDelay { get; set; }
        public int? turnstileDelay { get; set; }
        public int? terminalSyncInterval { get; set; }
        public string viewerDB { get; set; }
        public string serverDB { get; set; }
    }


}
