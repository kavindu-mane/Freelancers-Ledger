using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace API.Dtos.Account
{
    public class LoginDto
    {
        [Required]
        [NotNull]
        public string? Email { get; set; }

        [Required]
        [NotNull]
        public string? Password { get; set; }
    }
}