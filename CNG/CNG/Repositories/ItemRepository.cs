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

        public IQueryable<ItemAssignment> ListItemAssignments()
        {
            return context.ItemAssignments;
        }

        public Item GetById(int id)
        {
            Item item = context.Items.FirstOrDefault(p => p.Id == id);

            return item;
        }

        public string GeneratedItemCode()
        {
            int counter = 1;
            int lastId = 1;
            int cnt = List().Count();
            if (cnt > 0)
            {
                lastId = cnt + counter;
            }
            string ItemCode = lastId.ToString().PadLeft(3, '0');

            IQueryable<Item> lstItem = List();
            lstItem = lstItem.Where(s => s.Code.Contains(ItemCode));

            while (lstItem.Count()> 0)
            {
                counter += 1;
                lastId = cnt + counter;
                ItemCode = lastId.ToString().PadLeft(3, '0');
                lstItem = lstItem.Where(s => s.Code.Contains(ItemCode));
            }
            

            return ItemCode;
        }


        public  string Save(Item item)
        {
            string msg = "";
            if (item.Id == 0)
            {
                IQueryable<Item> lstItem = List();
                lstItem = lstItem.Where(s => s.Description == item.Description.Trim());

                if (lstItem.Count() == 0)
                {
                    context.Items.Add(item);
                    msg = "save";
                }
                else
                {
                    msg = "not save";
                }

            }
            else
            {
                Item dbEntry = context.Items.Find(item.Description);
                

                if (dbEntry != null)
                {
                    // update Items table
                    dbEntry.Code = GeneratedItemCode();
                    dbEntry.Description = item.Description;
                    dbEntry.Brand = item.Brand;
                    dbEntry.UnitCost = item.UnitCost;
                    dbEntry.TypeId = item.TypeId;
                    dbEntry.ClassificationId = item.ClassificationId;
                    dbEntry.Active = item.Active;

                    msg = "updated";
                }
              
            }

            context.SaveChanges();

            return msg;
        }

        public int SaveByEncoder(Item item)
        {
            int msg = 0;
           
                IQueryable<Item> lstItem = List();
                lstItem = lstItem.Where(s => s.Description == item.Description.Trim());
                int itemid = lstItem.Select(p => p.Id).Distinct().FirstOrDefault();

                if (lstItem.Count() == 0)
                {
                item.Code = GeneratedItemCode();
                    context.Items.Add(item);
                    context.SaveChanges();

                    msg = item.Id;
                }
               

            else
            { 
            
                Item dbEntry = context.Items.Find(itemid);


                if (dbEntry != null)
                {
                    msg = dbEntry.Id;
                }

            }

            context.SaveChanges();

            return msg;
        }

        public string Delete(int id)
        {
            string msg = "";
            IQueryable<ItemAssignment> lstitemAssign = ListItemAssignments();
            lstitemAssign = lstitemAssign.Where(p => p.CompanyId != Sessions.CompanyId && p.ItemId == id);

            if (lstitemAssign.Count() > 0)
            {
                msg = "not save";
            }
            else
            {
                Item item = context.Items.Find(id);

                context.Items.Remove(item);

                context.SaveChanges();
                msg = "save";
            }

            return msg;
        }

        //public void AdjustQuantity(int itemId, int quantity) {
        //    Item item = context.Items.Find(itemId);
        //    item.QuantityOnHand += quantity;

        //    Save(item);
        //}
    }
}