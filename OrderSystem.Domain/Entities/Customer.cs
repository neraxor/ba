using OrderSystem.Domain.Exceptions;
using OrderSystem.Domain.ValueObjects;

namespace OrderSystem.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; }

    private Customer() { }

    public Customer(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name is required");

        if (name.Length < 2)
            throw new DomainException("Customer name must be at least 2 characters");

        if (name.Length > 200)
            throw new DomainException("Customer name is too long");

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = new Email(email);
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("Customer name is required");

        if (newName.Length < 2)
            throw new DomainException("Customer name must be at least 2 characters");

        Name = newName.Trim();
    }

    public void UpdateEmail(string newEmail)
    {
        Email = new Email(newEmail);
    }
}