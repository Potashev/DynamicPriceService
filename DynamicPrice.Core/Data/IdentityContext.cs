using DynamicPrice.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Data;

public class IdentityContext : IdentityDbContext<ApplicationUser>
{
	public IdentityContext(DbContextOptions<IdentityContext> options)
		: base(options) { }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
	}
}
