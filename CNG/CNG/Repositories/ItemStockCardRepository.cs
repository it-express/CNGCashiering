using CNG.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ItemStockCardRepository
    {
        private CNGDBContext context = new CNGDBContext();
        public ItemRepository itemRepo = new ItemRepository();

        public IQueryable<StockCard> List()
        {
            return context.StockCards;
        }

        public void Add(StockCard stockcard)
        {
            if (stockcard.Date == DateTime.MinValue)
            {
                stockcard.Date = DateTime.Now;
            }

            stockcard.CompanyId = stockcard.CompanyId;

            context.StockCards.Add(stockcard);
            context.SaveChanges();
            
        }

        public void Update(StockCard stockcard)
        {
            StockCard st = List().FirstOrDefault(p => p.TransLogId == stockcard.TransLogId);

            StockCard sc = context.StockCards.Find(st.Id);
            context.SaveChanges();
        }

      
        public void Remove(int id)
        {
            StockCard stockCard = List().FirstOrDefault(p => p.Id == id);

            context.StockCards.Remove(stockCard);

            context.SaveChanges();
        }

    }

}