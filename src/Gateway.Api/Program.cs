using Gateway.Api.Models;
using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Configuration HttpClient pour Orders.Api (via Aspire)
// Aspire résoudra automatiquement "orders-api" via la référence WithReference
builder.Services.AddHttpClient<OrdersApiClient>(client =>
{
    // Le nom "orders-api" correspond au nom du projet dans AppHost
    client.BaseAddress = new Uri("http://orders-api");
});

// Configuration Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuration Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Endpoint proxy vers Orders.Api
app.MapPost("/orders", async (OrderRequest request, OrdersApiClient ordersApi) =>
{
    try
    {
        var response = await ordersApi.CreateOrderAsync(request);
        return response;
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem(
            detail: $"Erreur lors de la communication avec Orders.Api: {ex.Message}",
            statusCode: 503
        );
    }
})
.WithName("CreateOrder")
.WithOpenApi();

app.Run();

// Client HTTP pour Orders.Api
public class OrdersApiClient
{
    private readonly HttpClient _httpClient;

    public OrdersApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult> CreateOrderAsync(OrderRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/orders", request);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return Results.Accepted(response.Headers.Location?.ToString(), content);
        }
        
        var errorContent = await response.Content.ReadAsStringAsync();
        return Results.Problem(
            detail: $"Erreur depuis Orders.Api: {errorContent}",
            statusCode: (int)response.StatusCode
        );
    }
}

