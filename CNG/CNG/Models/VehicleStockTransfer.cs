using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class VehicleStockTransfer
    {
        public VehicleStockTransfer() {
            VehicleStockTransferItems = new List<VehicleStockTransferItem>();
        }

        public int Id { get; set; }
        public string No { get; set; }
        public DateTime Date { get; set; }

        public string RequestedBy { get; set; }
        public int CheckedBy { get; set; }
        public int ApprovedBy { get; set; }

        public List<VehicleStockTransferItem> VehicleStockTransferItems { get; set; }
    }
}