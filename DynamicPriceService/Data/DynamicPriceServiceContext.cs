using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DynamicPriceService.Models;

namespace DynamicPriceService.Data;
public class DynamicPriceServiceContext : DbContext
{
    public DynamicPriceServiceContext(DbContextOptions<DynamicPriceServiceContext> options)
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
}
