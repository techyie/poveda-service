using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCampusV2.Models
{
    public class IBaseEntity
    {
        public IBaseEntity(){
            //this.IsActive = true;
        }

        public DateTime Date_Time_Added { get; set; }
        public int Added_By { get; set; }
        public DateTime Last_Updated { get; set; }
        public int Updated_By { get; set; }
        public bool IsActive { get; set; }
        public bool ToDisplay { get; set; }
    }
}