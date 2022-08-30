using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_gov_ids")]
    public class govIdsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Column("SSS")]
        [StringLength(30)]
        public string SSS { get; set; }

        [Column("TIN")]
        [StringLength(30)]
        public string TIN { get; set; }

        [Column("PhilHealth")]
        [StringLength(30)]
        public string PhilHealth { get; set; }

        [Column("PAG_IBIG")]
        [StringLength(30)]
        public string PAG_IBIG { get; set; }

        [Column("Date_Added")]
        public DateTime Date_Added { get; set; }

        [Column("Last_Updated")]
        public DateTime Last_Updated { get; set; }
    }
}