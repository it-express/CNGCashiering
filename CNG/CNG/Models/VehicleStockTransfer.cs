using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class VehicleStockTransfer
    {
        public VehicleStockTransfer() {
            VehicleStockTransferItems = new List<VehicleStockTransferItem>();
        }

        [Key]
        public int Id { get; set; }
        public string No { get; set; }
        public DateTime Date { get; set; }

        public int CompanyId { get; set; }

        public string RequestedBy { get; set; }
        public int CheckedBy { get; set; }
        public int ApprovedBy { get; set; }
        public bool Checked { get; set; }
        public bool Approved { get; set; }

        public virtual List<VehicleStockTransferItem> VehicleStockTransferItems { get; set; }

        [ForeignKey("CheckedBy")]
        public virtual User CheckedByObj { get; set; }
        [ForeignKey("ApprovedBy")]
        public virtual User ApprovedByObj { get; set; }
    }
}