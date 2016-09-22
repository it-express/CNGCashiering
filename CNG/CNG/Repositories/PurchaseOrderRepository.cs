using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class PurchaseOrderRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<PurchaseOrder> List() {
            return context.PurchaseOrders;
        }

        public string GeneratePoNumber()
        {
            //get last id
            int lastId = 0;
            if (List().Count() > 0) {
                lastId = List().Max(p => p.Id);
            }

            //mmyy-series
            string poNumber = DateTime.Now.ToString("mmyy") + "-" + (lastId + 1).ToString().PadLeft(4, '0');

            return poNumber;
        }
    }
}