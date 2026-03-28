using System.Linq;
using System.Net;
using System.Net.Http.Json;
using BackEnd.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SampleApp.Tests;

public class ProductApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(Microsoft.EntityFrameworkCore.DbContextOptions<BackEnd.Data.AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<BackEnd.Data.AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BackEnd.Data.AppDbContext>();
                db.Database.EnsureCreated();

                if (!db.Products.Any())
                {
                    db.Products.AddRange(new []
                    {
                        new BackEnd.Models.Product { Name = "Notebook", Price = 2999.90m },
                        new BackEnd.Models.Product { Name = "Teclado", Price = 199.90m },
                        new BackEnd.Models.Product { Name = "Mouse", Price = 99.90m }
                    });
                    db.SaveChanges();
                }
            });
        }
    }

    [Fact]
    public async Task GetProducts_ReturnsOkAndSeedData()
    {
        var response = await _client.GetAsync("/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        products.Should().NotBeNull();
        products.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task PostPutDelete_ProductLifeCycle_Works()
    {
        var newProduct = new Product { Name = "Teste", Price = 10.0m };
        var createResponse = await _client.PostAsJsonAsync("/products", newProduct);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<Product>();
        created.Should().NotBeNull();
        created!.Id.Should().BeGreaterThan(0);
        created.Name.Should().Be(newProduct.Name);

        created.Price.Should().Be(newProduct.Price);

        created.Name = "Teste Atualizado";
        var updateResponse = await _client.PutAsJsonAsync($"/products/{created.Id}", created);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/products/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await getResponse.Content.ReadFromJsonAsync<Product>();
        updated!.Name.Should().Be("Teste Atualizado");

        var deleteResponse = await _client.DeleteAsync($"/products/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getAfterDelete = await _client.GetAsync($"/products/{created.Id}");
        getAfterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
