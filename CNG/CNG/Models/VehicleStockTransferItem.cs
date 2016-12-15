using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class VehicleStockTransferItem
    {

        [Key]
        public int Id { get; set; }
        public int VehicleStockTransferId { get; set; }
        public int ItemId { get; set; }
        public int VehicleFromId { get; set; }
        public int VehicleToId { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }

        public int? TransactionLogId { get; set; }

        public virtual VehicleStockTransfer VehicleStockTransfer { get; set; }
        public virtual Item Item { get; set; }

        public virtual Vehicle VehicleFrom { get; set; }
        public virtual Vehicle VehicleTo { get; set; }

        public virtual TransactionLog TransactionLog { get; set; }
    }
}