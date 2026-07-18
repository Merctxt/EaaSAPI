using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.WithTitle("SMTP API")
           .ForceDarkMode()
           .WithTheme(ScalarTheme.DeepSpace);
});

app.UseAuthorization();

app.MapControllers();

app.Run();
