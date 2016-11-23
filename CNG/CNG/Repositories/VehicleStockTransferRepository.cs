using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class VehicleStockTransferRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<VehicleStockTransfer> List()
        {
            return context.VehicleStockTransfers;
        }

        public VehicleStockTransfer GetById(int id)
        {
            VehicleStockTransfer vehicleStockTransfer = context.VehicleStockTransfers.FirstOrDefault(p => p.Id == id);

            return vehicleStockTransfer;
        }

        public VehicleStockTransfer GetByNo(string no)
        {
            VehicleStockTransfer vehicleStockTransfer = context.VehicleStockTransfers.FirstOrDefault(p => p.No == no);

            return vehicleStockTransfer;
        }

        public void Delete(string no)
        {
            VehicleStockTransfer vehicleStockTransfer = context.VehicleStockTransfers.FirstOrDefault(p => p.No == no);

            context.VehicleStockTransfers.Remove(vehicleStockTransfer);

            context.SaveChanges();
        }

        public string GenerateVehicleStockTransferNo()
        {
            //get last id
            int lastId = 0;
            if (List().Count() > 0)
            {
                lastId = List().Max(p => p.Id);
            }

            //MMyy-series
            string stNo = DateTime.Now.ToString("MMyy") + "-" + (lastId + 1).ToString().PadLeft(4, '0');

            return stNo;
        }

        public void Save(VehicleStockTransfer vst)
        {
            bool exists = context.VehicleStockTransfers.Count(p => p.No == vst.No) > 0;

            int id;

            if (!exists)
            {
                context.VehicleStockTransfers.Add(vst);

                context.SaveChanges();

                id = vst.Id;
            }
            else
            {
                VehicleStockTransfer dbEntry = context.VehicleStockTransfers.FirstOrDefault(p => p.No == vst.No);
                if (dbEntry != null)
                {
                    dbEntry.No = vst.No;

                    dbEntry.Date = vst.Date;
                    

                    dbEntry.RequestedBy = vst.RequestedBy;
                    dbEntry.CheckedBy = Common.GetCurrentUser.Id;
                    dbEntry.ApprovedBy = Common.GetCurrentUser.GeneralManagerId;
                }
                
                id = dbEntry.Id;

                //Delete previous items
                foreach (VehicleStockTransferItem vstIitem in dbEntry.VehicleStockTransferItems.ToList())
                {
                    context.VehicleStockTransferItems.Remove(vstIitem);
                }

                foreach (VehicleStockTransferItem vstIitem in vst.VehicleStockTransferItems.ToList())
                {
                    vstIitem.VehicleStockTransferId = id;

                    context.VehicleStockTransferItems.Add(vstIitem);
                }
            }

            context.SaveChanges();
        }
    }
}