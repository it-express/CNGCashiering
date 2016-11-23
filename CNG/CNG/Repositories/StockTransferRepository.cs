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

        public void Save(StockTransfer st)
        {
            bool exists = context.StockTransfers.Count(p => p.No == st.No) > 0;

            int id;

            if (!exists)
            {
                context.StockTransfers.Add(st);

                context.SaveChanges();

                id = st.Id;
            }
            else
            {
                StockTransfer dbEntry = context.StockTransfers.FirstOrDefault(p => p.No == st.No);
                if (dbEntry != null)
                {
                    dbEntry.No = st.No;

                    dbEntry.Date = st.Date;
                    dbEntry.TransferFrom = st.TransferFrom;

                    dbEntry.PreparedBy = Common.GetCurrentUser.Id;
                    dbEntry.ApprovedBy = Common.GetCurrentUser.GeneralManagerId;
                }

                id = dbEntry.Id;

                //Delete previous items
                foreach (StockTransferItem stItem in dbEntry.StockTransferItems.ToList())
                {
                    context.StockTransferItems.Remove(stItem);
                }

                foreach (StockTransferItem stItem in st.StockTransferItems.ToList())
                {
                    stItem.StockTransferId = id;

                    context.StockTransferItems.Add(stItem);
                }
            }

            context.SaveChanges();
        }
    }
}