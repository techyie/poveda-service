using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_fetcher_group")]
    public class fetcherGroupEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Fetcher_Group_ID")]
        public int Fetcher_Group_ID { get; set; }

        [Required]
        [Column("Group_Name")]
        [StringLength(250)]
        public string Group_Name { get; set; }

        [Required]
        [Column("User_ID")]
        public int User_ID { get; set; }

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        [ForeignKey("Campus_ID")]
        public virtual campusEntity CampusEntity { get; set; }
    }
}
