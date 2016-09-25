using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class RequisitionPurchaseItemRepository
    {
        private CNGDBContext context;

        public RequisitionPurchaseItemRepository(CNGDBContext _context)
        {
            this.context = _context;
        }

        public RequisitionPurchaseItem Find(int rpItemId)
        {
            RequisitionPurchaseItem item = context.RequisitionPurchaseItems.Find(rpItemId);

            return item;
        }

        public void Save(RequisitionPurchaseItem rpItem)
        {
            context.RequisitionPurchaseItems.Add(rpItem);

            context.SaveChanges();
        }

        public List<RequisitionPurchaseItem> ListByRpId(int id)
        {
            IQueryable<RequisitionPurchaseItem> lstRpItem = context.RequisitionPurchaseItems.Where(p => p.RequisitionPurchaseId == id);

            return lstRpItem.ToList();
        }
    }
}