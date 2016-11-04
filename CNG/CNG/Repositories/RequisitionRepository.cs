using CNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class RequisitionRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<Requisition> List()
        {
            return context.Requisitions;
        }

        public Requisition GetByNo(string reqNo)
        {
            Requisition req = context.Requisitions.FirstOrDefault(p => p.No == reqNo);

            return req;
        }

        public void Save(Requisition req) {
            bool reqExists = context.PurchaseOrders.Count(p => p.No == req.No) > 0;

            int id;

            if (!reqExists)
            {
                context.Requisitions.Add(req);

                context.SaveChanges();

                id = req.Id;
            }
            else
            {
                Requisition dbEntry = context.Requisitions.FirstOrDefault(p => p.No == req.No);
                if (dbEntry != null)
                {
                    req.Date = dbEntry.JobOrderDate;
                    req.JobOrderNo = dbEntry.JobOrderNo;
                    req.UnitPlateNo = dbEntry.UnitPlateNo;
                    req.JobOrderDate = dbEntry.JobOrderDate;
                    req.OdometerReading = dbEntry.OdometerReading;
                    req.DriverName = dbEntry.DriverName;
                    req.ReportedBy = dbEntry.ReportedBy;
                    req.CheckedBy = dbEntry.CheckedBy;
                    req.ApprovedBy = dbEntry.ApprovedBy;
                    req.CompanyId = dbEntry.CompanyId;
                }

                id = dbEntry.Id;

                //Delete previous items
                foreach (RequisitionItem reqItem in dbEntry.RequisitionItems.ToList())
                {
                    context.RequisitionItems.Remove(reqItem);
                }

                foreach (RequisitionItem reqItem in req.RequisitionItems.ToList())
                {
                    reqItem.RequisitionId = id;

                    context.RequisitionItems.Add(reqItem);

                    InsertLogs(reqItem.ItemId, reqItem.Quantity);
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

        public void InsertLogs(int itemId, int quantiy)
        {
            TransactionLogRepository transactionLogRepo = new TransactionLogRepository();

            TransactionLog transactionLog = new TransactionLog
            {
                ItemId = itemId,
                Quantity = -quantiy,
                TransactionMethodId = (int)ETransactionMethod.Requisition,
                CompanyId = Sessions.CompanyId.Value
            };

            transactionLogRepo.Add(transactionLog);
        }
    }
}