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

        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemType> ItemTypes { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }

        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        public virtual DbSet<RequisitionPurchase> RequisitionPurchases { get; set; }
        public virtual DbSet<RequisitionPurchaseItem> RequisitionPurchaseItems { get; set; }

        public virtual DbSet<ExcessPartsSet> ExcessPartsSets { get; set; }
        public virtual DbSet<ExcessPartsSetItem> ExcessPartsSetItems { get; set; }

        public virtual DbSet<TransactionLog> TransactionLogs { get; set; }
        public virtual DbSet<TransactionMethod> TransactionMethods { get; set; }

        public virtual DbSet<Requisition> Requisitions { get; set; }
        public virtual DbSet<RequisitionItem> RequisitionItems { get; set; }
    }
}