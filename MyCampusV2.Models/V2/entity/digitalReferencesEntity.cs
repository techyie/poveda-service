using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_digital_references")]
    public class digitalReferencesEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Digital_Reference_Code")]
        [StringLength(225)]
        public string Digital_Reference_Code { get; set; }

        [Required]
        [Column("Title")]
        [StringLength(500)]
        public string Title { get; set; }

        [Column("File_Name")]
        [StringLength(225)]
        public string File_Name { get; set; }

        [Column("File_Type")]
        [StringLength(50)]
        public string File_Type { get; set; }

        [Column("File_Path")]
        [StringLength(3000)]
        public string File_Path { get; set; }

        [Column("Date_Uploaded")]
        public DateTime Date_Uploaded { get; set; }
    }
}
