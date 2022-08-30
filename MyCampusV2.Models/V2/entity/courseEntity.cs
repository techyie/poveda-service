using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tblref_course")]
    public class courseEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Course_ID")]
        public int Course_ID { get; set; }

        [Required]
        [Column("Course_Name")]
        [StringLength(150)]
        public string Course_Name { get; set; }

        [Column("Course_Status")]
        [StringLength(10)]
        public string Course_Status { get; set; }

        [Column("College_ID")]
        public int College_ID { get; set; }

        [ForeignKey("College_ID")]
        public virtual collegeEntity CollegeEntity { get; set; }

        //public virtual ICollection<collegeEntity> CollegeList { get; set; }
    }
}