namespace OrderSystem.Api.Contracts.Responses;

public record CustomerResponse(
    Guid Id,
    string Name,
    string Email
);