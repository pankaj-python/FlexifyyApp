using System.ComponentModel.DataAnnotations;

namespace FlexifyyApp.DTOs
{
    public class AuthDTOs
    {
    }

    public class RegisterDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }

        [RegularExpression("Bulk|Cut|Maintain",
            ErrorMessage = "Goal must be Bulk, Cut, or Maintain")]
        public string? Goal { get; set; }
    }


    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }


    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
