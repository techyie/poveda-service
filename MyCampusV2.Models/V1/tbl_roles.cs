using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    //[Table("tbl_roles")]
    public class tbl_roles : IBaseEntity
    {
        [Key]
        public int ID { get; set; }
        public string Role_Name { get; set; }
        public string Role_Desc { get; set; }
        //public tbl_roles()
        //{
        //    tbl_user_roles = new HashSet<tbl_user_role>();
   
        //}
        //public virtual ICollection<tbl_user_role> tbl_user_roles { get; set; }
    }
}
