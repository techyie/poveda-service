using MyCampusV2.Models.V2.baseentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace MyCampusV2.Models.V2.entity
{
    [Table("tbl_person")]
    public class personEntity : povedaBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Person_ID")]
        public int Person_ID { get; set; }

        [Required]
        [Column("Person_Type")]
        [StringLength(1)]
        public string Person_Type { get; set; }

        [Required]
        [Column("ID_Number")]
        [StringLength(100)]
        public string ID_Number { get; set; }

        [Column("First_Name")]
        [StringLength(100)]
        public string First_Name { get; set; }

        [Column("Middle_Name")]
        [StringLength(50)]
        public string Middle_Name { get; set; }

        [Column("Last_Name")]
        [StringLength(100)]
        public string Last_Name { get; set; }

        [Column("Birthdate")]
        public DateTime Birthdate { get; set; }

        [Column("Gender")]
        [StringLength(1)]
        public string Gender { get; set; }

        [Column("Address")]
        [StringLength(500)]
        public string Address { get; set; }

        [Column("Contact_Number")]
        [StringLength(300)]
        public string Contact_Number { get; set; }

        [Column("Email_Address")]
        [StringLength(100)]
        public string Email_Address { get; set; }

        [Column("Fetcher_Relationship")]
        [StringLength(50)]
        public string Fetcher_Relationship { get; set; }

        [Column("Telephone_Number")]
        [StringLength(20)]
        public string Telephone_Number { get; set; }

        /* References */

        [Column("Campus_ID")]
        public int Campus_ID { get; set; }

        [Column("Educ_Level_ID")]
        public int Educ_Level_ID { get; set; }

        [Column("College_ID")]
        public int College_ID { get; set; }

        [Column("Course_ID")]
        public int Course_ID { get; set; }

        [Column("Year_Section_ID")]
        public int Year_Section_ID { get; set; }

        [Column("StudSec_ID")]
        public int StudSec_ID { get; set; }

        [Column("Department_ID")]
        public int Department_ID { get; set; }

        [Column("Position_ID")]
        public int Position_ID { get; set; }

        [Column("EmpType_ID")]
        public int EmpType_ID { get; set; }

        [Column("EmpSubtype_ID")]
        public int EmpSubtype_ID { get; set; }

        [Column("Office_ID")]
        public int Office_ID { get; set; }

        [Column("Designation_ID")]
        public int Designation_ID { get; set; }

        /* Deped References */

        [Column("Separated_Date")]
        public DateTime Separated_Date { get; set; }

        [Column("Is_Subscribed")]
        public bool Is_Subscribed { get; set; }

        [Column("DateEnrolled")]
        public DateTime DateEnrolled { get; set; }

        [Column("IsDropOut")]
        public bool IsDropOut { get; set; }

        [Column("DropOutCode")]
        [StringLength(245)]
        public string DropOutCode { get; set; }

        [Column("IsTransferred")]
        public bool IsTransferred { get; set; }

        [Column("TransferredSchoolName")]
        [StringLength(145)]
        public string TransferredSchoolName { get; set; }

        [Column("DropOutOtherRemark")]
        [StringLength(245)]
        public string DropOutOtherRemark { get; set; }

        [Column("IsTransferredIn")]
        public bool IsTransferredIn { get; set; }

        [Column("TransferredInSchoolName")]
        [StringLength(145)]
        public string TransferredInSchoolName { get; set; }


        [NotMapped]
        public string EducStatus { get; set; }

        [NotMapped]
        public string DropOutType { get; set; }

        [NotMapped]
        public string DropOutDesc { get; set; }

        [NotMapped]
        public string DropOutRemarks { get; set; }

        [NotMapped]
        public string SchoolName { get; set; }

        /* Foreign Key */

        [ForeignKey("Campus_ID")]
        public campusEntity CampusEntity { get; set; }

        [ForeignKey("Educ_Level_ID")]
        public educationalLevelEntity EducationalLevelEntity { get; set; }

        [ForeignKey("College_ID")]
        public collegeEntity CollegeEntity { get; set; }

        [ForeignKey("Course_ID")]
        public courseEntity CourseEntity { get; set; }

        [ForeignKey("StudSec_ID")]
        public studentSectionEntity StudentSectionEntity { get; set; }

        [ForeignKey("Year_Section_ID")]
        public yearSectionEntity YearSectionEntity { get; set; }

        [ForeignKey("Department_ID")]
        public departmentEntity DepartmentEntity { get; set; }

        [ForeignKey("Position_ID")]
        public positionEntity PositionEntity { get; set; }

        [ForeignKey("EmpType_ID")]
        public empTypeEntity EmployeeTypeEntity { get; set; }

        [ForeignKey("EmpSubtype_ID")]
        public employeeSubTypeEntity EmployeeSubTypeEntity { get; set; }

        [ForeignKey("Office_ID")]
        public officeEntity OfficeEntity { get; set; }

        [ForeignKey("Designation_ID")]
        public designationEntity DesignationEntity { get; set; }

        /* Other Connected Tables */

        public virtual emergencyContactEntity EmergencyContactEntity { get; set; }
        public virtual govIdsEntity GovIdsEntity { get; set; }
        public virtual ICollection<cardDetailsEntity> CardDetailsEntity { get; set; }
        public virtual userEntity UserEntity { get; set; }
    }
}