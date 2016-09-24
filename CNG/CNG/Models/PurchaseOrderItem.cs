using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class PurchaseOrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PurchaseOrderNo { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public decimal UnitCost { get; set; }

        [Required]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }

        [StringLength(200)]
        public string SerialNo { get; set; }

        public int ReceivedQuantity { get; set; }

        public string DrNo { get; set; }

        public DateTime Date { get; set; }

        public virtual Item Item { get; set; }

        public decimal Amount {
            get {
                return UnitCost * Quantity;
            }
        }

        public decimal Balance {
            get {
                return Quantity - ReceivedQuantity;
            }
        }
    }
}