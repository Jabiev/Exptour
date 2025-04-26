using Exptour.API;
using Exptour.Infrastructure.Middlewares;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.ConfigureServices();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());

app.UseSerilogRequestLogging();

app.UseHttpLogging();

app.MapGet("/", handler =>
{
    handler.Response.Redirect("/swagger/index.html", permanent: false);
    return Task.CompletedTask;
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    var authName = context?.User?.Identity?.IsAuthenticated is not null || true 
        ? context?.User?.Identity?.Name
        : "unknown";
    LogContext.PushProperty("user_name", authName);

    await next();
});

app.MapControllers();

app.Run();
