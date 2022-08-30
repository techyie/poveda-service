using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_campus")]
    public class campusEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        [Required]
        [Column("Campus_Code")]
        [StringLength(100)]
        public string Campus_Code { get; set; }

        [Required]
        [Column("Campus_Name")]
        [StringLength(100)]
        public string Campus_Name { get; set; }

        [Column("Campus_Status")]
        [StringLength(10)]
        public string Campus_Status { get; set; }

        [Column("Campus_Address")]
        [StringLength(300)]
        public string Campus_Address { get; set; }

        [Column("Campus_ContactNo")]
        [StringLength(20)]
        public string Campus_ContactNo { get; set; }

        [Column("Division_ID")]
        public int Division_ID { get; set; }

        [ForeignKey("Division_ID")]
        public virtual divisionEntity DivisionEntity { get; set; }

    }
}
