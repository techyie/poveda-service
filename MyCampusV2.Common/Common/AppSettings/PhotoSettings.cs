using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common
{
    public static class PhotoStatSettings
    {
        public static string photopath { get; set; }
        public static string dumppath { get; set; }
    }

    public class PhotoSettings
    {
        public string loc { get; set; }
        public string dumpdestination { get; set; }
    }
}
