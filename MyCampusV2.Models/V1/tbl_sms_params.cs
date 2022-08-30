using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models
{
    [Table("tbl_sms_params")]
    public class tbl_sms_params : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? Param_ID { get; set; }

        public string Param_Name { get; set; }
        //public bool IsActive { get; set; }
        //public long? Added_By { get; set; }
        //public DateTime Date_Time_Added { get; set; }
        //public long? Updated_By { get; set; }
        //public DateTime Last_Updated { get; set; }
    }
}

