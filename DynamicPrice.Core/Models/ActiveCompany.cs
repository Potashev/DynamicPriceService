namespace DynamicPrice.Core.Models;

public class ActiveCompany
{
	public int CompanyId { get; set; }
	public DateTime StartedAt { get; set; } = DateTime.UtcNow;
	public DateTime? LastMonitoring { get; set; }
	public Company? Company { get; set; }
}