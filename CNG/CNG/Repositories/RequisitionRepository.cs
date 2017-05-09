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
        CompanyRepository companyRepo = new CompanyRepository();
        VehicleRepository vehicleRepo = new VehicleRepository();
        VehicleItemsRepository veItemRepo = new VehicleItemsRepository();
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

            req.No = GenerateReqNo(req.Date);
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
                        InsertStockCard(id, reqItem.ItemId, reqItem.GetItemUnitCost, reqItem.Quantity, reqItem.TransactionLogId);
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


            int vehicleId = vehicleRepo.GetIdByPlateNo(req.UnitPlateNo);
            int? translogId = req.RequisitionItems.Last().TransactionLogId;
            SaveVehicle(vehicleId, translogId);

            context.SaveChanges();
        }

        public void SaveVehicle(int vehicleId, int? translogId)
        {
            VehicleItems vi = new VehicleItems();
            vi.VehicleId = vehicleId;
            vi.TransactionLogId = translogId;

            veItemRepo.Save(vi);
        }

        public void Delete(string reqNo) {
            Requisition req = context.Requisitions.FirstOrDefault(p => p.No == reqNo);

            context.Requisitions.Remove(req);

            context.SaveChanges();
        }

        public string GenerateReqNo(DateTime Date)
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
            string reqNo = prefix + Date.ToString("MMyy") + "-" + (lastId + 1).ToString().PadLeft(4, '0');

            bool poExist = context.Requisitions.Count(p => p.No == reqNo) > 0;

            while (poExist)
            {
                if (List().Count() > 0)
                {
                    lastId = lastId + 1;
                    reqNo = prefix + Date.ToString("MMyy") + "-" + lastId.ToString().PadLeft(4, '0');
                    poExist = context.Requisitions.Count(p => p.No == reqNo) > 0;
                }

            }

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

        public void Checked(Requisition po)
        {
            Requisition dbEntry = context.Requisitions.FirstOrDefault(p => p.No == po.No);
            if (dbEntry != null)
            {
                dbEntry.Checked = po.Checked;
            }

            context.SaveChanges();
        }

        public void Approved(Requisition po)
        {
            Requisition dbEntry = context.Requisitions.FirstOrDefault(p => p.No == po.No);
            if (dbEntry != null)
            {
                dbEntry.Approved = po.Approved;
            }

            context.SaveChanges();
        }
    }
}