using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_excused_section")]
    public class excusedSectionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Section_ID")]
        public int Section_ID { get; set; }

        [Column("Excused_Date")]
        public DateTime Excused_Date { get; set; }
    }
}