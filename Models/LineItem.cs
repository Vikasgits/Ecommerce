using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.Models
{
    public class LineItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = "Value must be atleast 1 and at max 50")]
        public int Quantity { get; set; }

        public double LineItemPrice { get; set; } = 0;

        public Order? Order { get; set; }
        [ForeignKey("Order")]
        public Guid OrderId { get; set; }

        public Product? Product { get; set; }
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        

        public double CalculateLineItemCost()
        {
            return Product.Price * Quantity;
        }
    }
}
