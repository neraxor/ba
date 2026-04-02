using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Ports;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByEmailAsync(string email);
    Task<List<Customer>> GetAllAsync();
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
}