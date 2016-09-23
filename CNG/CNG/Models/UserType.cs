using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class UserType
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Description { get; set; }
    }
}