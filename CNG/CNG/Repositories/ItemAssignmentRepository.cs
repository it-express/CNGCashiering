using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ItemAssignmentRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<ItemAssignment> List()
        {
            return context.ItemAssignments;

        }

       
        public void Save(List<ItemAssignment> lstItemAssignment) {
            int companyId = lstItemAssignment[0].CompanyId;

            context.Database.ExecuteSqlCommand("DELETE FROM ItemAssignments WHERE CompanyId = " + companyId);

            foreach (ItemAssignment itemAssignment in lstItemAssignment) {
                context.ItemAssignments.Add(itemAssignment);
            }

            context.SaveChanges();
        }

        public decimal GetUnitCostByCompany(int id, int? companyid)
        {
            decimal item = context.ItemAssignments.FirstOrDefault(p => p.ItemId == id && p.CompanyId == companyid).UnitCost;

            return item;
        }

        public decimal GetLatestUnitCostByCompany(int id, int? companyid)
        {
            decimal item = context.ItemPriceLogs.OrderByDescending(p=>p.Id).FirstOrDefault(p => p.ItemId == id && p.CompanyId == companyid).UnitCost;

            return item;
        }

    }
}