using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class VehicleAssignment
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int CompanyId { get; set; }
    }
}