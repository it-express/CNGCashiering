using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CNGCashier.Models
{
    public class UserType
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Description { get; set; }
    }
}