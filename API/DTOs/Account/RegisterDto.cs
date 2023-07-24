using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        [StringLength(15, ErrorMessage = "First name can be maximum {1} charecters")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "Last name can be maximum {1} charecters")]
        public string LastName { get; set; }
        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        [StringLength(15, MinimumLength =6, ErrorMessage = "First name must be minimum {2}, and maximum (1) charecters")]
        public string Password { get; set; }
    }
}
