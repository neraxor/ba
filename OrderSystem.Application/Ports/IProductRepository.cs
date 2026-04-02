using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Ports;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<List<Product>> GetAllAsync();
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
}