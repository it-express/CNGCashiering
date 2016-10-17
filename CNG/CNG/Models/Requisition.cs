using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

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
        [DisplayName("Job Order No")]
        public string JobOrderNo { get; set; }

        [Required]
        [DisplayName("Job Order Date")]
        public DateTime JobOrderDate { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Unit Plate No")]
        public string UnitPlateNo { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Odometer Reading")]
        public string OdometerReading { get; set; }

        [StringLength(100)]
        [DisplayName("Driver Name")]
        public string DriverName { get; set; }

        [Required]
        [DisplayName("Reporteed By")]
        public string ReportedBy { get; set; }

        [DisplayName("Checked By")]
        public string CheckedBy { get; set; }

        [DisplayName("Approved By")]
        public int ApprovedBy { get; set; }

        public virtual List<RequisitionItem> RequisitionItems { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual User ApprovedByObj { get; set; }
    }
}