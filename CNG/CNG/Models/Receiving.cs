using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class Receiving
    {
        PurchaseOrderRepository poItemRepo = new PurchaseOrderRepository();

        public int Id { get; set; }
        public int PurchaseOrderItemId { get; set; }
        public int Quantity { get; set; }
        public string SerialNo { get; set; }
        public string DrNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateReceived { get; set; }

        public int? TransactionLogId { get; set; }

        public virtual TransactionLog TransactionLog { get; set; }
        public virtual PurchaseOrderItem PurchaseOrderItem { get; set; }

        public int GetPOItemQuantity(int PoItemID)
        {
            return poItemRepo.GetQuantity(PoItemID);   
        }


    }
}