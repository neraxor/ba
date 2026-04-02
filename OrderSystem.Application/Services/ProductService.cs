using OrderSystem.Application.Ports;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Exceptions;
using OrderSystem.Domain.ValueObjects;

namespace OrderSystem.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product> CreateProductAsync(string name, string description, decimal price, int stock)
    {
        var product = new Product(name, description, new Money(price), stock);
        await _productRepository.AddAsync(product);
        return product;
    }

    public async Task<Product?> GetProductAsync(Guid id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task UpdatePriceAsync(Guid productId, decimal newPrice)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new DomainException("Product not found");

        product.UpdatePrice(new Money(newPrice));
        await _productRepository.UpdateAsync(product);
    }

    public async Task AddStockAsync(Guid productId, int quantity)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new DomainException("Product not found");

        product.AddStock(quantity);
        await _productRepository.UpdateAsync(product);
    }
}