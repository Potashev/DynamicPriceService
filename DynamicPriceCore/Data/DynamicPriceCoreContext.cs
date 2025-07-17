using Microsoft.EntityFrameworkCore;
using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DynamicPriceCore.Data;
public class DynamicPriceCoreContext : IdentityDbContext<ApplicationUser>
{
	public DynamicPriceCoreContext(DbContextOptions<DynamicPriceCoreContext> options)
		: base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<OrderItem>()
			.HasOne(oi => oi.Product)
			.WithMany()
			.HasForeignKey(oi => oi.ProductId)
			.OnDelete(DeleteBehavior.Restrict); // или NoAction, смысл один: каскада нет

		modelBuilder.Entity<OrderItem>()
			.HasOne(oi => oi.Order)
			.WithMany(o => o.OrderItems)
			.HasForeignKey(oi => oi.OrderId)
			.OnDelete(DeleteBehavior.Cascade); // Order удаляется -> OrderItems удаляются

		modelBuilder.Entity<CartItem>()
			.HasOne(ci => ci.Product)
			.WithMany()
			.HasForeignKey(ci => ci.ProductId)
			.OnDelete(DeleteBehavior.NoAction); // или Restrict, аналогично

		modelBuilder.Entity<CartItem>()
			.HasOne(ci => ci.Cart)
			.WithMany(c => c.CartItems)
			.HasForeignKey(ci => ci.CartId)
			.OnDelete(DeleteBehavior.Cascade); // корзина удаляется -> её items удаляются


		//modelBuilder.Entity<OrderProduct>()
		//	.HasKey(op => op.Id);

		//modelBuilder.Entity<OrderProduct>()
		//	.HasOne(op => op.Order)
		//	.WithMany(o => o.OrderProducts)
		//	.HasForeignKey(op => op.OrderId)
		//	.OnDelete(DeleteBehavior.NoAction);

		//modelBuilder.Entity<OrderProduct>()
		//	.HasOne(op => op.Product)
		//	.WithMany(p => p.OrderProducts)
		//	.HasForeignKey(op => op.ProductId)
		//	.OnDelete(DeleteBehavior.NoAction);
	}

	public DbSet<Company> Companies { get; set; } = default!;
	public DbSet<Product> Products { get; set; } = default!;
	public DbSet<PriceRule> PriceRules { get; set; } = default!;
	public DbSet<PriceDynamic> PriceDynamics { get; set; } = default!;
	public DbSet<Cart> Carts { get; set; } = default;
	public DbSet<Order> Orders { get; set; } = default;
	public DbSet<CartItem> CartItems { get; set; } = default;
	public DbSet<OrderItem> OrderItems { get; set; } = default;
	//public DbSet<OrderProduct> OrderProducts { get; set; } = default;	//todo: obsolete - remove
}
