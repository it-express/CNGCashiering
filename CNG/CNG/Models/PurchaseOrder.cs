using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class PurchaseOrder
    {
        public PurchaseOrder() {
            PurchaseOrderItems = new List<PurchaseOrderItem>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string No { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [DisplayName("Vendor")]
        public int VendorId { get; set; }

        [Required]
        [ForeignKey("ShipToCompany")]
        [DisplayName("Ship To")]
        public int ShipTo { get; set; }

        [Required]
        public int Terms { get; set; }

        [Required]
        public int PreparedBy { get; set; }

        [Required]
        public int ApprovedBy { get; set; }

        public int Status { get; set; }

        public virtual Vendor Vendor { get; set; }
        [ForeignKey("ShipTo")]
        public virtual Company ShipToCompany { get; set; }

        public virtual List<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    }

    public enum EPurchaseOrderStatus
    {
        Open = 0,
        Saved = 1,
        Submitted = 2
    }
}