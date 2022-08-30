using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class cardBatchDeactiveVM
    {
        [Required]
        public string IdNumber { get; set; }
    }
}
