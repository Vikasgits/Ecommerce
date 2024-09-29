using E_commerce.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOS
{
    public class LineItemDto
    {
        public Guid Id { get; set; }
        [Required]
        [Range(1, 50, ErrorMessage = "Value must be atleast 1 and at max 50")]
        public int Quantity { get; set; }

        public double LineItemPrice { get; set; } = 0;

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }


        
    }
}
