using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DisplayName("Unit Cost")]
        public decimal UnitCost { get; set; }
        
        public string GetUnitCost
        { get { return UnitCost.ToString("#,##0.00"); } }
    
        public int Quantity { get; set; }
        public string Remarks { get; set; }

        public decimal Amount {
            get {
                return UnitCost * Quantity;
            }
        }

        public string GetAmount
        { get { return Amount.ToString("#,##0.00"); } }

        public virtual RequisitionPurchase RequisitionPurchase { get; set; }
        public virtual Item Item { get; set; }
    }
}