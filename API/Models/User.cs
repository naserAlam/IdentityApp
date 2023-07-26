using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class User : IdentityUser
    {
        public User(
            int code,
            string firstName,
            string lastName,
            string userName,
            string email,
            bool emailConfirmed)
        {
            Code = code;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            UserName = userName;
            EmailConfirmed = emailConfirmed;
        }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public int Code { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
    }
}
