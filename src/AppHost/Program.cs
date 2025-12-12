var builder = DistributedApplication.CreateBuilder(args);

// Ajouter Redis comme ressource
var redis = builder.AddRedis("redis");

// Ajouter Orders.Api avec référence à Redis
var ordersApi = builder.AddProject<Projects.Orders_Api>("orders-api")
                       .WithReference(redis);

// Ajouter Billing.Worker avec référence à Redis
var billingWorker = builder.AddProject<Projects.Billing_Worker>("billing-worker")
                           .WithReference(redis);

// Ajouter Gateway.Api avec référence vers Orders.Api
var gatewayApi = builder.AddProject<Projects.Gateway_Api>("gateway-api")
                        .WithReference(ordersApi);

builder.Build().Run();

