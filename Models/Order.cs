using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.Models
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }

        [Required]
        public DateOnly Date {  get; set; }

        public double TotalPrice { get; set; }

        public Status? Status { get; set; }
        [ForeignKey("Status")]
        public int StatusId { get; set; }

        public User? User { get; set; }
        [ForeignKey("User")]

        public Guid UserId { get; set; }

        public List<LineItem>? LineItems { get; set; } = new List<LineItem>();

        public double CalculateOrderPrice()
        {
            double totalPrice = 0;
            foreach (LineItem lineItem in LineItems)
            {
                totalPrice += lineItem.CalculateLineItemCost();
            }
            return totalPrice;
        }







    }
}
