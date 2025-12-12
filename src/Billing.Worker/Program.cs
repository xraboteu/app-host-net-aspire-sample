using Dapr.Client;
using Billing.Worker.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration Dapr
builder.Services.AddDaprClient();

var app = builder.Build();

// Configuration Dapr CloudEvents
app.UseCloudEvents();
app.MapSubscribeHandler();

// Endpoint pour recevoir les événements order.created via Dapr Pub/Sub
app.MapPost("/orders/created", async (OrderCreatedEvent evt, DaprClient daprClient) =>
{
    Console.WriteLine($"[Billing] Facturation de la commande {evt.OrderId} pour le client {evt.CustomerId} (montant: {evt.Amount:C})");
    
    // Créer l'état de facturation
    var billingState = new BillingState(
        OrderId: evt.OrderId,
        CustomerId: evt.CustomerId,
        Amount: evt.Amount,
        BilledAt: DateTime.UtcNow,
        Status: "Billed"
    );
    
    // Sauvegarder l'état dans Dapr State Store
    await daprClient.SaveStateAsync("statestore", $"order-{evt.OrderId}", billingState);
    
    Console.WriteLine($"[Billing] État de facturation sauvegardé pour la commande {evt.OrderId}");
    
    return Results.Ok();
})
.WithTopic("pubsub", "order.created")
.WithName("ProcessOrderCreated")
.WithOpenApi();

// Endpoint pour récupérer l'état de facturation
app.MapGet("/billing/{orderId}", async (string orderId, DaprClient daprClient) =>
{
    var state = await daprClient.GetStateAsync<BillingState>("statestore", $"order-{orderId}");
    
    if (state == null)
    {
        return Results.NotFound(new { message = $"Aucune facturation trouvée pour la commande {orderId}" });
    }
    
    return Results.Ok(state);
})
.WithName("GetBillingState")
.WithOpenApi();

app.Run();

