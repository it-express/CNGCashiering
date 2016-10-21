using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class PurchaseOrderVM
    {
        public PurchaseOrder PurchaseOrder { get; set; }
        public Company SelectedCompany { get; set; }
    }
}