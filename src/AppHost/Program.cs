var builder = DistributedApplication.CreateBuilder(args);

// Ajouter Redis comme ressource
var redis = builder.AddRedis("redis");

// Ajouter Orders.Api avec référence à Redis
var ordersApi = builder.AddProject("orders-api", "../Orders.Api/Orders.Api.csproj")
                       .WithReference(redis);

// Ajouter Billing.Worker avec référence à Redis
var billingWorker = builder.AddProject("billing-worker", "../Billing.Worker/Billing.Worker.csproj")
                           .WithReference(redis);

// Ajouter Gateway.Api avec référence vers Orders.Api
var gatewayApi = builder.AddProject("gateway-api", "../Gateway.Api/Gateway.Api.csproj")
                        .WithReference(ordersApi);

builder.Build().Run();

