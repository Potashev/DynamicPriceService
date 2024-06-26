using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DynamicPriceCore.Models;

namespace DynamicPriceCore.Data;
public class DynamicPriceCoreContext : DbContext
{
	public DynamicPriceCoreContext(DbContextOptions<DynamicPriceCoreContext> options)
		: base(options)
	{
		Database.EnsureCreated();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<CompanyUser>()
			.HasKey(cu => new { cu.CompanyId, cu.UserId });
	}

	public DbSet<Company> Companies { get; set; } = default!;
	public DbSet<CompanyUser> CompanyUsers { get; set; } = default;
	public DbSet<Product> Products { get; set; } = default!;
	public DbSet<PriceRule> PriceRules { get; set; } = default!;
	public DbSet<Order> Orders { get; set; } = default;
	public DbSet<Customer> Customers { get; set; } = default;
}
