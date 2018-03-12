using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace CNGCashier.Models
{
    public class Driver
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Lastname")]
        public string LastName { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Firstname")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Middlename")]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("License Number")]
        public string LicenseNumber { get; set; }

    }
}