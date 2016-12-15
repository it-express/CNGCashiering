using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ReceivingDTO
    {
        public class Item
        {
            public int PoItemId { get; set; }
            public string SerialNo { get; set; }
            public int ReceivedQuantity { get; set; }
            public string DrNo { get; set; }
            public DateTime Date { get; set; }

            public DateTime? RemainingBalanceDate { get; set; }
        }

        public string PoNo { get; set; }
        public string RRNo { get; set; }
        public int Status { get; set; }

        public List<Item> Items { get; set; }
    }
}