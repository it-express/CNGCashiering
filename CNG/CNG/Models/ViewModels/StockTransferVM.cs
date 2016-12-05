using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNG.Models
{
    public class StockTransferVM
    {
        public StockTransfer StockTransfer { get; set; }
        public IEnumerable<SelectListItem> ItemTypes { get; set; }
    }
}