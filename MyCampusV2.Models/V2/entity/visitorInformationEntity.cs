using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_visitor_information")]
    public class visitorInformationEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Visitor_Info_ID")]
        public int Visitor_Info_ID { get; set; }

        [Column("Cardholder_ID")]
        public int Cardholder_ID { get; set; }

        [Column("Area_ID")]
        public int Area_ID { get; set; }

        [Column("Full_Name")]
        [StringLength(250)]
        public string Full_Name { get; set; }

        [Column("ID_Number")]
        [StringLength(50)]
        public string ID_Number { get; set; }

        [Column("Remarks")]
        [StringLength(300)]
        public string Remarks { get; set; }

        [Column("Schedule_Date")]
        public DateTime Schedule_Date { get; set; }

        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Column("Is_Attended")]
        public bool Is_Attended { get; set; }

        [Column("Presented_ID_Number")]
        [StringLength(100)]
        public string Presented_ID_Number { get; set; }

        [Column("Presented_ID_Type")]
        [StringLength(100)]
        public string Presented_ID_Type { get; set; }

        [Column("Date_Time_Registered")]
        public DateTime Date_Time_Registered { get; set; }

        [Column("Date_Time_Surrender")]
        public DateTime Date_Time_Surrender { get; set; }
    }
}