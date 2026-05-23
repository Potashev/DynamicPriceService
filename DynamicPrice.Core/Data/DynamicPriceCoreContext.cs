using DynamicPrice.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Data;

public class DynamicPriceCoreContext : DbContext
{
	public DynamicPriceCoreContext(DbContextOptions<DynamicPriceCoreContext> options)
		: base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<ActiveCompany>(b =>
		{
			b.HasKey(ac => ac.Id);

			b.Property(ac => ac.StartedAt)
				.IsRequired();

			b.Property(ac => ac.LastMonitoring);

			b.HasOne(ac => ac.Company)
			 .WithOne()
			 .HasForeignKey<ActiveCompany>(ac => ac.Id)
			 .OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<OrderItem>()
			.HasOne(oi => oi.Product)
			.WithMany()
			.HasForeignKey(oi => oi.ProductId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<OrderItem>()
			.HasOne(oi => oi.Order)
			.WithMany(o => o.OrderItems)
			.HasForeignKey(oi => oi.OrderId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<CartItem>()
			.HasOne(ci => ci.Product)
			.WithMany()
			.HasForeignKey(ci => ci.ProductId)
			.OnDelete(DeleteBehavior.NoAction);

		modelBuilder.Entity<CartItem>()
			.HasOne(ci => ci.Cart)
			.WithMany(c => c.CartItems)
			.HasForeignKey(ci => ci.CartId)
			.OnDelete(DeleteBehavior.Cascade);
	}

	public DbSet<Company> Companies { get; set; } = default!;
	public DbSet<ActiveCompany> ActiveCompanies { get; set; } = default!;
	public DbSet<Product> Products { get; set; } = default!;
	public DbSet<PriceRule> PriceRules { get; set; } = default!;
	public DbSet<PriceDynamic> PriceDynamics { get; set; } = default!;
	public DbSet<Cart> Carts { get; set; } = default!;
	public DbSet<CartItem> CartItems { get; set; } = default!;
	public DbSet<Order> Orders { get; set; } = default!;
	public DbSet<OrderItem> OrderItems { get; set; } = default!;
}
