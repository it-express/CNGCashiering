﻿using CNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class VehicleAssignmentRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<VehicleAssignment> List()
        {
            return context.VehicleAssignments;
        }

        public int GetIdByVehicleId(int VehicleId)
        {
            int companyId = context.VehicleAssignments.FirstOrDefault(p => p.VehicleId == VehicleId).CompanyId;

            return companyId;
        }

        public string GetCompanyByVehicleId(int VehicleId)
        {
            string company = context.VehicleAssignments.FirstOrDefault(p => p.VehicleId == VehicleId).Company.Name;

            return company;
        }

        public void Save(List<VehicleAssignment> lstItemAssignment)
        {
            int companyId = lstItemAssignment[0].CompanyId;

            context.Database.ExecuteSqlCommand("DELETE FROM VehicleAssignments WHERE CompanyId = " + companyId);

            foreach (VehicleAssignment vehicleAssignment in lstItemAssignment)
            {
                context.VehicleAssignments.Add(vehicleAssignment);
            }

            context.SaveChanges();
        }

        public void Remove(int vsItem)
        {
            VehicleStockTransferItem item = context.VehicleStockTransferItems.Find(vsItem);

            context.VehicleStockTransferItems.Remove(item);

            context.SaveChanges();
        }


    }
}