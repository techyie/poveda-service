using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_school_year_enrollment")]
    public class schoolYearEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int? School_Year_ID { get; set; }

        [Required]
        [Column("School_Year")]
        public string School_Year { get; set; }

        [Column("Start_Date")]
        public DateTime? Start_Date { get; set; }

        [Column("End_Date")]
        public DateTime? End_Date { get; set; }
        
    }
}
