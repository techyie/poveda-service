using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_emergency_contact")]
    public class emergencyContactEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("EmerCon_ID")]
        public int EmerCon_ID { get; set; }

        [Column("Full_Name")]
        [StringLength(30)]
        public string Full_Name { get; set; }

        [Column("Address")]
        [StringLength(300)]
        public string Address { get; set; }

        [Column("Contact_Number")]
        [StringLength(20)]
        public string Contact_Number { get; set; }

        [Column("Relationship")]
        [StringLength(50)]
        public string Relationship { get; set; }

        [Column("Connected_PersonID")]
        public int Connected_PersonID { get; set; }

        [Column("Access_Token")]
        [StringLength(500)]
        public string Access_Token { get; set; }

        [ForeignKey("Connected_PersonID")]
        public virtual personEntity PersonEntity { get; set; }
    }
}