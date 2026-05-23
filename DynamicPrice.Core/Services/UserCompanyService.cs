using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Services;

public class UserCompanyService(
	DynamicPriceCoreContext dbContext) : IUserCompanyService
{
	public async Task<Company?> GetCompanyAsync(ApplicationUser user)
	{
		if (!user.CompanyId.HasValue)
			return null;

		return await dbContext.Companies
			.FirstOrDefaultAsync(c => c.Id == user.CompanyId);
	}
}

public interface IUserCompanyService
{
	Task<Company?> GetCompanyAsync(ApplicationUser user);
}
