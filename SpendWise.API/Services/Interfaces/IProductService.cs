using SpendWise.API.DTOs.Products;

namespace SpendWise.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();

        Task<ProductDto?> GetByIdAsync(int id);

        Task<ProductDto?> GetByCodeAsync(string code);

        Task<ProductDto> CreateAsync(
            CreateProductDto dto,
            string userId,
            bool isAdmin);

        Task<bool> UpdateAsync(int id, UpdateProductDto dto);

        Task<bool> DeleteAsync(int id);
    }
}