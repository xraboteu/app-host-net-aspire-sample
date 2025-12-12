namespace Orders.Api.Models;

public record OrderRequest(string CustomerId, decimal Amount);

