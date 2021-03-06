﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class CompanyRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<Company> List()
        {
            return context.Companies;
        }

        public Company GetById(int id)
        {
            Company company = context.Companies.FirstOrDefault(p => p.Id == id);

            SqlParameter parameter1 = new SqlParameter("@CompanyID", Sessions.CompanyId);
            var affectedRows = context.Database.ExecuteSqlCommand("sp_Update_Item_UnitCost @CompanyID", parameter1);
            var affectedRows1 = context.Database.ExecuteSqlCommand("spUpdate_Items_QuantityOnHand");

            return company;
        }

        public void Save(Company company)
        {
            if (company.Id == 0)
            {
                context.Companies.Add(company);
            }
            else
            {
                Company dbEntry = context.Companies.Find(company.Id);
                if (dbEntry != null)
                {
                    dbEntry.Name = company.Name;

                    dbEntry.Address = company.Address;
                    dbEntry.ContactPerson = company.ContactPerson;
                    dbEntry.ContactNo = company.ContactNo;
                    dbEntry.Prefix = company.Prefix;
                    dbEntry.Active = company.Active;

                    //update identity seed
                    //Company dbEntry = context.Companies.Find(company.Id);
                }
            }

            context.SaveChanges();
        }

        public void Delete(int id)
        {
            Company company = context.Companies.Find(id);

            context.Companies.Remove(company);

            context.SaveChanges();
        }
    }
}