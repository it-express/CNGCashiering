using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class RequisitionDTO
    {
        public class Item
        {
            public int ItemId { get; set; }
            public int Quantity { get; set; }
            public string SerialNo { get; set; }
            public int Type { get; set; }
        }

        public DateTime RequisitionDate { get; set; }
        public string JobOrderNo { get; set; }
        public string UnitPlateNo { get; set; }
        public DateTime JobOrderDate { get; set; }
        public string OdometerReading { get; set; }
        public string DriverName { get; set; }

        public List<Item> Items { get; set; }
    }
}