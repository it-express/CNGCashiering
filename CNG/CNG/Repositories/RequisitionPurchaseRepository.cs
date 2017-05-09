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
        CompanyRepository companyRepo = new CompanyRepository();

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

        public string GenerateRpNo(DateTime Date)
        {
            int companyId = Sessions.CompanyId.Value;
            //get last id
            int lastId = 0;
            if (List().Count() > 0)
            {
                lastId = List().Max(p => p.Id);
            }

            string prefix = companyRepo.GetById(companyId).Prefix;
            //MMyy-series
            string poNumber = prefix + "-RP" + Date.ToString("MMyy") + "-" + (lastId + 1).ToString().PadLeft(4, '0');

            bool poExist = context.RequisitionPurchases.Count(p => p.No == poNumber) > 0;

            while (poExist)
            {
                if (List().Count() > 0)
                {
                    lastId = lastId + 1;
                    poNumber = prefix + "-RP" + Date.ToString("MMyy") + "-" + lastId.ToString().PadLeft(4, '0');
                    poExist = context.RequisitionPurchases.Count(p => p.No == poNumber) > 0;
                }

            }
            return poNumber;
        }

      
    }
}