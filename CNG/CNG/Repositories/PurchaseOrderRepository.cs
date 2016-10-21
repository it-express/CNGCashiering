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

        public PurchaseOrder GetByNo(string poNo) {
            PurchaseOrder po = context.PurchaseOrders.FirstOrDefault(p => p.No == poNo);

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
            }
            else
            {
                PurchaseOrder dbEntry = context.PurchaseOrders.FirstOrDefault(p => p.No == po.No);
                if (dbEntry != null)
                {
                    dbEntry.No = po.No;

                    dbEntry.Date = DateTime.Now;
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
            }

            context.SaveChanges();
        }

        public string GeneratePoNumber()
        {
            int companyId = Convert.ToInt32(HttpContext.Current.Request.QueryString["companyId"]);
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

        public IQueryable<PurchaseOrder> ListReceived() {
            IQueryable<PurchaseOrder> lst = List().Where(p =>
            p.Status != (int) EPurchaseOrderStatus.Open);

            return lst;
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