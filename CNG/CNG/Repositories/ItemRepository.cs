using CNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ItemRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<Item> List()
        {
            return context.Items;
        }

        public Item GetById(int id)
        {
            Item item = context.Items.FirstOrDefault(p => p.Id == id);

            return item;
        }

        public void Save(Item item)
        {
            if (item.Id == 0)
            {
                context.Items.Add(item);
            }
            else
            {
                Item dbEntry = context.Items.Find(item.Id);
                if (dbEntry != null)
                {
                    dbEntry.Code = item.Code;
                    dbEntry.Description = item.Description;
                    dbEntry.Brand = item.Brand;
                    dbEntry.UnitCost = item.UnitCost;
                    dbEntry.TypeId = item.TypeId;
                    dbEntry.ClassificationId = item.ClassificationId;
                    dbEntry.Active = item.Active;
                }
            }

            context.SaveChanges();
        }

        public void Delete(int id)
        {
            Item item = context.Items.Find(id);

            context.Items.Remove(item);

            context.SaveChanges();
        }

        //public void AdjustQuantity(int itemId, int quantity) {
        //    Item item = context.Items.Find(itemId);
        //    item.QuantityOnHand += quantity;

        //    Save(item);
        //}
    }
}