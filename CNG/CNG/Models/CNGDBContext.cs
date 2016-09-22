using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CNG.Models
{
    public class CNGDBContext : DbContext
    {
        public CNGDBContext()
            : base("name=CNGDBContext")
        {
        }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemType> ItemTypes { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }

        public System.Data.Entity.DbSet<CNG.Models.PurchaseOrder> PurchaseOrders { get; set; }
        public System.Data.Entity.DbSet<CNG.Models.PurchaseOrderItem> PurchaseOrderItems { get; set; }
    }
}