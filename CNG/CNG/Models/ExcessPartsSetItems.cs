using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ExcessPartsSetItems
    {
        public int Id { get; set; }
        public string ExcessPartsSetNo { get; set; }
        public int ItemId { get; set; }
        public decimal Money { get; set; }
        public string Remarks { get; set; }
    }
}