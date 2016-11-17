using CNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG
{
    public class ItemClassificationRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<ItemClassification> List()
        {
            return context.ItemClassifications;
        }

        public ItemClassification GetById(int id)
        {
            ItemClassification itemClassification = context.ItemClassifications.FirstOrDefault(p => p.Id == id);

            return itemClassification;
        }
    }
}