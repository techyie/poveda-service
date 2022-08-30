using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyCampusV2.Models
{
    [Table("tbl_failedlogs")]
    public class tblref_failedlogs
    {
        [Key]
        public int id { get; set; }
        public int fld_id { get; set; }
        public string fld_msg { get; set; }
    }
}
