using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class areaBatchUploadVM
    {
        //public long campus_Id { get; set; }
        //public string campus_Name { get; set; }
        //public string area_Name { get; set; }
        //public string area_Desc { get; set; }

        public string CampusName { get; set; }
        public string AreaName { get; set; }
        public string AreaDescription { get; set; }
    }

    public class areaBatchUploadHeaders
    {
        public string CampusName { get; set; }
        public string AreaName { get; set; }
        public string AreaDescription { get; set; }
    }
}
