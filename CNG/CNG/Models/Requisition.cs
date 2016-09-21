using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CNG.Models
{
    public class Requisition
    {
        [Key]
        public string No { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string JobOrderNo { get; set; }

        [Required]
        [StringLength(100)]
        public string UnitPlateNo { get; set; }

        [Required]
        [StringLength(100)]
        public string OdometerReader { get; set; }

        [Required]
        [StringLength(100)]
        public string DriverName { get; set; }

        [Required]
        public int ReportedBy { get; set; }

        public int CheckedBy { get; set; }

        public int ApprovedBy { get; set; }
    }
}