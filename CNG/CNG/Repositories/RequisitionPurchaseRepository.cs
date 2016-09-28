using CNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class RequisitionPurchaseRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<RequisitionPurchase> List()
        {
            return context.RequisitionPurchases;
        }

        public RequisitionPurchase GetById(int id) {
            RequisitionPurchase rp = context.RequisitionPurchases.FirstOrDefault(p => p.Id == id);

            return rp;
        }

        public RequisitionPurchase GetByRpNo(string rpNo) {
            RequisitionPurchase rp = context.RequisitionPurchases.FirstOrDefault(p => p.No == rpNo);

            return rp;
        }

        public void Delete(string rpNo) {
            RequisitionPurchase rp = context.RequisitionPurchases.FirstOrDefault(p => p.No == rpNo);

            context.RequisitionPurchases.Remove(rp);

            context.SaveChanges();
        }

        public string GenerateRpNo()
        {
            //get last id
            int lastId = 0;
            if (List().Count() > 0)
            {
                lastId = List().Max(p => p.Id);
            }

            //MMyy-series
            string poNumber = DateTime.Now.ToString("MMyy") + "-" + (lastId + 1).ToString().PadLeft(4, '0');

            return poNumber;
        }
    }
}