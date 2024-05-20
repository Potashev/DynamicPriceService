using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DynamicPriceService.Models;

namespace DynamicPriceService.Data;
public class DynamicPriceServiceContext : DbContext
{
	public DynamicPriceServiceContext(DbContextOptions<DynamicPriceServiceContext> options)
		: base(options)
	{
		//todo: for development - remove
		//Database.EnsureDeleted();
		Database.EnsureCreated();
	}

	public DbSet<Company> Company { get; set; } = default!;
	public DbSet<Product> Product { get; set; } = default!;
	public DbSet<PriceRule> PriceRule { get; set; } = default!;
}
