using System.Globalization;
using Application;
using FluentValidation;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog(config => config
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<AppDbContext>().Database;
    if (database.IsRelational())
        await database.MigrateAsync();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler(handler => handler.Run(async context =>
{
    var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var response = error switch
    {
        ValidationException validation => Results.ValidationProblem(
            validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()),
            statusCode: StatusCodes.Status400BadRequest),
        _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Unexpected error"),
    };

    await response.ExecuteAsync(context);
}));

app.MapControllers();

app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Ok(new { service = "CleanArchitectureSample", status = "healthy" }));

app.Run();

public partial class Program;
