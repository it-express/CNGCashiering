﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ExcessPartsSetDTO
    {
        public class Item
        {
            public int ItemId { get; set; }
            public decimal UnitCost { get; set; }
            public int Quantity { get; set; }
            public string Remarks { get; set; }
        }

        public string No { get; set; }
        public DateTime Date { get; set; }

        public string CheckedBy { get; set; }

        public List<Item> Items { get; set; }
    }
}