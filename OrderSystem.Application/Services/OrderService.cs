using OrderSystem.Application.Ports;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Exceptions;

namespace OrderSystem.Application.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IClock _clock;

    public OrderService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        ICouponRepository couponRepository,
        IPaymentGateway paymentGateway,
        IClock clock)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _couponRepository = couponRepository;
        _paymentGateway = paymentGateway;
        _clock = clock;
    }

    public async Task<Order> CreateOrderAsync(Guid customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            throw new DomainException("Customer not found");

        var order = new Order(customer, _clock.UtcNow);
        await _orderRepository.AddAsync(order);
        return order;
    }

    public async Task AddItemToOrderAsync(Guid orderId, Guid productId, int quantity)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new DomainException("Order not found");

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new DomainException("Product not found");

        if (product.Stock < quantity)
            throw new DomainException("Insufficient stock");

        order.AddItem(product, quantity);
        await _orderRepository.UpdateAsync(order);
    }

    public async Task RemoveItemFromOrderAsync(Guid orderId, Guid productId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new DomainException("Order not found");

        order.RemoveItem(productId);
        await _orderRepository.UpdateAsync(order);
    }

    public async Task ApplyCouponAsync(Guid orderId, string couponCode)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new DomainException("Order not found");

        var coupon = await _couponRepository.GetByCodeAsync(couponCode);
        if (coupon == null)
            throw new DomainException("Coupon not found");

        if (!coupon.IsValidAt(_clock.UtcNow))
            throw new DomainException("Coupon is not valid");

        order.ApplyDiscount(coupon.DiscountAmount);
        coupon.Use();

        await _orderRepository.UpdateAsync(order);
        await _couponRepository.UpdateAsync(coupon);
    }

    public async Task ConfirmOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new DomainException("Order not found");

        // Reserve stock
        foreach (var item in order.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new DomainException($"Product {item.ProductId} not found");

            product.RemoveStock(item.Quantity);
            await _productRepository.UpdateAsync(product);
        }

        order.Confirm();
        await _orderRepository.UpdateAsync(order);
    }

    public async Task PayOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new DomainException("Order not found");

        var result = await _paymentGateway.ProcessPaymentAsync(orderId, order.Total);
        if (!result.Success)
            throw new DomainException($"Payment failed: {result.ErrorMessage}");

        order.MarkAsPaid(_clock.UtcNow);
        await _orderRepository.UpdateAsync(order);
    }

    public async Task CancelOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new DomainException("Order not found");

        // Restore stock if order was confirmed
        if (order.Status == Domain.Enums.OrderStatus.Confirmed ||
            order.Status == Domain.Enums.OrderStatus.Paid)
        {
            foreach (var item in order.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.AddStock(item.Quantity);
                    await _productRepository.UpdateAsync(product);
                }
            }
        }

        order.Cancel();
        await _orderRepository.UpdateAsync(order);
    }
}