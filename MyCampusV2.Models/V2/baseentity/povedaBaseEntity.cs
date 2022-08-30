using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace MyCampusV2.Models.V2.baseentity
{
    public class povedaBaseEntity
    {
        [Column("Date_Time_Added")]
        public DateTime Date_Time_Added { get; set; }

        [Column("Last_Updated")]
        public DateTime Last_Updated { get; set; }

        [Column("Added_By")]
        public int Added_By { get; set; }

        [Column("Updated_By")]
        public int Updated_By { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("ToDisplay")]
        public bool ToDisplay { get; set; }
    }
}
