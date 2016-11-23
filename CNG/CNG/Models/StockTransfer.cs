﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class StockTransfer
    {
        public StockTransfer()
        {
            StockTransferItems = new List<StockTransferItem>();
        }

        [Key]
        public int Id { get; set; }
        public string No { get; set; }
        public DateTime Date { get; set; }

        public int TransferFrom { get; set; }

        [DisplayName("Prepared By")]
        public int PreparedBy { get; set; }

        [DisplayName("Approved By")]
        public int ApprovedBy { get; set; }

        [ForeignKey("PreparedBy")]
        public virtual User PreparedByObj { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual User ApprovedByObj { get; set; }

        public virtual List<StockTransferItem> StockTransferItems { get; set; }
    }
}