using BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Notebook", Price = 2999.90m },
            new Product { Id = 2, Name = "Teclado", Price = 199.90m },
            new Product { Id = 3, Name = "Mouse", Price = 99.90m }
        );
    }
}
