using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_designation")]
    public class designationEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Designation_ID")]
        public int Designation_ID { get; set; }

        [Required]
        [Column("Designation_Name")]
        [StringLength(150)]
        public string Designation_Name { get; set; }

        [Column("Position_ID")]
        public int Position_ID { get; set; }

        [Column("Designation_Status")]
        [StringLength(10)]
        public string Designation_Status { get; set; }

        [ForeignKey("Position_ID")]
        public virtual positionEntity PositionEntity { get; set; }
    }
}