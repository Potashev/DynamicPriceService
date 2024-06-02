using System.ComponentModel.DataAnnotations;

namespace DynamicPriceCore.Models;

public class CompanyUser
{
    //public int CompanyUserId { get; set; }
    //[Key]
    public int CompanyId { get; set; }
    public Company Company { get; set; }

    //[Key]
    public string UserId { get; set; }
    //todo: add user property later
}
