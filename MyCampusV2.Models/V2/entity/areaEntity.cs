﻿using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_area")]
    public class areaEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Area_ID")]
        public int Area_ID { get; set; }

        [Required]
        [Column("Area_Name")]
        [StringLength(100)]
        public string Area_Name { get; set; }

        [Column("Area_Description")]
        [StringLength(300)]
        public string Area_Description { get; set; }

        [Column("Area_Status")]
        [StringLength(10)]
        public string Area_Status { get; set; }

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        [ForeignKey("Campus_ID")]
        public virtual campusEntity CampusEntity { get; set; }

        //public virtual ICollection<campusEntity> CampusList { get; set; }
    }
}