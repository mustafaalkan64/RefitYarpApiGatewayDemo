using Refit; // Ensure this using directive is present
using ConsumerApi.Services;
using Carter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddCarter();
builder.Services.AddEndpointsApiExplorer();

// Register Refit client for IProductApi
builder.Services.AddRefitClient<IProductApi>() // Ensure Refit.HttpClientFactory package is installed
   .ConfigureHttpClient(c =>
   {
       c.BaseAddress = new Uri("http://localhost:5001"); // ProductApi URL
   });

var app = builder.Build();

app.MapCarter();

app.Run();