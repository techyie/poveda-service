using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Models
{
    public class CardEntity
    {
        public string IDNumber { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string CardStatus { get; set; }
        public string Remarks { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DateUpdated { get; set; }
    }

    public class CardVM
    {
        public string IDNumber { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public string IssuedDate { get; set; }
        public string ExpiryDate { get; set; }
        public string CardStatus { get; set; }
        public string Remarks { get; set; }
        public string UpdatedBy { get; set; }
        public string DateUpdated { get; set; }
    }

    public class cardFilterVM
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string filter { get; set; }
        public string cardStatus { get; set; }
        public string personType { get; set; }
    }
}
