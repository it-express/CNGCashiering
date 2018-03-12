using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CNGCashier.Models
{
    public class CNGCashierDBContext : DbContext
    {
        public CNGCashierDBContext()
           : base("name=CNGCashierDBContext")
        {
        }

        public virtual DbSet<Driver> Drivers { get; set; }

        public virtual DbSet<Company> Companies { get; set; }

        public virtual DbSet<Vehicle> Vehicles { get; set; }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }


    }
}