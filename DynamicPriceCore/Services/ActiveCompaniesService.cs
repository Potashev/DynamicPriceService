using Azure.Identity;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;

namespace DynamicPriceCore.Services;

/// <summary>
/// Список компаний для мониторинга "простоя".
/// </summary>
public class ActiveCompaniesService : IActiveCompaniesService
{
	//private List<Company> _activeCompanies = new List<Company>();

	//private List<Company> _companiesToAdd = new List<Company>();
	//private List<Company> _companiesToRemove = new List<Company>();

	//private readonly DynamicPriceCoreContext _context;
	private readonly IServiceProvider _serviceProvider;

	private List<int> _activeCompanies = new List<int>();
	private List<int> _companiesToAdd = new List<int>();
	private List<int> _companiesToRemove = new List<int>();

	public ActiveCompaniesService(IServiceProvider serviceProvider)
	{
		//_context = context;
		_serviceProvider = serviceProvider;
	}

	public IEnumerable<Company> GetActiveCompanies()
	{
		//var companies = _context.Companies
		//	.Where(c => _activeCompanies.Contains(c.CompanyId))
		//	.ToList();
		//return companies;


		if (!_activeCompanies.Any()) return Enumerable.Empty<Company>();

		//bad
		using var scope = _serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
		return context.Companies
			.Where(c => _activeCompanies.Contains(c.CompanyId))
			.ToList();
	}
	public void AddRequest(int companyId) => _companiesToAdd.Add(companyId);
	//public void RemoveRequest(Company company) => _companiesToRemove.Add(company);
	public void RemoveRequest(int companyId) => _companiesToRemove.Add(companyId);

	//public void RemoveRequest(Company company) => _companiesToRemove.RemoveAll(c => c.CompanyId == company.CompanyId);
	//public bool IsActive(Company company) => _activeCompanies.Any(c => c.CompanyId == company.CompanyId);
	public bool IsActive(int companyId) => _activeCompanies.Any(cid => cid == companyId);

	//todo: looks like not good. The issue changing collection when foreach in reduceprice job. Make better solution or check to avoid collision
	public void HandleRequests()
	{
		//todo: check
		_activeCompanies.AddRange(_companiesToAdd);
		_activeCompanies.RemoveAll(cid => _companiesToRemove.Any(rid => rid == cid));
		_companiesToAdd.Clear();
		_companiesToRemove.Clear();
	}
}

public interface IActiveCompaniesService
{
	IEnumerable<Company> GetActiveCompanies();
	//void AddRequest(Company company);
	//void RemoveRequest(Company company);
	//bool IsActive(Company company);

	//IEnumerable<int> GetActiveCompanies();
	bool IsActive(int companyId);
	void AddRequest(int companyId);
	void RemoveRequest(int companyId);
	void HandleRequests();
}