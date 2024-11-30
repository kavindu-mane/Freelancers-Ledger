using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace API.Dtos.Account
{
    public class RegisterDto
    {
        [Required]
        [NotNull]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [NotNull]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [MaxLength(16, ErrorMessage = "Password must not be more than 16 characters long")]
        public string? Password { get; set; }

        [Required]
        [NotNull]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

         [Required]
        [NotNull]
        [MinLength(3 , ErrorMessage = "First name must be at least 3 characters")]
        [MaxLength(30 , ErrorMessage = "First name must not exceed 30 characters")]
        public string? FirstName { get; set; }

        [Required]
        [NotNull]
        [MinLength(3 , ErrorMessage = "Last name must be at least 3 characters")]
        [MaxLength(30 , ErrorMessage = "Last name must not exceed 30 characters")]
        public string? LastName { get; set; }
    }
}
