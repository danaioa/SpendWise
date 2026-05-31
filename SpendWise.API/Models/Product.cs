namespace SpendWise.API.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }
    }
}