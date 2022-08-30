using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace MyCampusV2.Common.ViewModels
{
    public class departmentBatchUploadVM
    {
       // public long campus_Id { get; set; }
       [Column("campus_Name")]
        public string CampusName { get; set; }
        [Column("department_Name")]
        public string DepartmentName { get; set; }
    }

    public class departmentBatchUploadHeaders
    {
        public string CampusName { get; set; }
        public string DepartmentName { get; set; }
    }

}
