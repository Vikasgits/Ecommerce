using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOS
{
    public class UserLoginDto
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public string Recaptcha {  get; set; }

    }
}
