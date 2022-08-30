using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models
{
    //[Table("tbl_terminal")]
    public class tbl_terminal : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Terminal_ID")]
        public int Terminal_ID { get; set; }

        [Required]
        [Column("Terminal_Code")]
        [StringLength(4000)]
        public string Terminal_Code { get; set; }

        [Required]
        [Column("Terminal_Name")]
        [StringLength(4000)]
        public string Terminal_Name { get; set; }

        [Required]
        [Column("Terminal_IP")]
        [StringLength(4000)]
        public string Terminal_IP { get; set; }

        [Required]
        [Column("Area_ID")]
        public int Area_ID { get; set; }

        [Required]
        [Column("Terminal_Category_ID")]
        [StringLength(4000)]
        public string Terminal_Category_ID { get; set; }
        
        [Column("Terminal_Status")]
        [StringLength(4000)]
        public string Terminal_Status { get; set; }

        [Required]
        [Column("IsForFetcher")]
        public bool IsForFetcher { get; set; }

        [Required]
        [Column("WithFetcher")]
        public bool WithFetcher { get; set; }

        [Required]
        [Column("HostIP")]
        [StringLength(50)]
        public string HostIP { get; set; }
        
        //[ForeignKey("Area_ID")]
        //public virtual tbl_area tbl_area { get; set; }

        //public virtual ICollection<tbl_notification> tbl_notification { get; set; }

        //public virtual ICollection<tbl_terminal_whitelist> tbl_terminal_whitelist { get; set; }
    }
}
