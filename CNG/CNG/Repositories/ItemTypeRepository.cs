using CNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ItemTypeRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<ItemType> List()
        {
            return context.ItemTypes;
        }

        public ItemType GetById(int id)
        {
            ItemType itemType = context.ItemTypes.FirstOrDefault(p => p.Id == id);

            return itemType;
        }
    }
}