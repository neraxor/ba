using OrderSystem.Domain.Enums;
using OrderSystem.Domain.Exceptions;
using OrderSystem.Domain.ValueObjects;

namespace OrderSystem.Domain.Entities;

public class Order
{
    public const decimal MinimumOrderValue = 10m;
    public const int MaxItemsPerOrder = 50;

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public Money? Discount { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public Money Subtotal => _items.Count == 0
        ? Money.Zero
        : _items.Aggregate(Money.Zero, (sum, item) => sum + item.TotalPrice);

    public Money Total => Discount.HasValue ? Subtotal - Discount.Value : Subtotal;

    public int TotalItemCount => _items.Sum(i => i.Quantity);

    private Order() { }

    public Order(Customer customer, DateTime createdAt)
    {
        if (customer == null)
            throw new DomainException("Customer is required");

        Id = Guid.NewGuid();
        CustomerId = customer.Id;
        Status = OrderStatus.Pending;
        CreatedAt = createdAt;
    }

    public void AddItem(Product product, int quantity)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot modify a non-pending order");

        if (product == null)
            throw new DomainException("Product is required");

        if (quantity <= 0)
            throw new DomainException("Quantity must be at least 1");

        var newTotalItems = TotalItemCount + quantity;
        if (newTotalItems > MaxItemsPerOrder)
            throw new DomainException($"Cannot exceed {MaxItemsPerOrder} items per order");

        var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            _items.Add(new OrderItem(product, quantity));
        }
    }

    public void RemoveItem(Guid productId)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot modify a non-pending order");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new DomainException("Item not found in order");

        _items.Remove(item);
    }

    public void UpdateItemQuantity(Guid productId, int newQuantity)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot modify a non-pending order");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new DomainException("Item not found in order");

        var quantityDifference = newQuantity - item.Quantity;
        var newTotalItems = TotalItemCount + quantityDifference;

        if (newTotalItems > MaxItemsPerOrder)
            throw new DomainException($"Cannot exceed {MaxItemsPerOrder} items per order");

        item.UpdateQuantity(newQuantity);
    }

    public void ApplyDiscount(Money discount)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot apply discount to a non-pending order");

        if (Discount.HasValue)
            throw new DomainException("A discount has already been applied to this order");

        if (discount.Amount > Subtotal.Amount)
            throw new DomainException("Discount cannot exceed subtotal");

        if (discount.Amount <= 0)
            throw new DomainException("Discount must be greater than zero");

        Discount = discount;
    }

    public void RemoveDiscount()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot modify a non-pending order");

        Discount = null;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be confirmed");

        if (!_items.Any())
            throw new DomainException("Cannot confirm an empty order");

        if (Total.Amount < MinimumOrderValue)
            throw new DomainException($"Minimum order value is {MinimumOrderValue} {Total.Currency}");

        Status = OrderStatus.Confirmed;
    }

    public void MarkAsPaid(DateTime paidAt)
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainException("Only confirmed orders can be marked as paid");

        Status = OrderStatus.Paid;
        PaidAt = paidAt;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Paid)
            throw new DomainException("Only paid orders can be shipped");

        Status = OrderStatus.Shipped;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new DomainException("Only shipped orders can be delivered");

        Status = OrderStatus.Delivered;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            throw new DomainException("Cannot cancel a shipped or delivered order");

        if (Status == OrderStatus.Cancelled)
            throw new DomainException("Order is already cancelled");

        Status = OrderStatus.Cancelled;
    }
}