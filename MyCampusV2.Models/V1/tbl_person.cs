using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_person")]
    public class tbl_person : IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Person_ID")]
        public int Person_ID { get; set; }
        [Column("Person_Type")]
        public string Person_Type { get; set; }
        [Column("ID_Number")]
        public string ID_Number { get; set; }
        [Column("First_Name")]
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public DateTime Birthdate { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Contact_Number { get; set; }
        public string Email_Address { get; set; }

        public int Campus_ID { get; set; }
        public int Educ_Level_ID { get; set; }
        public int College_ID { get; set; }
        public int Course_ID { get; set; }
        public int Year_Section_ID { get; set; }
        public int Department_ID { get; set; }
        public int Position_ID { get; set; }
        public int EmpType_ID { get; set; }
        public int EmpSubtype_ID { get; set; }
        public int Office_ID { get; set; }
        //public int Designation_ID { get; set; }

        public string Fetcher_Relationship { get; set; }
        public string StudSec_ID { get; set; }
        public DateTime Separated_Date { get; set; }
        public string Access_Token { get; set; }
        public bool Is_Subscribed { get; set; }
        public string Telephone_Number { get; set; }
        public string DateEnrolled { get; set; }
        public bool IsDropOut { get; set; }
        public string DropOutCode { get; set; }
        public bool IsTransferred { get; set; }
        public string TransferredSchoolName { get; set; }
        public string DropOutOtherRemark { get; set; }
        public bool IsTransferredIn { get; set; }
        public string TransferredInSchoolName { get; set; }

        //[ForeignKey("Campus_ID")]
        //public tblref_campus tblref_campus { get; set; }

        //[ForeignKey("Educ_Level_ID")]
        //public tblref_educ_level tblref_educ_level { get; set; }

        //[ForeignKey("College_ID")]
        //public tblref_college tblref_college { get; set; }

        //[ForeignKey("Course_ID")]
        //public tblref_course tblref_course { get; set; }

        //[ForeignKey("Year_Section_ID")]
        //public tblref_year_section tblref_year_section { get; set; }
        
        //[ForeignKey("Department_ID")]
        //public tblref_department tblref_department { get; set; }
        
        //[ForeignKey("Position_ID")]
        //public tblref_position tblref_position { get; set; }

        //[ForeignKey("EmpType_ID")]
        //public tbl_employee_type tbl_employee_type { get; set; }

        //[ForeignKey("EmpSubtype_ID")]
        //public tbl_employee_subtype tbl_employee_subtype { get; set; }

        //[ForeignKey("Office_ID")]
        //public tbl_office tbl_office { get; set; }

        //[ForeignKey("Designation_ID")]
        //public tblref_designation tbl_designation { get; set; }

        //public virtual tbl_emergency_contact tbl_emergency_contact { get; set; }
        //public virtual tbl_gov_ids tbl_gov_ids { get; set; }
        //public virtual ICollection<tbl_card_details> tbl_card_details { get; set; }
        //public virtual tbl_user tbl_user { get; set; }
    }
}
