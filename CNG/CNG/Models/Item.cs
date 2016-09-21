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
        public decimal UnitCost { get; set; }

        [Required]
        public int TypeId { get; set; }

        [Required]
        public bool Active { get; set; }

        public virtual ItemType Type { get; set; }
    }
}