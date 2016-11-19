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
    }
}