using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNG.Models
{
    public class RequisitionVM
    {
        public Requisition Requisition { get; set; }
        public IEnumerable<SelectListItem> ItemTypes { get; set; }
    }
}