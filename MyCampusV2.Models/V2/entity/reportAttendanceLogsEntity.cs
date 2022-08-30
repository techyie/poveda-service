using MyCampusV2.Models.V2.baseentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCampusV2.Models.V2.entity
{
    [Table("report_attendance_logs")]
    public class reportAttendanceLogsEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Column("ID_Number")]
        [StringLength(50)]
        public string ID_Number { get; set; }

        [Column("FullName")]
        [StringLength(500)]
        public string FullName { get; set; }

        [Column("Person_Type")]
        [StringLength(1)]
        public string Person_Type { get; set; }

        [Column("CardHolderID")]
        public int CardHolderID { get; set; }

        [Column("LogDate")]
        public DateTime LogDate { get; set; }

        [Column("LogIn")]
        [StringLength(50)]
        public string LogIn { get; set; }

        [Column("LogOut")]
        [StringLength(50)]
        public string LogOut { get; set; }

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        [Column("Campus_Name")]
        [StringLength(50)]
        public string Campus_Name { get; set; }

        [Column("Level_ID")]
        public int Level_ID { get; set; }

        [Column("Level_Name")]
        [StringLength(50)]
        public string Level_Name { get; set; }

        [Column("YearSec_ID")]
        public int YearSec_ID { get; set; }

        [Column("YearSec_Name")]
        [StringLength(50)]
        public string YearSec_Name { get; set; }

        [Column("StudSec_ID")]
        public int StudSec_ID { get; set; }

        [Column("Description")]
        [StringLength(50)]
        public string Description { get; set; }

        [Column("EmpType_ID")]
        public int EmpType_ID { get; set; }

        [Column("EmpTypeDesc")]
        [StringLength(50)]
        public string EmpTypeDesc { get; set; }

        [Column("EmpSubtype_ID")]
        public int EmpSubtype_ID { get; set; }

        [Column("EmpSubTypeDesc")]
        [StringLength(50)]
        public string EmpSubTypeDesc { get; set; }

        [Column("Office_ID")]
        public int Office_ID { get; set; }

        [Column("Office_Name")]
        [StringLength(50)]
        public string Office_Name { get; set; }

        [ForeignKey("Person_ID")]
        public virtual personEntity PersonEntity { get; set; }
    }
}
