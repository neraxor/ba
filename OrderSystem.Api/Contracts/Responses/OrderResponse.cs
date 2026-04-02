namespace OrderSystem.Api.Contracts.Responses;

public record OrderResponse(
    Guid Id,
    Guid CustomerId,
    string Status,
    DateTime CreatedAt,
    DateTime? PaidAt,
    decimal Subtotal,
    decimal? Discount,
    decimal Total,
    List<OrderItemResponse> Items
);

public record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice
);