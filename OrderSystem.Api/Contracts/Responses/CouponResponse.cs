namespace OrderSystem.Api.Contracts.Responses;

public record CouponResponse(
    Guid Id,
    string Code,
    decimal DiscountAmount,
    string Currency,
    DateTime ValidFrom,
    DateTime ValidUntil,
    int MaxUses,
    int TimesUsed,
    bool IsActive
);