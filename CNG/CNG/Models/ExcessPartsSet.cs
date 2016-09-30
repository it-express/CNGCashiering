using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ExcessPartsSet
    {
        public ExcessPartsSet() {
            ExcessPartsSetItems = new List<ExcessPartsSetItem>();
        }

        [Key]
        public int Id { get; set; }
        public string No { get; set; }
        public DateTime Date { get; set; }

        [DisplayName("Prepared By")]
        public int PreparedBy { get; set; }

        [DisplayName("ApprovedBy")]
        public int ApprovedBy { get; set; }

        public virtual List<ExcessPartsSetItem> ExcessPartsSetItems { get; set; }

        [ForeignKey("PreparedBy")]
        public virtual User PreparedByObj { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual User ApprovedByObj { get; set; }
    }
}