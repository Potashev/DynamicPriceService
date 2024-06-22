using DynamicPriceCore.Models;

namespace DynamicPriceCore.Services;

/// <summary>
/// Список компаний для мониторинга "простоя".
/// </summary>
public class ActiveCompaniesService : IActiveCompaniesService
{
	private List<Company> _activeCompanies = new List<Company>();

	public IEnumerable<Company> GetActiveCompanies() => _activeCompanies;
	public void Add(Company company) => _activeCompanies.Add(company);
	public void Remove(Company company) => _activeCompanies.RemoveAll(c => c.CompanyId == company.CompanyId);
	public bool IsActive(Company company) => _activeCompanies.Any(c => c.CompanyId == company.CompanyId);
}

public interface IActiveCompaniesService
{
	IEnumerable<Company> GetActiveCompanies();
	void Add(Company company);
	void Remove(Company company);
	bool IsActive(Company company);
}