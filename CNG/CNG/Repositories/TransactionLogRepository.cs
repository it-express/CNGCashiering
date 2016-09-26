﻿using CNG.Models;
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
            context.TransactionLogs.Add(transactionLog);
           
            context.SaveChanges();

            itemRepo.AdjustQuantity(transactionLog.ItemId, transactionLog.Quantity);
        }
    }
}