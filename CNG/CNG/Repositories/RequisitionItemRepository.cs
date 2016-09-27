using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class RequisitionItemRepository
    {
        private CNGDBContext context;

        public RequisitionItemRepository(CNGDBContext _context)
        {
            this.context = _context;
        }

        public RequisitionItem Find(int reqItemId)
        {
            RequisitionItem item = context.RequisitionItems.Find(reqItemId);

            return item;
        }

        public void Save(RequisitionItem reqItem)
        {
            context.RequisitionItems.Add(reqItem);

            context.SaveChanges();
        }
    }
}