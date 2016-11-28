using CNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class StockTransferRepository
    {
        private CNGDBContext context = new CNGDBContext();
        StockTransferItemRepository stItemRepo = new StockTransferItemRepository();

        public IQueryable<StockTransfer> List()
        {
            return context.StockTransfers;
        }

        public StockTransfer GetById(int id)
        {
            StockTransfer stockTransfer = context.StockTransfers.FirstOrDefault(p => p.Id == id);

            return stockTransfer;
        }

        public StockTransfer GetByNo(string no)
        {
            StockTransfer stockTransfer = context.StockTransfers.FirstOrDefault(p => p.No == no);

            return stockTransfer;
        }

        public void Delete(string no)
        {
            StockTransfer stockTransfer = context.StockTransfers.FirstOrDefault(p => p.No == no);

            context.StockTransfers.Remove(stockTransfer);

            context.SaveChanges();
        }

        public string GenerateStockTransferNo()
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

        public void Save(StockTransfer st)
        {
            bool exists = context.StockTransfers.Any(p => p.No == st.No);

            int id;

            if (!exists)
            {
                context.StockTransfers.Add(st);

                context.SaveChanges();

                id = st.Id;

                foreach (StockTransferItem stItem in st.StockTransferItems)
                {
                    stItem.StockTransferId = id;

                    if (stItem.Quantity != 0)
                    {
                        stItem.TransactionLogId = InsertLogs(stItem.ItemId, stItem.Quantity);
                    }
                }
            }
            else
            {
                StockTransfer dbEntry = context.StockTransfers.FirstOrDefault(p => p.No == st.No);
                if (dbEntry != null)
                {
                    dbEntry.JobOrderDate = st.Date;
                    dbEntry.JobOrderNo = st.JobOrderNo;
                    dbEntry.UnitPlateNo = st.UnitPlateNo;
                    dbEntry.JobOrderDate = st.JobOrderDate;
                    dbEntry.OdometerReading = st.OdometerReading;
                    dbEntry.DriverName = st.DriverName;
                    dbEntry.ReportedBy = st.ReportedBy;
                    dbEntry.CheckedBy = st.CheckedBy;
                    dbEntry.ApprovedBy = st.ApprovedBy;
                    dbEntry.CompanyId = st.CompanyId;
                }

                context.SaveChanges();

                id = dbEntry.Id;

                //Delete previous items
                foreach (StockTransferItem stItem in dbEntry.StockTransferItems.ToList())
                {
                    //Delete previous logs
                    TransactionLogRepository transLogRepo = new TransactionLogRepository();
                    transLogRepo.Remove(stItem.TransactionLogId.Value);

                    stItemRepo.Remove(stItem.Id);
                }

                foreach (StockTransferItem stItem in st.StockTransferItems)
                {
                    stItem.StockTransferId = id;
                    if (stItem.Quantity != 0)
                    {
                        stItem.TransactionLogId = InsertLogs(stItem.ItemId, stItem.Quantity);
                    }

                    context.StockTransferItems.Add(stItem);
                }
            }

            context.SaveChanges();
        }

        public int InsertLogs(int itemId, int quantiy)
        {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = -quantiy,
                TransactionMethodId = (int)ETransactionMethod.Requisition,
                CompanyId = Sessions.CompanyId.Value
            };

            return transactionLogRepo.Add(transactionLog);
        }
    }
}