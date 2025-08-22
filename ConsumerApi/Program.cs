using Refit; // Ensure this using directive is present
using ConsumerApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Register Refit client for IProductApi
builder.Services.AddRefitClient<IProductApi>() // Ensure Refit.HttpClientFactory package is installed
   .ConfigureHttpClient(c =>
   {
       c.BaseAddress = new Uri("http://localhost:5001"); // ProductApi URL
   });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();