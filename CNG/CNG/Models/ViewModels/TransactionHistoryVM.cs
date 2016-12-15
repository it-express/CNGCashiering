using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class TransactionHistoryVM
    {
        public int ItemId { get; set; }
        public int CompanyId { get; set; }
        public IPagedList<TransactionLog> TransactionLogs {get;set;}
        public virtual Item Item { get; set; }
        public virtual Company Company { get; set; }

    }

    
}