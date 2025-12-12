namespace Billing.Worker.Models;

public record OrderCreatedEvent(string OrderId, string CustomerId, decimal Amount);

