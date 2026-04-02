using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Ports;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<List<Order>> GetByCustomerIdAsync(Guid customerId);
    Task<List<Order>> GetAllAsync();
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}