using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_region")]
    public class tbl_region
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Code")]
        [StringLength(30)]
        public string Code { get; set; }

        [Required]
        [Column("Name")]
        [StringLength(225)]
        public string Name { get; set; }

        //public virtual ICollection<tbl_division> tbl_division { get; set; }
    }
}
