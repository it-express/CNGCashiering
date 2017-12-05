using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CNG.Models
{
    public class ItemHistory
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int OldItemTypeId { get; set; }

        [Required]
        public int NewItemTypeId { get; set; }

        public int CompanyId { get; set; }

        public DateTime Date { get; set; }



    }


}