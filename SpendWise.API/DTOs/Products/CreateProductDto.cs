using System.ComponentModel.DataAnnotations;

namespace SpendWise.API.DTOs.Products
{
    public class CreateProductDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 999999)]
        public decimal UnitPrice { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}