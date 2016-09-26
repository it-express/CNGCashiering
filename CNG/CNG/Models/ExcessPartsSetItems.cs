﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public decimal UnitCost { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }
    }
}