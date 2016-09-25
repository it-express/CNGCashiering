using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ExcessPartsSetItemRepository
    {
        private CNGDBContext context;

        public ExcessPartsSetItemRepository(CNGDBContext _context)
        {
            this.context = _context;
        }

        public ExcessPartsSetItem Find(int epsItemId)
        {
            ExcessPartsSetItem item = context.ExcessPartsSetItems.Find(epsItemId);

            return item;
        }

        public void Save(ExcessPartsSetItem epsItem)
        {
            context.ExcessPartsSetItems.Add(epsItem);

            context.SaveChanges();
        }

        public List<ExcessPartsSetItem> ListByEpsId(int id)
        {
            IQueryable<ExcessPartsSetItem> lstEpsItem = context.ExcessPartsSetItems.Where(p => p.ExcessPartsSetId == id);

            return lstEpsItem.ToList();
        }
    }
}