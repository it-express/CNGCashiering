using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNGCashier.Models
{
    public class DriverRepo
    {
        private CNGCashierDBContext cngContext = new CNGCashierDBContext();
        public IQueryable<Driver> List()
        {
            return cngContext.Drivers;
        }

        public Driver GetById(int id)
        {
            Driver driver = cngContext.Drivers.FirstOrDefault(p => p.Id == id);

            return driver;
        }

        public int GetByLicenseNo(string licenseNo)
        {
            int driverId = cngContext.Drivers.FirstOrDefault(p => p.LicenseNumber == licenseNo).Id;

            return driverId;
        }

        public void Save(Driver driver)
        {
            if (driver.Id == 0)
            {
                cngContext.Drivers.Add(driver);
            }
            else
            {
                Driver dbEntry = cngContext.Drivers.Find(driver.Id);
                if (dbEntry != null)
                {
                    dbEntry.FirstName = driver.FirstName;
                    dbEntry.MiddleName = driver.MiddleName;
                    dbEntry.LastName = driver.LastName;
                    dbEntry.LicenseNumber = driver.LicenseNumber;
                }
            }

            cngContext.SaveChanges();
        }

        public void Delete(int id)
        {
            Driver driver = cngContext.Drivers.Find(id);

            cngContext.Drivers.Remove(driver);

            cngContext.SaveChanges();
        }
    }
}