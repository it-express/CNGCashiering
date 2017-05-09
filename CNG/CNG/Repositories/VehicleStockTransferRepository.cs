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
        CompanyRepository companyRepo = new CompanyRepository();
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

        public string GenerateVehicleStockTransferNo(DateTime Date)
        {
            int companyId = Sessions.CompanyId.Value;
            //get last id
            int lastId = 0;
            if (List().Count() > 0)
            {
                lastId = List().Max(p => p.Id);
            }

            string prefix = companyRepo.GetById(companyId).Prefix;
            //MMyy-series
            string stNo = prefix + Date.ToString("MMyy") + "-" + (lastId + 1).ToString().PadLeft(4, '0');

            bool poExist = context.VehicleStockTransfers.Count(p => p.No == stNo) > 0;

            while (poExist)
            {
                if (List().Count() > 0)
                {
                    lastId = lastId + 1;
                    stNo = prefix + Date.ToString("MMyy") + "-" + lastId.ToString().PadLeft(4, '0');
                    poExist = context.VehicleStockTransfers.Count(p => p.No == stNo) > 0;
                }

            }

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

            vst.No = GenerateVehicleStockTransferNo(vst.Date);
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

        public void Checked(VehicleStockTransfer po)
        {
            VehicleStockTransfer dbEntry = context.VehicleStockTransfers.FirstOrDefault(p => p.No == po.No);
            if (dbEntry != null)
            {
                dbEntry.Checked = po.Checked;
            }

            context.SaveChanges();
        }

        public void Approved(VehicleStockTransfer po)
        {
            VehicleStockTransfer dbEntry = context.VehicleStockTransfers.FirstOrDefault(p => p.No == po.No);
            if (dbEntry != null)
            {
                dbEntry.Approved = po.Approved;
            }

            context.SaveChanges();
        }
    }
}