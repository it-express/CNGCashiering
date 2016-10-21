using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CNG.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Contact Person")]
        public string ContactPerson { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Contact No")]
        public string ContactNo { get; set; }

        [Required]
        public bool Active { get; set; }


        [Required]
        public string Prefix { get; set; }
    }
}