using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_terminal")]
    public class terminalEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Terminal_ID")]
        public int Terminal_ID { get; set; }

        [Required]
        [Column("Terminal_Name")]
        [StringLength(2000)]
        public string Terminal_Name { get; set; }

        [Column("Terminal_Code")]
        [StringLength(2000)]
        public string Terminal_Code { get; set; }

        [Column("Terminal_IP")]
        [StringLength(2000)]
        public string Terminal_IP { get; set; }

        [Column("Area_ID")]
        public int Area_ID { get; set; }

        [Column("Terminal_Category")]
        [StringLength(1000)]
        public string Terminal_Category { get; set; }

        [Column("Terminal_Status")]
        [StringLength(50)]
        public string Terminal_Status { get; set; }

        [Column("IsForFetcher")]
        public bool IsForFetcher { get; set; }

        [Column("WithFetcher")]
        public bool WithFetcher { get; set; }

        [Column("HostIP")]
        [StringLength(50)]
        public string HostIP { get; set; }

        [ForeignKey("Area_ID")]
        public virtual areaEntity AreaEntity { get; set; }
    }
}