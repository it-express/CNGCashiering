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

        ItemAssignmentRepository itemAssignmentRepo = new ItemAssignmentRepository();

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

        public ItemAssignment GetByItemId(int id, int? companyid)
        {
            ItemAssignment item = context.ItemAssignments.FirstOrDefault(p => p.ItemId == id && p.CompanyId == companyid);

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
                Item dbEntry = context.Items.Find(item.Id);
                

                if (dbEntry != null)
                {
                    // update Items table
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

        public string SaveItem(Item item)
        {
            string msg = "";
            if (item.Id == 0)
            {
                IQueryable<Item> lstItem = List();
                IQueryable<ItemAssignment> lstItemAssign = itemAssignmentRepo.List();

                lstItem = lstItem.Where(s => s.Description == item.Description.Trim());
                lstItemAssign = lstItemAssign.Where(s => s.ItemId == item.Id && s.CompanyId == Sessions.CompanyId);

                if (lstItem.Count() == 0 || lstItemAssign.Count() == 0)
                {
                    context.Items.Add(item);
                    context.SaveChanges();

                    int itemid = item.Id;

                    ItemAssignment itemAssign = new ItemAssignment();
                    itemAssign.ItemId = itemid;
                    itemAssign.CompanyId =  Sessions.CompanyId.Value;
                    itemAssign.UnitCost = item.UnitCost;

                    context.ItemAssignments.Add(itemAssign);
                    msg = "save";
                }
                else
                {
                    msg = "not save";
                }

            }
            else
            {
                Item dbEntry = context.Items.Find(item.Id);
                int assignid  = context.ItemAssignments.FirstOrDefault(p => p.CompanyId == Sessions.CompanyId && p.ItemId == item.Id).Id;

                ItemAssignment dbEntry1 = context.ItemAssignments.Find(assignid);

                if (dbEntry != null || dbEntry1!= null)
                {
                    // update Items table
                    dbEntry.Description = item.Description;
                    dbEntry.Brand = item.Brand;
                    dbEntry.UnitCost =0;
                    dbEntry.TypeId = item.TypeId;
                    dbEntry.ClassificationId = item.ClassificationId;
                    dbEntry.Active = item.Active;

                    //
                    dbEntry1.UnitCost = item.UnitCost;

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
                int itemid = lstItem.Where(s => s.Description == item.Description.Trim()).Select(p => p.Id).Distinct().FirstOrDefault();

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

        public int GetItemId(string itemdesc)
        {
            IQueryable<Item> lstItem = List();
            
            return lstItem.Where(s => s.Description == itemdesc.Trim()).Select(p => p.Id).Distinct().FirstOrDefault();
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