using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models
{
    [Table("tbl_visitor")]
    public class Visitor 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int id { get; set; }

        [Required]
        [Column("First_Name")]
        public string firstName { get; set; }

        [Column("Middle_Name")]
        public string middleName { get; set; }

        [Required]
        [Column("Last_Name")]
        public string lastName { get; set; }

        [Required]
        [Column("Address")]
        public string address { get; set; }

        public int Campus_ID { get; set; }

        [Column("Gender")]
        public string Gender { get; set; }

        [Column("Name_Of_Employer")]
        public string Name_Of_Employer { get; set; }

        //public virtual VisitorInformation VisitorInformation {get;set;}
        //[ForeignKey("Campus_ID")]
        //public virtual tblref_campus tbl_campus {get;set;}

    }
}
