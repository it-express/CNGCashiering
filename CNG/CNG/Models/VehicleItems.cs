using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CNG.Models
{
    public class VehicleItems
    {
        [Key]
        public int Id { get; set; }

        public int VehicleId { get; set; }

        public int? TransactionLogId { get; set; }
      
    }
}