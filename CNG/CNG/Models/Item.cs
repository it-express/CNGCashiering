using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CNG.Models
{
    public class Item
    {
        TransactionLogRepository transLogRepo = new TransactionLogRepository();

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Code { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(250)]
        public string Brand { get; set; }

        [Required]
        [DisplayName("Unit Cost")]
        [DataType(DataType.Text)]
        public decimal UnitCost { get; set; }

        [Required]
        [DisplayName("Type")]
        public int TypeId { get; set; }

        [Required]
        public bool Active { get; set; }

        [DisplayName("Quantity on Hand")]
        public int QuantityOnHand {
            get {
                 return transLogRepo.SumByItemId(Id);
            }
        }

        public virtual ItemType Type { get; set; }
    }
}