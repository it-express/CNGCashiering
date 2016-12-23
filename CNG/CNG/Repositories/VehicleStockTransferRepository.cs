using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class VehicleStockTransferRepository
    {
        private CNGDBContext context = new CNGDBContext();
        VehicleAssignmentRepository vsItemRepo = new VehicleAssignmentRepository();
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
        public int InsertLogs(int itemId, int quantiy)
        {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = -quantiy,
                TransactionMethodId = (int)ETransactionMethod.StockTransfer_Unit,
                CompanyId = Sessions.CompanyId.Value
            };

            return transactionLogRepo.Add(transactionLog);
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

                foreach (VehicleStockTransferItem vsItem in vst.VehicleStockTransferItems)
                {
                    vsItem.VehicleStockTransferId = id;
                    if (vsItem.Quantity != 0)
                    {
                        vsItem.TransactionLogId = InsertLogs(vsItem.ItemId, vsItem.Quantity);
                    }
                }
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
                foreach (VehicleStockTransferItem vsItem in dbEntry.VehicleStockTransferItems.ToList())
                {
                    //Delete previous logs
                    TransactionLogRepository transLogRepo = new TransactionLogRepository();
                    transLogRepo.Remove(vsItem.TransactionLogId.Value);

                    vsItemRepo.Remove(vsItem.Id);
                }

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