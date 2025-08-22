using Carter;

var builder = WebApplication.CreateBuilder(args);

// Add Carter
builder.Services.AddCarter();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapCarter();

app.Run();
