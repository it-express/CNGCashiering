using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class StockTransferItem
    {
        [Key]
        public int Id { get; set; }
        public int StockTransferId { get; set; }
        public int ItemId { get; set; }
        [DisplayName("Unit Cost")]
        public decimal UnitCost { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }

        public decimal Amount
        {
            get
            {
                return UnitCost * Quantity;
            }
        }

        public virtual StockTransfer StockTransfer { get; set; }
        public virtual Item Item { get; set; }
    }
}