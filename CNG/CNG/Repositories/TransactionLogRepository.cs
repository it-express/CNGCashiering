using CNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class TransactionLogRepository
    {
        private CNGDBContext context = new CNGDBContext();
        public ItemRepository itemRepo = new ItemRepository();

        public IQueryable<TransactionLog> List()
        {
            return context.TransactionLogs;
        }

        public int Add(TransactionLog transactionLog)
        {
            transactionLog.Date = DateTime.Now;
            transactionLog.UserId = Convert.ToInt32(HttpContext.Current.Session["uid"]); //get from current session
            transactionLog.CompanyId = transactionLog.CompanyId;

            context.TransactionLogs.Add(transactionLog);
            context.SaveChanges();

            return transactionLog.Id;
        }

        public int SumByItemId(int itemId, int companyId)
        {
            IQueryable<TransactionLog> lstTransactionLog = context.TransactionLogs.Where(p => p.ItemId == itemId && p.CompanyId == companyId);
            int quantity = 0;
            if (lstTransactionLog.Count() > 0) {
                quantity = lstTransactionLog.Sum(p => p.Quantity);
            }

            return quantity;   
        }

        public void Remove(int id) {
            TransactionLog transLog = List().FirstOrDefault(p => p.Id == id);

            context.TransactionLogs.Remove(transLog);

            context.SaveChanges();
        }
    }
}