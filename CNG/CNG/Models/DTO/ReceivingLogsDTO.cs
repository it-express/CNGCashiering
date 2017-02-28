using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ReceivingLogsDTO
    {
        public class Item
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
            public string SerialNo { get; set; }
            public string DrNo { get; set; }       
            public DateTime DateReceived { get; set; }
            public int TransLogId { get; set; }
        }

        public int Id { get; set; }
        public int PurchaseOrderItemId { get; set; }
        public string RRNo { get; set; }
        public List<Item> Items { get; set; }
    }
}