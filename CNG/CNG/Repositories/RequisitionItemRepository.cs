using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class RequisitionItemRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public RequisitionItemRepository()
        {
        }

        public RequisitionItem Find(int reqItemId)
        {
            RequisitionItem item = context.RequisitionItems.Find(reqItemId);

            return item;
        }

        public void Remove(int reqItemId) {
            RequisitionItem item = context.RequisitionItems.Find(reqItemId);

            context.RequisitionItems.Remove(item);

            context.SaveChanges();
        }

        public void Save(RequisitionItem reqItem)
        {
            context.RequisitionItems.Add(reqItem);

            context.SaveChanges();
        }
    }
}