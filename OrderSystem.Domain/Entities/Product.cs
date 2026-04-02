using OrderSystem.Domain.Exceptions;
using OrderSystem.Domain.ValueObjects;

namespace OrderSystem.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public int Stock { get; private set; }

    private Product() { }

    public Product(string name, string description, Money price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required");

        if (price.Amount <= 0)
            throw new DomainException("Price must be greater than zero");

        if (stock < 0)
            throw new DomainException("Stock cannot be negative");

        Id = Guid.NewGuid();
        Name = name;
        Description = description ?? string.Empty;
        Price = price;
        Stock = stock;
    }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new DomainException("Price must be greater than zero");

        Price = newPrice;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive");

        Stock += quantity;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive");

        if (quantity > Stock)
            throw new DomainException("Insufficient stock");

        Stock -= quantity;
    }
}