using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CNG.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CNG.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(150)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(150)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name ="User Type")]
        public int UserTypeId { get; set; }

        [Required]
        [Display(Name = "User Level")]
        public int UserLevel { get; set; }

        [Required]
        [Display(Name = "General Manager")]
        public int GeneralManagerId { get; set; }

        public string FullName {
            get { return FirstName + " " + LastName; }
        }

        public virtual UserType UserType { get; set; }
        public virtual User GeneralManager { get; set; }
    }

    public enum EUserType
    {
        User = 1,
        GeneralManager = 2
    }
}