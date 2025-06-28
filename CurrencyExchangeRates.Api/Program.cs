using CurrencyExchangeRates.Infrastructure.Configuration;
using CurrencyExchangeRates.Infrastructure.DepedencyInjection;
using Microsoft.AspNetCore.Diagnostics;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<EcbGatewayOptions>(builder.Configuration.GetSection("EcbGateway"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientIp,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            });
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", token);
    };
});
var app = builder.Build();

app.UseExceptionHandler(builder =>
{
    builder.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = feature?.Error;

        logger.LogError(exception, "Unhandled exception occurred: {Message}", exception?.Message);

        context.Response.ContentType = "application/json";

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred."
        });
        
    });
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRateLimiter();
app.MapControllers();

app.Run();
