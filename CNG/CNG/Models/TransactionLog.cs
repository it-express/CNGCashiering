﻿using System;
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
        public int UserId { get; set; }

        public int CumulativeQuantity { get; set; }

        public int CompanyId { get; set; }
        public string Method {
            get {
                if (Quantity >= 0)
                {
                    return "In";
                }
                else {
                    return "Out";
                }
            }
        }

        public int? ItemTypeId { get; set; }

        public virtual TransactionMethod TransactionMethod { get; set; }
        public virtual User User { get; set; }
        public virtual Item Item { get; set; }
        public virtual Company Company { get; set; }
    }
}