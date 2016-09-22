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

        public List<PurchaseOrderItem> ListByPoNo(string poNo) {
            IQueryable<PurchaseOrderItem> lstPoItem = context.PurchaseOrderItems.Where(p => p.PurchaseOrderNo == poNo);

            return lstPoItem.ToList();
        }
    }
}