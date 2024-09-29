using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOS
{
    public class UserUpdateDto
    {
        public Guid UserId { get; set; }
        public string? FilePath { get; set; }

        public IFormFile? ImageFile { get; set; }

        [Required]
        [MaxLength(25)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The Name field can only contain alphabets.")]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(10)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "The Name field can only contain alphabets.")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}
