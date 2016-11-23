using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class TransactionMethod
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public enum ETransactionMethod
    {
        Receiving = 1,
        RequisitionToPurchase = 2,
        ExcessPartsFromSet = 3,
        Requisition = 4,
        StockTransfer = 5
    }
}