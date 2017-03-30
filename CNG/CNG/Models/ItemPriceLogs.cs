using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CNG.Models
{
    public class ItemPriceLogs
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal UnitCost { get; set; }

        [Required]
        public int Qty { get; set; }

        public DateTime Date { get; set; }


      

    }


}