using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class VehicleStockTransferItemVM
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int VehicleFromId { get; set; }
        public int VehicleToId { get; set; }
        public string VehicleFromPlateNo { get; set; }
        public string VehicleToPlateNo { get; set; }
        public string Remarks { get; set; }
    }
}