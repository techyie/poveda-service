using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_pap_acct_linked_students")]
    public class papAccountLinkedStudentsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("PAP_Account_ID")]
        public int PAP_Account_ID { get; set; }

        [ForeignKey("PAP_Account_ID")]
        public virtual papAccountEntity PapAccountEntity { get; set; }

        [Required]
        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [ForeignKey("Person_ID")]
        public virtual personEntity PersonEntity { get; set; }
    }
}
