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

        public void Add(TransactionLog transactionLog)
        {
            transactionLog.Date = DateTime.Now;
            transactionLog.UserId = Convert.ToInt32(HttpContext.Current.Session["uid"]); //get from current session

            context.TransactionLogs.Add(transactionLog);
           
            context.SaveChanges();

            //itemRepo.AdjustQuantity(transactionLog.ItemId, transactionLog.Quantity);
        }

        public int SumByItemId(int itemId)
        {
            IQueryable<TransactionLog> lstTransactionLog = context.TransactionLogs.Where(p => p.ItemId == itemId);
            int quantity = 0;
            if (lstTransactionLog.Count() > 0) {
                quantity = lstTransactionLog.Sum(p => p.Quantity);
            }

            return quantity;   
        }
    }
}