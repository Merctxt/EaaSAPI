using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto |
                               ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddPolicy("fixed", httpContext =>
    {
        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(remoteIp, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 30,
            Window = TimeSpan.FromMinutes(60),
            QueueLimit = 0
        });
    });
});

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOpenApi();


var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.WithTitle("SMTP API")
           .ForceDarkMode()
           .WithTheme(ScalarTheme.DeepSpace)
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
           .AddPreferredSecuritySchemes("https");
});


if (!app.Environment.IsDevelopment())
{
    app.UseRateLimiter();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
