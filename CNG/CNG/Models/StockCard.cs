using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace CNG.Models
{
    public class StockCard
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string ReferenceModule { get; set; }

        [Required]
        public int ReferenceId { get; set; }
    
        [Required]
        public int ItemId { get; set; }

        [Required]
        public int Qty { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal UnitCost { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public int CompanyId { get; set; }
        public int? TransLogId { get; set; }



    }


}