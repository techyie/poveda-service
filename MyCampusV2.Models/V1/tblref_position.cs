using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyCampusV2.Models
{
    //[Table("tblref_position")]
    public class tblref_position : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Position_ID")]
        public int Position_ID { get; set; }

        [Required]
        [Column("Position_Name")]
        [StringLength(150)]
        public string Position_Name { get; set; }

        [Column("Position_Status")]
        [StringLength(10)]
        public string Position_Status { get; set; }

        [Column("Department_ID")]
        public int Department_ID { get; set; }

        //[ForeignKey("Department_ID")]
        //public virtual tblref_department tblref_department { get; set; }
        
    }
}
