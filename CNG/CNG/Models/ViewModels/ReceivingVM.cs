using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ReceivingVM
    {
        public PurchaseOrder PurchaseOrder { get; set; }
        public Receiving Receiving { get; set; }
    }
}