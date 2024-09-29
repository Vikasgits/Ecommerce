using E_commerce.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOS
{
    public class OrderDto
    {
        [Key]
        public Guid OrderId { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        public double TotalPrice { get; set; }

       
        public Guid UserId { get; set; }

        public int StatusId { get; set; }


    }
}
