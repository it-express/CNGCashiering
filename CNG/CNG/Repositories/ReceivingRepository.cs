﻿using CNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG
{
    public class ReceivingRepository
    {
        private CNGDBContext context = new CNGDBContext();
        CompanyRepository companyRepo = new CompanyRepository();
  
        public IQueryable<Receiving> List()
        {
            return context.Receivings;
        }

        public Receiving GetById(int id)
        {
            Receiving item = context.Receivings.FirstOrDefault(p => p.Id == id);

            return item;
        }

        public IEnumerable<Receiving> ListByPurchaseOrderItemId(int poItemId) {
            IEnumerable<Receiving> lstItem = context.Receivings.Where(p => p.PurchaseOrderItemId == poItemId);

            return lstItem;
        }


        public void Save(Receiving receiving, int itemid, string unitcost)
        {
            if (receiving.Id == 0)
            {
                context.Receivings.Add(receiving);

                context.SaveChanges();
                int id;
                id = receiving.Id;

                InsertStockCard(id, itemid, Convert.ToDecimal(unitcost), receiving.Quantity);
                UpdateItemPriceLogs(itemid, receiving.Quantity);
            }
            else
            {
                Receiving dbEntry = context.Receivings.Find(receiving.Id);
                if (dbEntry != null)
                {
                    dbEntry.PurchaseOrderItemId = receiving.PurchaseOrderItemId;
                    dbEntry.Quantity = receiving.Quantity;
                    dbEntry.SerialNo = receiving.SerialNo;
                    dbEntry.DrNo = receiving.DrNo;
                    dbEntry.DateReceived = receiving.DateReceived;
                    dbEntry.TransactionLogId = receiving.TransactionLogId;

                    context.SaveChanges();

                    TransactionLogRepository transLogRepo = new TransactionLogRepository();
                    transLogRepo.Update(dbEntry.TransactionLogId.Value, receiving.Quantity, receiving.DateReceived.Value);

                }
            }

            context.SaveChanges();
        }

        public void InsertStockCard(int ReferenceId, int itemId, decimal unitcost, int quantiy)
        {
            ItemStockCardRepository stockcardRepo = new ItemStockCardRepository();

            StockCard stockCard = new StockCard
            {
                ItemId = itemId,
                ReferenceModule = "Receiving",
                ReferenceId = ReferenceId,
                Qty = quantiy,
                UnitCost = unitcost,
                CompanyId = Sessions.CompanyId.Value,
                Date = DateTime.Now
            };


            stockcardRepo.Add(stockCard);
        }

        public void UpdateItemPriceLogs(int itemid, int quantity)
        {
            int diff = 0;
            ItemPriceLogs itempricelogs = context.ItemPriceLogs.Where(p => p.ItemId == itemid && p.Qty>0).First();

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
    }
}