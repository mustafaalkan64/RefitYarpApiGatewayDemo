using Carter;
using ProductApi.Models;
using System.ComponentModel.DataAnnotations;

public class ProductModule : ICarterModule
{
    private static readonly List<Product> _products = [
        new() { Id = Guid.NewGuid(), Name = "Printer", Price = 10000 },
        new() { Id = Guid.NewGuid(), Name = "Notebook", Price = 5000 },
        new() { Id = Guid.NewGuid(), Name = "Kitap", Price = 200 }
    ];

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", () => Results.Ok(_products));

        app.MapGet("/api/products/{id}", (Guid id) =>
        {
            var product = _products.Find(p => p.Id == id);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        });

        app.MapPost("/api/products", (Product product) =>
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(product);
            if (!Validator.TryValidateObject(product, context, validationResults, true))
            {
                return Results.ValidationProblem(validationResults.ToDictionary(
                    v => v.MemberNames.FirstOrDefault() ?? "",
                    v => new[] { v.ErrorMessage ?? "Invalid" }
                ));
            }
            product.Id = Guid.NewGuid();
            _products.Add(product);
            return Results.Created($"/api/products/{product.Id}", product);
        });
    }
}
