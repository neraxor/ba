using OrderSystem.Domain.Exceptions;

namespace OrderSystem.Domain.ValueObjects;

public readonly record struct Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email is required");

        if (!value.Contains('@') || !value.Contains('.'))
            throw new DomainException("Invalid email format");

        if (value.Length > 254)
            throw new DomainException("Email is too long");

        Value = value.ToLowerInvariant();
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}