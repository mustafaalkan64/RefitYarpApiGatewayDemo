using System.ComponentModel.DataAnnotations;
using Carter;
using ConsumerApi.Models;
using ConsumerApi.Services;

public class ProductProxyModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/productproxy", async (IProductApi api) =>
        {
            var products = await api.GetProducts();
            return Results.Ok(products);
        });

        app.MapGet("/api/productproxy/{id}", async (int id, IProductApi api) =>
        {
            var product = await api.GetProduct(id);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        });

        app.MapPost("/api/productproxy", async (Product product, IProductApi api) =>
        {
            var created = await api.PostProduct(product);
            return Results.Created($"/api/productproxy/{created.Id}", created);
        });
    }
}
