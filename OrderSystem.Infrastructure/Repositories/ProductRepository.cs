using Microsoft.EntityFrameworkCore;
using OrderSystem.Application.Ports;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Persistence;

namespace OrderSystem.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly OrderSystemDbContext _context;

    public ProductRepository(OrderSystemDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}