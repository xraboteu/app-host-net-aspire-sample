namespace Orders.Api.Models;

public record OrderCreatedEvent(string OrderId, string CustomerId, decimal Amount);

