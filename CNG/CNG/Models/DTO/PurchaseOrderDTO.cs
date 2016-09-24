using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class PurchaseOrderDTO
    {
        public class Item
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
            public string Remarks { get; set; }
        }

        public string No { get; set; }
        public int VendorId { get; set; }
        public int ShipTo { get; set; }

        public List<Item> Items { get; set; }
    }
}