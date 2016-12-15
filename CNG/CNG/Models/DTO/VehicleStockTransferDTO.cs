using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class VehicleStockTransferDTO
    {
        public class Item
        {
            public int Id { get; set; }
            public int VehicleFromId { get; set; }
            public int VehicleToId { get; set; }
            public int Quantity { get; set; }
            public string Remarks { get; set; }
        }

        public string No { get; set; }
        public string Date { get; set; }
       
        public string RequestedBy { get; set; }

        public List<Item> Items { get; set; }
    }
}