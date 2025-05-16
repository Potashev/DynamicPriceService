using Microsoft.EntityFrameworkCore;
using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DynamicPriceCore.Data;
public class DynamicPriceCoreContext : IdentityDbContext
{
	public DynamicPriceCoreContext(DbContextOptions<DynamicPriceCoreContext> options)
		: base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		//modelBuilder.Entity<CompanyUser>()
		//	.HasKey(cu => new { cu.CompanyId, cu.UserId });

		modelBuilder.Entity<OrderProduct>()
			.HasKey(op => op.Id);

		modelBuilder.Entity<OrderProduct>()
			.HasOne(op => op.Order)
			.WithMany(o => o.OrderProducts)
			.HasForeignKey(op => op.OrderId)
			.OnDelete(DeleteBehavior.NoAction);

		modelBuilder.Entity<OrderProduct>()
			.HasOne(op => op.Product)
			.WithMany(p => p.OrderProducts)
			.HasForeignKey(op => op.ProductId)
			.OnDelete(DeleteBehavior.NoAction);
	}

	public DbSet<Company> Companies { get; set; } = default!;
	//public DbSet<CompanyUser> CompanyUsers { get; set; } = default;
	public DbSet<Product> Products { get; set; } = default!;
	public DbSet<PriceRule> PriceRules { get; set; } = default!;
	public DbSet<PriceDynamic> PriceDynamics { get; set; } = default!;
	public DbSet<Order> Orders { get; set; } = default;
	public DbSet<OrderProduct> OrderProducts { get; set; } = default;
	public DbSet<Customer> Customers { get; set; }
	public DbSet<Manager> Managers { get; set; }
}
