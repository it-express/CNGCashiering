using CNG.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class RequisitionRepository
    {
        private CNGDBContext context = new CNGDBContext();
        RequisitionItemRepository reqItemRepo = new RequisitionItemRepository();

        public RequisitionRepository() {
        }

        public IQueryable<Requisition> List()
        {
            return context.Requisitions;
        }

        public Requisition GetByNo(string reqNo)
        {
            Requisition req = context.Requisitions.FirstOrDefault(p => p.No == reqNo);

            return req;
        }

        public Requisition GetAll()
        {
            Requisition req = context.Requisitions.FirstOrDefault();

            return req;
        }

        public void Save(Requisition req) {
            bool reqExists = context.Requisitions.Any(p => p.No == req.No);

            int id;

            if (!reqExists)
            {
                context.Requisitions.Add(req);

                context.SaveChanges();
                
                id = req.Id;

                foreach (RequisitionItem reqItem in req.RequisitionItems)
                {
                    reqItem.RequisitionId = id;
                    if (reqItem.Quantity != 0)
                    {
                        reqItem.TransactionLogId = InsertLogs(reqItem.ItemId, reqItem.Quantity, req.Date);
                        InsertStockCard(id, reqItem.ItemId, reqItem.Item.UnitCost, reqItem.Quantity, reqItem.TransactionLogId);
                    }
                }
            }
            else
            {
                Requisition dbEntry = context.Requisitions.FirstOrDefault(p => p.No == req.No);
                if (dbEntry != null)
                {
                    dbEntry.JobOrderDate = req.Date;
                    dbEntry.JobOrderNo = req.JobOrderNo;
                    dbEntry.UnitPlateNo = req.UnitPlateNo;
                    dbEntry.JobOrderDate = req.JobOrderDate;
                    dbEntry.OdometerReading = req.OdometerReading;
                    dbEntry.DriverName = req.DriverName;
                    dbEntry.ReportedBy = req.ReportedBy;
                    dbEntry.CheckedBy = req.CheckedBy;
                    dbEntry.ApprovedBy = req.ApprovedBy;
                    dbEntry.CompanyId = req.CompanyId;
                }

                context.SaveChanges();

                id = dbEntry.Id;

                //Delete previous items
                foreach (RequisitionItem reqItem in dbEntry.RequisitionItems.ToList())
                {
                    //Delete previous logs
                    TransactionLogRepository transLogRepo = new TransactionLogRepository();
                    transLogRepo.Remove(reqItem.TransactionLogId.Value);

                    reqItemRepo.Remove(reqItem.Id);
                }

                foreach (RequisitionItem reqItem in req.RequisitionItems)
                {
                    reqItem.RequisitionId = id;
                    if (reqItem.Quantity != 0)
                    {
                        reqItem.TransactionLogId = InsertLogs(reqItem.ItemId, reqItem.Quantity, req.Date);
                    }

                    context.RequisitionItems.Add(reqItem);
                }
            }

            context.SaveChanges();
        }

        public void Delete(string reqNo) {
            Requisition req = context.Requisitions.FirstOrDefault(p => p.No == reqNo);

            context.Requisitions.Remove(req);

            context.SaveChanges();
        }

        public string GenerateReqNo()
        {
            //get last id
            int lastId = 0;
            if (List().Count() > 0)
            {
                lastId = List().Max(p => p.Id);
            }

            //MMyy-series
            string reqNo = DateTime.Now.ToString("MMyy") + "-" + (lastId + 1).ToString().PadLeft(4, '0');

            return reqNo;
        }

        public int InsertLogs(int itemId, int quantiy, DateTime req_Date)
        {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = -quantiy,
                Date = req_Date,
                TransactionMethodId = (int)ETransactionMethod.Requisition,
                CompanyId = Sessions.CompanyId.Value
            };

            return transactionLogRepo.Add(transactionLog);
        }

        public void InsertStockCard(int ReferenceId, int itemId, decimal unitcost, int quantiy, int? TransLogId)
        {
            ItemStockCardRepository stockcardRepo = new ItemStockCardRepository();

            StockCard stockCard = new StockCard
            {
                ItemId = itemId,
                ReferenceModule = "Requisition",
                ReferenceId = ReferenceId,
                Qty = quantiy,
                UnitCost = unitcost,
                CompanyId = Sessions.CompanyId.Value,
                Date = DateTime.Now,
                TransLogId = TransLogId
            };


            stockcardRepo.Add(stockCard);
        }
    }
}