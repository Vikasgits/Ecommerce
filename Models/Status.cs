using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models
{
    public class Status
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        public string StatusName { get; set; }

        public List<Order> Orders { get; set; }
    }

}
