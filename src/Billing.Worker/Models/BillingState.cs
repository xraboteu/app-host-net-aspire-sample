namespace Billing.Worker.Models;

public record BillingState(
    string OrderId,
    string CustomerId,
    decimal Amount,
    DateTime BilledAt,
    string Status = "Billed"
);

