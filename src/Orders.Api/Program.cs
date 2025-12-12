using Dapr.Client;
using Orders.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration Dapr
builder.Services.AddDaprClient();

// Configuration Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuration Dapr CloudEvents
app.UseCloudEvents();
app.MapSubscribeHandler();

// Configuration Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Endpoint pour créer une commande
app.MapPost("/orders", async (OrderRequest request, DaprClient daprClient) =>
{
    var orderId = Guid.NewGuid().ToString();
    var evt = new OrderCreatedEvent(orderId, request.CustomerId, request.Amount);
    
    // Publier l'événement dans Dapr Pub/Sub
    await daprClient.PublishEventAsync("pubsub", "order.created", evt);
    
    return Results.Accepted($"/orders/{orderId}", new { orderId });
})
.WithName("CreateOrder")
.WithOpenApi();

app.Run();

