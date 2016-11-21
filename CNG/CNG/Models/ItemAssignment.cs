using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ItemAssignment
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int CompanyId { get; set; }

        public virtual Item Item { get; set; }
        public virtual Company Company { get; set; }
    }
}