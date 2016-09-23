using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class RequisitionPurchaseItems
    {
        public int Id { get; set; }
        public string RequisitionNo { get; set; }
        public int ItemId { get; set; }
        public decimal UnitCost { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }

        public virtual RequisitionPurchase Requisition { get; set; }
        public virtual Item Item { get; set; }
    }
}