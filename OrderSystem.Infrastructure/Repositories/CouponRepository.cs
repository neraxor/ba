using Microsoft.EntityFrameworkCore;
using OrderSystem.Application.Ports;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Persistence;

namespace OrderSystem.Infrastructure.Repositories;

public class CouponRepository : ICouponRepository
{
    private readonly OrderSystemDbContext _context;

    public CouponRepository(OrderSystemDbContext context)
    {
        _context = context;
    }

    public async Task<Coupon?> GetByIdAsync(Guid id)
    {
        return await _context.Coupons.FindAsync(id);
    }

    public async Task<Coupon?> GetByCodeAsync(string code)
    {
        return await _context.Coupons
            .FirstOrDefaultAsync(c => c.Code == code.ToUpperInvariant());
    }

    public async Task<List<Coupon>> GetAllAsync()
    {
        return await _context.Coupons.ToListAsync();
    }

    public async Task AddAsync(Coupon coupon)
    {
        await _context.Coupons.AddAsync(coupon);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Coupon coupon)
    {
        _context.Coupons.Update(coupon);
        await _context.SaveChangesAsync();
    }
}