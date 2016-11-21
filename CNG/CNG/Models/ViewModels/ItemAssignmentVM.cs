using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ItemAssignmentVM
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public bool IsAssigned { get; set; }
    }
}