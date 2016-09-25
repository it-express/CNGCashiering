using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class RequisitionPurchaseItem
    {
        [Key]
        public int Id { get; set; }
        public int RequisitionPurchaseId { get; set; }
        public int ItemId { get; set; }
        public decimal UnitCost { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }

        public virtual RequisitionPurchase RequisitionPurchase { get; set; }
        public virtual Item Item { get; set; }
    }
}