﻿using System;
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

        [Required]
        public string No { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        [DisplayName("Job Order No")]
        public string JobOrderNo { get; set; }

        [Required]
        [DisplayName("Job Order Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyy}", ApplyFormatInEditMode = true)]
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
        [DisplayName("Reported By")]
        public string ReportedBy { get; set; }

        [DisplayName("Checked By")]
        public string CheckedBy { get; set; }

        [DisplayName("Approved By")]
        public int ApprovedBy { get; set; }

        public int CompanyId { get; set; }
        public bool Checked { get; set; }
        public bool Approved { get; set; }
        public virtual List<RequisitionItem> RequisitionItems { get; set; }

        public virtual Company Company { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual User ApprovedByObj { get; set; }
    }
}