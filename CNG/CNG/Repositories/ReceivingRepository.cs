using CNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG
{
    public class ReceivingRepository
    {
        private CNGDBContext context = new CNGDBContext();
        CompanyRepository companyRepo = new CompanyRepository();
        public IQueryable<Receiving> List()
        {
            return context.Receivings;
        }

        public Receiving GetById(int id)
        {
            Receiving item = context.Receivings.FirstOrDefault(p => p.Id == id);

            return item;
        }

        public IEnumerable<Receiving> ListByPurchaseOrderItemId(int poItemId) {
            IEnumerable<Receiving> lstItem = context.Receivings.Where(p => p.PurchaseOrderItemId == poItemId);

            return lstItem;
        }


        public void Save(Receiving receiving)
        {
            if (receiving.Id == 0)
            {
                context.Receivings.Add(receiving);
            }
            else
            {
                Receiving dbEntry = context.Receivings.Find(receiving.Id);
                if (dbEntry != null)
                {
                    dbEntry.PurchaseOrderItemId = receiving.PurchaseOrderItemId;
                    dbEntry.Quantity = receiving.Quantity;
                    dbEntry.SerialNo = receiving.SerialNo;
                    dbEntry.DrNo = receiving.DrNo;
                    dbEntry.DateReceived = receiving.DateReceived;
                    dbEntry.TransactionLogId = receiving.TransactionLogId;

                    context.SaveChanges();

                    TransactionLogRepository transLogRepo = new TransactionLogRepository();
                    transLogRepo.Update(dbEntry.TransactionLogId.Value, receiving.Quantity, receiving.DateReceived.Value);
                }
            }

            context.SaveChanges();
        }
    }
}