using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOS
{
    public class PostProductDto
    {
        public string? FilePath { get; set; }

        public IFormFile? ImageFile { get; set; }


        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "The Name field can only contain alphabets,numbers and spaces.")]
        public string Name { get; set; }

        [MaxLength(150)]
        [RegularExpression(@"^[a-zA-Z0-9\s\-,().&]+$", ErrorMessage = "The Description field can only contain alphabets and numbers.")]
        public string Description { get; set; }

        public string Category { get; set; }

        public string SubCategory { get; set; }

        [Required]
        [Range(0, 100000, ErrorMessage = "Price must be non-negative.")]
        public double Price { get; set; }
    }
}
