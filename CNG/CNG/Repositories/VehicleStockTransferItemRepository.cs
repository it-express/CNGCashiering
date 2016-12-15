using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class VehicleStockTransferItemRepository
    {
        private CNGDBContext context;

        public VehicleStockTransferItemRepository(CNGDBContext _context)
        {
            this.context = _context;
        }
        public IQueryable<VehicleStockTransferItem> List()
        {
            return context.VehicleStockTransferItems;
        }
        public VehicleStockTransferItem Find(int stItemId)
        {
            VehicleStockTransferItem item = context.VehicleStockTransferItems.Find(stItemId);

            return item;
        }

        public void Save(VehicleStockTransferItem vehicleStockTransItem)
        {
            context.VehicleStockTransferItems.Add(vehicleStockTransItem);

            context.SaveChanges();
        }

        public List<VehicleStockTransferItem> ListByVehicleStockTransId(int id)
        {
            IQueryable<VehicleStockTransferItem> lstVehicleStockTransItem = context.VehicleStockTransferItems.Where(p => p.VehicleStockTransferId == id);

            return lstVehicleStockTransItem.ToList();
        }
    }
}