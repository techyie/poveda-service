using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Models
{
    public class tbl_terminal_category
    {
        public int ID { get; set; }
        public string Terminal_Category_Code { get; set; }
        public string Terminal_Category_Name { get; set; }

        //public virtual ICollection<tbl_terminal> tbl_terminal { get; set; }
    }
}
