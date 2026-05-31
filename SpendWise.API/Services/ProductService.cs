using SpendWise.API.DTOs.Products;
using SpendWise.API.Models;
using SpendWise.API.Repositories.Interfaces;
using SpendWise.API.Services.Interfaces;

namespace SpendWise.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();

            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductDto?> GetByCodeAsync(string code)
        {
            code = code.Trim();

            var product = await _productRepository.GetByCodeAsync(code);

            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductDto> CreateAsync(
            CreateProductDto dto,
            string userId,
            bool isAdmin)
                {
            var product = new Product
            {
                Code = dto.Code,
                Name = dto.Name,
                UnitPrice = dto.UnitPrice,
                CategoryId = dto.CategoryId,
                UserId = isAdmin ? null : userId
            };

            var createdProduct = await _productRepository.CreateAsync(product);

            var productWithCategory = await _productRepository.GetByIdAsync(createdProduct.Id);

            return MapToDto(productWithCategory!);
        }
        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return false;

            product.Code = dto.Code;
            product.Name = dto.Name;
            product.UnitPrice = dto.UnitPrice;
            product.CategoryId = dto.CategoryId;

            await _productRepository.UpdateAsync(product);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return false;

            await _productRepository.DeleteAsync(product);

            return true;
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                UnitPrice = product.UnitPrice,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty
            };
        }
    }
}