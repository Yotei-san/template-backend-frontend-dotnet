using BackEnd.Data;
using BackEnd.Models;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi(options =>
{
    // current workaround for port forwarding in codespaces
    // https://github.com/dotnet/aspnetcore/issues/57332
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Servers = [];
        return Task.CompletedTask;
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=products.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () =>
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/products", async (AppDbContext db) =>
    await db.Products.AsNoTracking().ToListAsync())
    .WithName("GetProducts");

app.MapGet("/products/{id:int}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

app.MapPost("/products", async (Product newProduct, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(newProduct.Name) || newProduct.Price <= 0)
    {
        return Results.BadRequest("Nome e preço válidos são exigidos.");
    }

    db.Products.Add(newProduct);
    await db.SaveChangesAsync();

    return Results.Created($"/products/{newProduct.Id}", newProduct);
});

app.MapPut("/products/{id:int}", async (int id, Product update, AppDbContext db) =>
{
    var existing = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
    if (existing is null) return Results.NotFound();

    existing.Name = update.Name;
    existing.Price = update.Price;
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/products/{id:int}", async (int id, AppDbContext db) =>
{
    var existing = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
    if (existing is null) return Results.NotFound();

    db.Products.Remove(existing);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public partial class Program { }

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
