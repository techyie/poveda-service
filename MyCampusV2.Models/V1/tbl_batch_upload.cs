﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models
{
    [Table("tbl_batch_upload")]
    public class tbl_batch_upload : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        public int Form_ID { get; set; }
        public int User_ID { get; set; }

        public string Path { get; set; }
        public string Filename { get; set; }

        public int ProcessCount { get; set; }
        public int TotalCount { get; set; }
        public string Status { get; set; }

        //[ForeignKey("Form_ID")]
        //public virtual tbl_form tbl_form { get; set; }

        //[ForeignKey("User_ID")]
        //public virtual tbl_user tbl_user { get; set; }
    }
}
