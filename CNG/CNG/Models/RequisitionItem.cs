using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class RequisitionItem
    {
        [Key]
        public int Id { get; set; }
        public int RequisitionId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string SerialNo { get; set; }
        public int Type { get; set; }

        public virtual Item Item { get; set; }
    }
}