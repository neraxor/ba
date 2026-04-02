namespace OrderSystem.Api.Contracts.Requests;

public record CreateCouponRequest(string Code, decimal DiscountAmount, DateTime ValidFrom, DateTime ValidUntil, int MaxUses);