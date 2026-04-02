using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Ports;

public interface ICouponRepository
{
    Task<Coupon?> GetByIdAsync(Guid id);
    Task<Coupon?> GetByCodeAsync(string code);
    Task<List<Coupon>> GetAllAsync();
    Task AddAsync(Coupon coupon);
    Task UpdateAsync(Coupon coupon);
}