namespace DynamicPriceCore.Models;

public class CompanyUser
{
    public int CompanyId { get; set; }
    public Company Company { get; set; }
    public string UserId { get; set; }
    //todo: add user property later
}
