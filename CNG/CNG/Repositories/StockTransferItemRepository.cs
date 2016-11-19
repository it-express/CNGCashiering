using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class StockTransferItemRepository
    {
        private CNGDBContext context;

        public StockTransferItemRepository(CNGDBContext _context)
        {
            this.context = _context;
        }

        public StockTransferItem Find(int stItemId)
        {
            StockTransferItem item = context.StockTransferItems.Find(stItemId);

            return item;
        }

        public void Save(StockTransferItem rpItem)
        {
            context.StockTransferItems.Add(rpItem);

            context.SaveChanges();
        }

        public List<StockTransferItem> ListByStId(int id)
        {
            IQueryable<StockTransferItem> lstStItem = context.StockTransferItems.Where(p => p.StockTransferId == id);

            return lstStItem.ToList();
        }
    }
}