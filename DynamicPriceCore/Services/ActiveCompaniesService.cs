using DynamicPriceCore.Models;

namespace DynamicPriceCore.Services;

/// <summary>
/// Список компаний для мониторинга "простоя".
/// </summary>
public class ActiveCompaniesService : IActiveCompaniesService
{
	private List<Company> _activeCompanies = new List<Company>();

	private List<Company> _companiesToAdd = new List<Company>();
	private List<Company> _companiesToRemove = new List<Company>();

	public IEnumerable<Company> GetActiveCompanies() => _activeCompanies;
	public void AddRequest(Company company) => _companiesToAdd.Add(company);
	public void RemoveRequest(Company company) => _companiesToRemove.Add(company);

	//public void RemoveRequest(Company company) => _companiesToRemove.RemoveAll(c => c.CompanyId == company.CompanyId);
	public bool IsActive(Company company) => _activeCompanies.Any(c => c.CompanyId == company.CompanyId);

	//todo: looks like not good. The issue changing collection when foreach in reduceprice job. Make better solution or check to avoid collision
	public void HandleRequests()
	{
		_activeCompanies.AddRange(_companiesToAdd);
		_activeCompanies.RemoveAll(c => _companiesToRemove.Any(r => r.CompanyId == c.CompanyId));
		_companiesToAdd.Clear();
		_companiesToRemove.Clear();
	}
}

public interface IActiveCompaniesService
{
	IEnumerable<Company> GetActiveCompanies();
	void AddRequest(Company company);
	void RemoveRequest(Company company);
	bool IsActive(Company company);
	void HandleRequests();
}