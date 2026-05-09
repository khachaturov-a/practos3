using Microsoft.EntityFrameworkCore;
using Practos3.Models;

namespace Practos3.Data;

public class ShopContext : DbContext
{
    public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Chetkas>  Chetkas    { get; set; } = null!;

    // Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chetkas>()
            .HasIndex(c => c.Name)
            .IsUnique();
        
        modelBuilder.Entity<Chetkas>()
            .HasOne(c => c.Category)
            .WithMany(cat => cat.Chetkas)
            .HasForeignKey(c => c.CategoryId);

        modelBuilder.Entity<Chetkas>().ToTable("Chetkas");
    }
}

