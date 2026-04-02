using OrderSystem.Domain.Exceptions;
using OrderSystem.Domain.ValueObjects;

namespace OrderSystem.Domain.Entities;

public class Coupon
{
    public const int MinCodeLength = 3;
    public const int MaxCodeLength = 20;
    public const decimal MaxDiscountAmount = 1000m;

    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public Money DiscountAmount { get; private set; }
    public Money? MinimumOrderValue { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidUntil { get; private set; }
    public int MaxUses { get; private set; }
    public int TimesUsed { get; private set; }

    public bool IsActive => TimesUsed < MaxUses;
    public bool IsExpired(DateTime currentDate) => currentDate > ValidUntil;
    public bool HasStarted(DateTime currentDate) => currentDate >= ValidFrom;

    private Coupon() { }

    public Coupon(string code, Money discountAmount, DateTime validFrom, DateTime validUntil, int maxUses, Money? minimumOrderValue = null)
    {
        ValidateCode(code);
        ValidateDiscountAmount(discountAmount);
        ValidateDateRange(validFrom, validUntil);
        ValidateMaxUses(maxUses);
        ValidateMinimumOrderValue(minimumOrderValue, discountAmount);

        Id = Guid.NewGuid();
        Code = code.ToUpperInvariant().Trim();
        DiscountAmount = discountAmount;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
        MaxUses = maxUses;
        MinimumOrderValue = minimumOrderValue;
        TimesUsed = 0;
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Coupon code is required");

        var trimmedCode = code.Trim();

        if (trimmedCode.Length < MinCodeLength)
            throw new DomainException($"Coupon code must be at least {MinCodeLength} characters");

        if (trimmedCode.Length > MaxCodeLength)
            throw new DomainException($"Coupon code cannot exceed {MaxCodeLength} characters");

        if (!trimmedCode.All(c => char.IsLetterOrDigit(c)))
            throw new DomainException("Coupon code can only contain letters and numbers");
    }

    private static void ValidateDiscountAmount(Money discountAmount)
    {
        if (discountAmount.Amount <= 0)
            throw new DomainException("Discount amount must be greater than zero");

        if (discountAmount.Amount > MaxDiscountAmount)
            throw new DomainException($"Discount amount cannot exceed {MaxDiscountAmount}");
    }

    private static void ValidateDateRange(DateTime validFrom, DateTime validUntil)
    {
        if (validUntil <= validFrom)
            throw new DomainException("ValidUntil must be after ValidFrom");

        if (validUntil <= validFrom.AddHours(1))
            throw new DomainException("Coupon must be valid for at least 1 hour");
    }

    private static void ValidateMaxUses(int maxUses)
    {
        if (maxUses <= 0)
            throw new DomainException("MaxUses must be at least 1");

        if (maxUses > 10000)
            throw new DomainException("MaxUses cannot exceed 10000");
    }

    private static void ValidateMinimumOrderValue(Money? minimumOrderValue, Money discountAmount)
    {
        if (minimumOrderValue.HasValue)
        {
            if (minimumOrderValue.Value.Amount <= 0)
                throw new DomainException("Minimum order value must be greater than zero");

            if (minimumOrderValue.Value.Amount < discountAmount.Amount)
                throw new DomainException("Minimum order value cannot be less than discount amount");
        }
    }

    public bool IsValidAt(DateTime dateTime)
    {
        return IsActive && HasStarted(dateTime) && !IsExpired(dateTime);
    }

    public bool CanBeAppliedTo(Money orderTotal, DateTime currentDate)
    {
        if (!IsValidAt(currentDate))
            return false;

        if (MinimumOrderValue.HasValue && orderTotal.Amount < MinimumOrderValue.Value.Amount)
            return false;

        return true;
    }

    public void Use()
    {
        if (!IsActive)
            throw new DomainException("Coupon has reached maximum uses");

        TimesUsed++;
    }

    public void ExtendValidity(DateTime newValidUntil)
    {
        if (newValidUntil <= ValidUntil)
            throw new DomainException("New validity date must be after current validity date");

        ValidUntil = newValidUntil;
    }
}