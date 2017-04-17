﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class PurchaseOrderRepository
    {
        private CNGDBContext context = new CNGDBContext();
        VendorRepository vendorRepo = new VendorRepository();
        CompanyRepository companyRepo = new CompanyRepository();

        public IQueryable<PurchaseOrder> List() {
            return context.PurchaseOrders;
        }

        public IQueryable<PurchaseOrder> ListForReceiving() {
            return context.PurchaseOrders.Where(p => p.Status == (int) EPurchaseOrderStatus.Open);
        }

        public PurchaseOrder Find(int poId) {
            PurchaseOrder po = List().FirstOrDefault(p => p.Id == poId);

            return po;
        }

        public PurchaseOrder GetByNo(string poNo) {
            PurchaseOrder po = context.PurchaseOrders.FirstOrDefault(p => p.No == poNo);

            return po;
        }

        public PurchaseOrder GetById(int poId)
        {
            PurchaseOrder po = context.PurchaseOrders.FirstOrDefault(p => p.Id == poId);

            return po;
        }

        public void Save(PurchaseOrder po) {
            bool poExist = context.PurchaseOrders.Count(p => p.No == po.No) > 0;

            int id;

            if (!poExist)
            {
                po.ShipToCompany = null;
                po.Vendor = null;
                context.PurchaseOrders.Add(po);
              
                context.SaveChanges();


                id = po.Id;

                foreach (ItemPriceLogs poItem in po.ItemPriceLogs.ToList())
                {
                    poItem.PurchaseOrderId = id;
                    InsertStockCard(id, poItem.ItemId, poItem.UnitCost, poItem.Qty);
                }


            }
            else
            {
                PurchaseOrder dbEntry = context.PurchaseOrders.FirstOrDefault(p => p.No == po.No);
                if (dbEntry != null)
                {
                    dbEntry.No = po.No;

                    dbEntry.Date = po.Date;
                    dbEntry.VendorId = po.VendorId;
                    dbEntry.ShipTo = po.ShipTo;
                    dbEntry.Terms = vendorRepo.GetById(po.VendorId).Terms;

                    dbEntry.PreparedBy = Common.GetCurrentUser.Id;
                    dbEntry.ApprovedBy = Common.GetCurrentUser.GeneralManagerId;
                    dbEntry.CheckedBy = po.CheckedBy;
                }

                id = dbEntry.Id;

                //Delete previous items
                foreach (PurchaseOrderItem poItem in dbEntry.PurchaseOrderItems.ToList())
                {
                    context.PurchaseOrderItems.Remove(poItem);
                }

                foreach (PurchaseOrderItem poItem in po.PurchaseOrderItems.ToList())
                {
                    poItem.PurchaseOrderId = id;

                    context.PurchaseOrderItems.Add(poItem);
                }

                //Delete previous items
                foreach (ItemPriceLogs poItem in dbEntry.ItemPriceLogs.ToList())
                {
                    StockCard sc = context.StockCards.FirstOrDefault(p => p.ReferenceId==poItem.PurchaseOrderId);
                    context.StockCards.Remove(sc);
                    context.ItemPriceLogs.Remove(poItem);
                }

                foreach (ItemPriceLogs poItem in po.ItemPriceLogs.ToList())
                {
                    poItem.PurchaseOrderId = id;

                    InsertStockCard(id, poItem.ItemId, poItem.UnitCost, poItem.Qty);
                    context.ItemPriceLogs.Add(poItem);
                }
            }

            context.SaveChanges();
        }

        //For Stock Card

        public void InsertStockCard(int ReferenceId, int itemId, decimal unitcost, int quantiy)
        {
            ItemStockCardRepository stockcardRepo = new ItemStockCardRepository();

            StockCard stockCard = new StockCard
            {
                ItemId = itemId,
                ReferenceModule = "Purchase Order",
                ReferenceId = ReferenceId,
                Qty = quantiy,
                UnitCost = unitcost,
                CompanyId = Sessions.CompanyId.Value,
                Date = DateTime.Now
            };


            context.StockCards.Add(stockCard);


        }

        public string GeneratePoNumber()
        {
            int companyId = Sessions.CompanyId.Value;
            //get last id
            int lastId = 1;
            int cnt = List().Where(p => p.ShipTo == companyId).Count();
            if (cnt > 0) {
                lastId = cnt + 1;
            }

            string prefix = companyRepo.GetById(companyId).Prefix;

            //MMyy-series
            string poNumber = prefix + "-" + DateTime.Now.ToString("MMyy") + "-" + lastId.ToString().PadLeft(4, '0');

            return poNumber;
        }

        public string GenerateReNumber()
        {
            int companyId = Sessions.CompanyId.Value;
            //get last id
            int lastId = 1;
            int cnt = List().Where(p => p.ShipTo == companyId && p.Status > 0).Count();
            if (cnt > 0)
            {
                lastId = cnt + 1;
            }

            string prefix = companyRepo.GetById(companyId).Prefix;

            //MMyy-series
            string reNumber = prefix + "-" + DateTime.Now.ToString("MMyy") + "-" + lastId.ToString().PadLeft(4, '0');

            return reNumber;
        }

        public IQueryable<PurchaseOrder> ListReceived() {
            IQueryable<PurchaseOrder> lst = List().Where(p =>
            p.Status != (int) EPurchaseOrderStatus.Open);

            return lst;
        }

        public void ChangeStatus(string poNo, int status, string RRNo)
        {
            PurchaseOrder po = context.PurchaseOrders.FirstOrDefault(p => p.No == poNo);
            po.Status = status;
            po.RRNo = RRNo;

            context.SaveChanges();

            foreach (PurchaseOrderItem Item in po.PurchaseOrderItems.ToList())
            {
                UpdateItemPriceLogs(Item.ItemId, Item.Quantity);
            }
        }

        public void UpdateItemPriceLogs(int itemid, int quantity)
        {
            int diff = 0;
            ItemPriceLogs itempricelogs = context.ItemPriceLogs.Where(p => p.ItemId == itemid && p.Qty > 0).First();


            diff = quantity;
            while (diff > itempricelogs.Qty)
            {
                diff -= itempricelogs.Qty;

                ItemPriceLogs update = context.ItemPriceLogs.Find(itempricelogs.Id);

                update.Qty = 0;
                context.SaveChanges();
                itempricelogs = context.ItemPriceLogs.Where(p => p.ItemId == itemid && p.Qty > 0).First();
            }

            if (diff < itempricelogs.Qty && diff != 0)
            {
                itempricelogs = context.ItemPriceLogs.Where(p => p.ItemId == itemid && p.Qty > 0).First();

                itempricelogs.Qty -= diff;

                ItemPriceLogs update = context.ItemPriceLogs.Find(itempricelogs.Id);

                update.Qty = itempricelogs.Qty;
                context.SaveChanges();

            }
        }

        public void Delete(string poNo) {
            PurchaseOrder po = context.PurchaseOrders.FirstOrDefault(p => p.No == poNo);

            context.PurchaseOrders.Remove(po);

            context.SaveChanges();
        }
    }

    public enum EReceivingStatus {
        NotReceived = 0,
        Received = 1
    }
}