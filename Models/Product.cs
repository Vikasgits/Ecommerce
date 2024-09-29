using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "The Name field can only contain alphabets,numbers and spaces")]
        public string Name { get; set; }

        [MaxLength(150)]
        [RegularExpression(@"^[a-zA-Z0-9\s\-,().&]+$", ErrorMessage = "The Description field can only contain alphabets, numbers and other special characters.")]
        public string Description { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required]
        [Range(0, 100000, ErrorMessage = "Price must be non-negative.")]
        public double Price { get; set; }

        [Required]
        [Range(0, 250, ErrorMessage = "InStock must be non-negative.")]
        public int InStock { get; set; } = 0;

        [Required]
        [RegularExpression(@"^[a-zA-Z\s']+$", ErrorMessage = "The Category field can only contain alphabets, spaces, and single quotes.")]
        public string Category { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s']+$", ErrorMessage = "The SubCategory field can only contain alphabets, spaces, and single quotes.")]
        public string SubCategory { get; set; }



        public bool IsAvailable { get; set; } = true;
    }
}
