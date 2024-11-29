using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
namespace API.Dtos.Account
{
    public class ConfirmEmailDto
    {
        [Required]
        [NotNull]
        public string? Email { get; set; }

        [Required]
        [NotNull]
        public string? Token { get; set; }
    }
}