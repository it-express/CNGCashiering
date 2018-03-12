using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CNGCashier.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Make { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [StringLength(100)]
        public string Model { get; set; }

        [StringLength(100)]
        [DisplayName("CN No")]
        public string CnNo { get; set; }

        [StringLength(100)]
        [DisplayName("Plate No")]
        public string LicenseNo { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Engine No")]
        public string EngineNo { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Chasis No")]
        public string ChasisNo { get; set; }

        [Required]
        [StringLength(100)]
        public string Color { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}