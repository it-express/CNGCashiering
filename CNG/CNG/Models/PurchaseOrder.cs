using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class PurchaseOrder
    {
        [Required]
        public string No { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int VendorId { get; set; }

        [Required]
        public int ShipTo { get; set; }

        [Required]
        public string Terms { get; set; }

        [Required]
        public int PreparedBy { get; set; }

        [Required]
        public int ApprovedBy { get; set; }
    }
}