using System;
using MyCampusV2.Models.V2.baseentity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_excused_student")]
    public class excusedStudentEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("IDNumber")]
        [StringLength(45)]
        public string IDNumber { get; set; }

        [Column("Excused_Date")]
        public DateTime Excused_Date { get; set; }

        [NotMapped]
        public DateTime Excused_Start_Date { get; set; }

        [NotMapped]
        public DateTime Excused_End_Date { get; set; }
    }
}