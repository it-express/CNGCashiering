﻿using System;
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

        public IQueryable<PurchaseOrder> ListForReceiving() {
            return context.PurchaseOrders.Where(p => p.Status == (int) EPurchaseOrderStatus.Open);
        }

        public PurchaseOrder GetByNo(string poNo) {
            PurchaseOrder po = context.PurchaseOrders.FirstOrDefault(p => p.No == poNo);

            return po;
        }

        public string GeneratePoNumber()
        {
            //get last id
            int lastId = 0;
            if (List().Count() > 0) {
                lastId = List().Max(p => p.Id);
            }

            //MMyy-series
            string poNumber = DateTime.Now.ToString("MMyy") + "-" + (lastId + 1).ToString().PadLeft(4, '0');

            return poNumber;
        }

        public List<PurchaseOrder> ListReceived() {
            IQueryable<PurchaseOrder> lst = List().Where(p =>
            p.Status != (int) EPurchaseOrderStatus.Open);

            return lst.ToList();
        }

        public void ChangeStatus(string poNo, int status) {
            PurchaseOrder po = context.PurchaseOrders.FirstOrDefault(p => p.No == poNo);
            po.Status = status;

            context.SaveChanges();
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