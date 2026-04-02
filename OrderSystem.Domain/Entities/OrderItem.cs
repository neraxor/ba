using OrderSystem.Domain.Exceptions;
using OrderSystem.Domain.ValueObjects;

namespace OrderSystem.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public Money TotalPrice => UnitPrice * Quantity;

    private OrderItem() { }

    public OrderItem(Product product, int quantity)
    {
        if (product == null)
            throw new DomainException("Product is required");

        if (quantity <= 0)
            throw new DomainException("Quantity must be at least 1");

        Id = Guid.NewGuid();
        ProductId = product.Id;
        ProductName = product.Name;
        UnitPrice = product.Price;
        Quantity = quantity;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new DomainException("Quantity must be at least 1");

        Quantity = newQuantity;
    }
}