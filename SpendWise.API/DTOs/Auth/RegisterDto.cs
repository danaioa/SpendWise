using System.ComponentModel.DataAnnotations;

namespace SpendWise.API.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Range(1, 1000000)]
        public decimal MonthlyIncome { get; set; }
    }
}