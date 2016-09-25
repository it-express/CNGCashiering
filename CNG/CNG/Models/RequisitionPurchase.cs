using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class RequisitionPurchase
    {
        [Key]
        public int Id { get; set; }
        public string No { get; set; }
        public DateTime Date { get; set; }
        public int PreparedBy { get; set; }
        public int ApprovedBy { get; set; }


        public virtual List<RequisitionPurchaseItem> RequisitionPurchaseItems { get; set; }
    }
}