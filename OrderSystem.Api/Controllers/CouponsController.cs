using Microsoft.AspNetCore.Mvc;
using OrderSystem.Api.Contracts.Requests;
using OrderSystem.Api.Contracts.Responses;
using OrderSystem.Application.Services;

namespace OrderSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouponsController : ControllerBase
{
    private readonly CouponService _couponService;

    public CouponsController(CouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CouponResponse>>> GetAll()
    {
        var coupons = await _couponService.GetAllCouponsAsync();
        return Ok(coupons.Select(MapToResponse));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CouponResponse>> GetById(Guid id)
    {
        var coupon = await _couponService.GetCouponAsync(id);
        if (coupon == null)
            return NotFound();

        return Ok(MapToResponse(coupon));
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<CouponResponse>> GetByCode(string code)
    {
        var coupon = await _couponService.GetCouponByCodeAsync(code);
        if (coupon == null)
            return NotFound();

        return Ok(MapToResponse(coupon));
    }

    [HttpPost]
    public async Task<ActionResult<CouponResponse>> Create(CreateCouponRequest request)
    {
        var coupon = await _couponService.CreateCouponAsync(
            request.Code,
            request.DiscountAmount,
            request.ValidFrom,
            request.ValidUntil,
            request.MaxUses
        );
        return CreatedAtAction(nameof(GetById), new { id = coupon.Id }, MapToResponse(coupon));
    }

    [HttpGet("code/{code}/validate")]
    public async Task<ActionResult<bool>> Validate(string code)
    {
        var isValid = await _couponService.ValidateCouponAsync(code);
        return Ok(isValid);
    }

    private static CouponResponse MapToResponse(Domain.Entities.Coupon coupon)
    {
        return new CouponResponse(
            coupon.Id,
            coupon.Code,
            coupon.DiscountAmount.Amount,
            coupon.DiscountAmount.Currency,
            coupon.ValidFrom,
            coupon.ValidUntil,
            coupon.MaxUses,
            coupon.TimesUsed,
            coupon.IsActive
        );
    }
}