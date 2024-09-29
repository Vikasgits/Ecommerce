using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.Models
{
    public class FavouriteItem
    {
        [Key]
        public Guid ItemID { get; set; }

        public Product? Product { get; set; }
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public User? User { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }


    }
}
