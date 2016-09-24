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
    }
}