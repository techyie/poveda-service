using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models
{
    [Table("tbl_visitor_information")]
    public class VisitorInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Visitor_Info_ID")]
        public int Visitor_Info_ID { get; set; }

        [Required]
        [Column("Cardholder_ID")]
        public int Cardholder_ID { get; set; }

        [Required]
        [Column("Area_ID")]
        public int Area_ID { get; set; }

        [Required]
        [Column("Full_Name")]
        [StringLength(250)]
        public string Full_Name { get; set; }

        [Required]
        [Column("ID_Number")]
        [StringLength(50)]
        public int ID_Number { get; set; }

        [Required]
        [Column("Remarks")]
        [StringLength(300)]
        public long Remarks { get; set; }
        
        [Required]
        [Column("Schedule_Date")]
        public DateTime Schedule_Date { get; set; }

        [Required]
        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Required]
        [Column("Is_Attended")]
        public bool Is_Attended { get; set; }

        [Required]
        [Column("Presented_ID_Number")]
        [StringLength(100)]
        public long Presented_ID_Number { get; set; }

        [Required]
        [Column("Presented_ID_Type")]
        [StringLength(100)]
        public long Presented_ID_Type { get; set; }

        [Required]
        [Column("Date_Time_Registered")]
        public DateTime Date_Time_Registered { get; set; }

        [Required]
        [Column("Date_Time_Surrender")]
        public DateTime Date_Time_Surrender { get; set; }

        //[ForeignKey("Cardholder_ID")]
        //public virtual tbl_card_details tbl_card_details { get;set; }

        //[ForeignKey("Area_ID")]
        //public virtual tblref_area tblref_area { get; set; }

        //[ForeignKey("Person_ID")]
        //public virtual tbl_person tbl_person { get; set; }


    }
}
