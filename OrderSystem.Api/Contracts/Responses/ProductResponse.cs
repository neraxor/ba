namespace OrderSystem.Api.Contracts.Responses;

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int Stock
);