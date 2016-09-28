using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNG.Models
{
    public class Requisition
    {
        public Requisition() {
            RequisitionItems = new List<RequisitionItem>();
        }

        [Key]
        public int Id { get; set; }

        public string No { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string JobOrderNo { get; set; }

        [Required]
        public DateTime JobOrderDate { get; set; }

        [Required]
        [StringLength(100)]
        public string UnitPlateNo { get; set; }

        [Required]
        [StringLength(100)]
        public string OdometerReading { get; set; }

        [Required]
        [StringLength(100)]
        public string DriverName { get; set; }

        [Required]
        public string ReportedBy { get; set; }

        public string CheckedBy { get; set; }

        public int ApprovedBy { get; set; }

        public virtual List<RequisitionItem> RequisitionItems { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual User ApprovedByObj { get; set; }
    }
}