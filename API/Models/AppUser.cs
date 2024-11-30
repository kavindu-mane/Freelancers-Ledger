using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace API.Models
{
    public class AppUser : IdentityUser { 

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
