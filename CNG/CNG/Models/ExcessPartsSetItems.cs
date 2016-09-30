using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ExcessPartsSetItem
    {
        [Key]
        public int Id { get; set; }
        public int ExcessPartsSetId { get; set; }
        public int ItemId { get; set; }
        [DisplayName("Unit Cost")]
        public decimal UnitCost { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }

        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }
    }
}