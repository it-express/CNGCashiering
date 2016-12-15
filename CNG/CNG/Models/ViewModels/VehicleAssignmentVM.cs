using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class VehicleAssignmentVM
    {
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public string VehiclePlateNo { get; set; }
        public bool IsAssigned { get; set; }
    }
}