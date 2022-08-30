using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class positionBatchUploadVM
    {
        //public long campus_Id { get; set; }
        //public string campus_Name { get; set; }
        //public long department_Id { get; set; }
        //public string department_Name { get; set; }
        //public string position_Name { get; set; }
        //public string position_Desc { get; set; }

        public string CampusName { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public string PositionDescription { get; set; }
    }

    public class positionBatchUploadHeaders
    {
        public string CampusName { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public string PositionDescription { get; set; }
    }
}
