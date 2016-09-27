﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class UserAccount
    {
        [Key]
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}