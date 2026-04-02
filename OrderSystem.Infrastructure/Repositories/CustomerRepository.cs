using Microsoft.EntityFrameworkCore;
using OrderSystem.Application.Ports;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Persistence;

namespace OrderSystem.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly OrderSystemDbContext _context;

    public CustomerRepository(OrderSystemDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == email.ToLowerInvariant());
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }
}