using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOS
{
    public class PostUserDto
    {

       
        public string? FilePath { get; set; }

        public IFormFile? ImageFile { get; set; }

        [Required]
        [MaxLength(25)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "The Name field can only contain alphabets and spaces.")]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Please enter a valid email address.")]

        public string Email { get; set; }

        [Required]
        [MinLength(10), MaxLength(10)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "The Name field can only contain alphabets.")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        public bool IsAdmin { get; set; } = false;


    }
}
