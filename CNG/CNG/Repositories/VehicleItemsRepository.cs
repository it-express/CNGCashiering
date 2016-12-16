using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class VehicleItemsRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<VehicleItems> List()
        {
            return context.VehicleItems;
        }

        public Vehicle GetById(int id)
        {
            Vehicle vehicle = context.Vehicles.FirstOrDefault(p => p.Id == id);

            return vehicle;
        }


        public void Save(VehicleItems vehicleitem)
        {
            if (vehicleitem.Id == 0)
            {
                context.VehicleItems.Add(vehicleitem);
            }
            else
            {
                VehicleItems dbEntry = context.VehicleItems.Find(vehicleitem.Id);
                if (dbEntry != null)
                {
                    dbEntry.VehicleId = vehicleitem.VehicleId;
                    dbEntry.TransactionLogId = vehicleitem.TransactionLogId;
                }
            }

            context.SaveChanges();
        }

        public void Delete(int id)
        {
            VehicleItems vehicleitem = context.VehicleItems.Find(id);

            context.VehicleItems.Remove(vehicleitem);

            context.SaveChanges();
        }
    }
}