﻿using System;
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

        [ForeignKey("PreparedBy")]
        public virtual User PreparedByObj { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual User ApprovedByObj { get; set; }

        public string StatusDescription {
            get {
                string description = "";
                switch (Status) {
                    case (int)EPurchaseOrderStatus.Open:
                        description = "Open";
                        break;
                    case (int)EPurchaseOrderStatus.Saved:
                        description = "Saved";
                        break;
                    case (int)EPurchaseOrderStatus.Submitted:
                        description = "Submitted";
                        break;
                }

                return description;
            }
        }
    }

    public enum EPurchaseOrderStatus
    {
        Open = 0,
        Saved = 1,
        Submitted = 2
    }
}