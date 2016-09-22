using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class PurchaseOrderItemRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public void Save(PurchaseOrderItem poItem)
        {
            context.PurchaseOrderItems.Add(poItem);

            context.SaveChanges();
        }
    }
}