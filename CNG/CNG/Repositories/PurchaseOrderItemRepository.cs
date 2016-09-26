using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class PurchaseOrderItemRepository
    {
        private CNGDBContext context;

        public PurchaseOrderItemRepository(CNGDBContext _context) {
            this.context = _context;
        }

        public PurchaseOrderItem Find(int poItemId) {
            PurchaseOrderItem item = context.PurchaseOrderItems.Find(poItemId);

            return item;
        }

        public void Save(PurchaseOrderItem poItem)
        {
            context.PurchaseOrderItems.Add(poItem);

            context.SaveChanges();
        }
    }
}