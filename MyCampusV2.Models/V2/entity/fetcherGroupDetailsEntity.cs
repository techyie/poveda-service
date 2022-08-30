using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_fetcher_group_details")]
    public class fetcherGroupDetailsEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Group_Detail_ID")]
        public int Group_Detail_ID { get; set; }

        [Required]
        [Column("Fetcher_Group_ID")]
        public int Fetcher_Group_ID { get; set; }

        [ForeignKey("Fetcher_Group_ID")]
        public virtual fetcherGroupEntity FetcherGroupEntity { get; set; }

        [Required]
        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [ForeignKey("Person_ID")]
        public virtual personEntity PersonEntity { get; set; }
    }
}