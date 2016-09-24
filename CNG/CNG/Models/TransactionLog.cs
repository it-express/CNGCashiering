using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class TransactionLog
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public int TransactionMethodId { get; set; }
        public DateTime Date { get; set; }

        public virtual TransactionMethod TransactionMethod { get; set; }
    }
}