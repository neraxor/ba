namespace OrderSystem.Api.Contracts.Requests;

public record AddItemRequest(Guid ProductId, int Quantity);