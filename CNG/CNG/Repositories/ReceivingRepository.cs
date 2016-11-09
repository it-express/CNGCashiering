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

        public void Save(Receiving receiving)
        {
            if (receiving.Id == 0)
            {
                context.Receivings.Add(receiving);
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
                }
            }

            context.SaveChanges();
        }
    }
}