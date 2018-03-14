using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNGCashier.Models
{
    public class VehicleRepo
    {
        private CNGCashierDBContext context = new CNGCashierDBContext();

        public IQueryable<Vehicle> List()
        {
            return context.Vehicles;
        }

        public Vehicle GetById(int id)
        {
            Vehicle vehicle = context.Vehicles.FirstOrDefault(p => p.Id == id);

            return vehicle;
        }

        public int GetIdByPlateNo(string PlateNo)
        {
            int vehicleId = context.Vehicles.FirstOrDefault(p => p.LicenseNo == PlateNo).Id;

            return vehicleId;
        }


        public void Save(Vehicle vehicle)
        {
            if (vehicle.Id == 0)
            {
                context.Vehicles.Add(vehicle);
            }
            else
            {
                Vehicle dbEntry = context.Vehicles.Find(vehicle.Id);
                if (dbEntry != null)
                {
                    dbEntry.Make = vehicle.Make;
                    dbEntry.Year = vehicle.Year;
                    dbEntry.Model = vehicle.Model;
                    dbEntry.CnNo = vehicle.CnNo;
                    dbEntry.LicenseNo = vehicle.LicenseNo;
                    dbEntry.EngineNo = vehicle.EngineNo;
                    dbEntry.ChasisNo = vehicle.ChasisNo;
                    dbEntry.Color = vehicle.Color;
                    dbEntry.Active = vehicle.Active;
                }
            }

            context.SaveChanges();
        }


        public void Delete(int id)
        {
            Vehicle vehicle = context.Vehicles.Find(id);

            context.Vehicles.Remove(vehicle);

            context.SaveChanges();
        }
    }
}