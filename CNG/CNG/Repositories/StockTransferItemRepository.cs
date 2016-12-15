using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class StockTransferItemRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public StockTransferItemRepository()
        {
        }
        public IQueryable<StockTransferItem> List()
        {
            return context.StockTransferItems;
        }
        public StockTransferItem Find(int stItemId)
        {
            StockTransferItem item = context.StockTransferItems.Find(stItemId);

            return item;
        }

        public void Remove(int stItemId)
        {
            StockTransferItem item = context.StockTransferItems.Find(stItemId);
            context.StockTransferItems.Remove(item);

            context.SaveChanges();
        }

        public void Save(StockTransferItem stItem)
        {
            context.StockTransferItems.Add(stItem);

            context.SaveChanges();
        }
    }
}