using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ItemHistoryVM
    {
        public int ItemId { get; set; }
        public int CompanyId { get; set; }
        public IPagedList<TransactionLogVM> TransactionLogs { get; set; }
        public virtual Item Item { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual Company Company { get; set; }
    }

    public class TransactionLogVM
    {
        public TransactionLog TransactionLog { get; set; }
        public string VehicleFrom { get; set; }
    }
}