using OrderSystem.Application.Ports;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Exceptions;
using OrderSystem.Domain.ValueObjects;

namespace OrderSystem.Application.Services;

public class CouponService
{
    private readonly ICouponRepository _couponRepository;
    private readonly IClock _clock;

    public CouponService(ICouponRepository couponRepository, IClock clock)
    {
        _couponRepository = couponRepository;
        _clock = clock;
    }

    public async Task<Coupon> CreateCouponAsync(
        string code,
        decimal discountAmount,
        DateTime validFrom,
        DateTime validUntil,
        int maxUses,
        decimal? minimumOrderValue = null)
    {
        var existing = await _couponRepository.GetByCodeAsync(code);
        if (existing != null)
            throw new DomainException("Coupon code already exists");

        var minOrderValue = minimumOrderValue.HasValue
            ? new Money(minimumOrderValue.Value)
            : (Money?)null;

        var coupon = new Coupon(
            code,
            new Money(discountAmount),
            validFrom,
            validUntil,
            maxUses,
            minOrderValue);

        await _couponRepository.AddAsync(coupon);
        return coupon;
    }

    public async Task<Coupon?> GetCouponAsync(Guid id)
    {
        return await _couponRepository.GetByIdAsync(id);
    }

    public async Task<Coupon?> GetCouponByCodeAsync(string code)
    {
        return await _couponRepository.GetByCodeAsync(code);
    }

    public async Task<List<Coupon>> GetAllCouponsAsync()
    {
        return await _couponRepository.GetAllAsync();
    }

    public async Task<List<Coupon>> GetActiveCouponsAsync()
    {
        var all = await _couponRepository.GetAllAsync();
        var now = _clock.UtcNow;
        return all.Where(c => c.IsValidAt(now)).ToList();
    }

    public async Task<bool> ValidateCouponAsync(string code)
    {
        var coupon = await _couponRepository.GetByCodeAsync(code);
        if (coupon == null)
            return false;

        return coupon.IsValidAt(_clock.UtcNow);
    }

    public async Task<bool> CanApplyCouponAsync(string code, decimal orderTotal)
    {
        var coupon = await _couponRepository.GetByCodeAsync(code);
        if (coupon == null)
            return false;

        return coupon.CanBeAppliedTo(new Money(orderTotal), _clock.UtcNow);
    }

    public async Task ExtendCouponValidityAsync(Guid couponId, DateTime newValidUntil)
    {
        var coupon = await _couponRepository.GetByIdAsync(couponId);
        if (coupon == null)
            throw new DomainException("Coupon not found");

        coupon.ExtendValidity(newValidUntil);
        await _couponRepository.UpdateAsync(coupon);
    }
}