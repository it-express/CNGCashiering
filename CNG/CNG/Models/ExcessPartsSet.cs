using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ExcessPartsSet
    {
        [Key]
        public string No { get; set; }
        public DateTime Date { get; set; }
        public int PreparedBy { get; set; }
        public int ApprovedBy { get; set; }
    }
}