using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class roleModuleVM
    {
        public int Module_ID { get; set; }
        public bool Can_Access { get; set; }
        public bool Can_Insert { get; set; }
        public bool Can_Update { get; set; }
        public bool Can_Delete { get; set; }
    }
}
