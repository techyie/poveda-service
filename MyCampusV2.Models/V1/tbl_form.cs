using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_form")]
    public class tbl_form
    {
        [Key]
        [Column("Form_ID")]
        public int Form_ID { get; set; }

        [Required]
        [Column("Form_Name")]
        [StringLength(300)]
        public string Form_Name { get; set; }

        [Column("Form_Description")]
        [StringLength(300)]
        public string Form_Description { get; set; }

        [Required]
        [Column("Form_Code")]
        [StringLength(125)]
        public string Form_Code { get; set; }

        [Required]
        [Column("Url")]
        [StringLength(3000)]
        public string Url { get; set; }

        [Column("Icon")]
        [StringLength(3000)]
        public string Icon { get; set; }

        public bool? isTitle { get; set; }

        public int? Parent_ID { get; set; }

        public bool Searchable { get; set; }

        public bool Administrator { get; set; }
        public bool AdminRights { get; set; }

        //[NotMapped]
        //public List<tbl_form> children = new List<tbl_form>();

        public tbl_form()
         {
        //    tbl_formsub = new HashSet<tbl_formsub>();
            //user_role_access = new HashSet<tbl_user_role_access>();
         }

        //[Key]
        //public int Form_ID { get; set; }
        //public string Form_Name { get; set; }
        //public string Form_Description { get; set; }
        //public string Form_Code { get; set; }
        //public string url { get; set; }
        //public string icon { get; set; }
        ////public int Parent_ID { get; set; }
        //public ICollection<tbl_formsub> tbl_formsub { get; set; }
        //public virtual ICollection<tbl_user_access> user_role_access { get; set; }

    }
}
