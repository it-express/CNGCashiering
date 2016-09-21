using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CNG.Models
{
    public class VendorRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<Vendor> List()
        {
            return context.Vendors;
        }

        public Vendor GetById(int id)
        {
            Vendor vendor = context.Vendors.FirstOrDefault(p => p.Id == id);

            return vendor;
        }

        public void Save(Vendor vendor)
        {
            if (vendor.Id == 0)
            {
                context.Vendors.Add(vendor);
            }
            else
            {
                Vendor dbEntry = context.Vendors.Find(vendor.Id);
                if (dbEntry != null)
                {
                    dbEntry.Name = vendor.Name;
                    dbEntry.Address = vendor.Address;
                    dbEntry.ContactPerson = vendor.ContactPerson;
                    dbEntry.ContactNo = vendor.ContactNo;
                    dbEntry.Terms = vendor.Terms;
                    dbEntry.Active = vendor.Active;
                }
            }

            context.SaveChanges();
        }

        public void Delete(int id)
        {
            Vendor vendor = context.Vendors.Find(id);

            context.Vendors.Remove(vendor);

            context.SaveChanges();
        }
    }
}