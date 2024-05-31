using System.ComponentModel.DataAnnotations;

namespace DynamicPriceService.Models;

public class CompanyUser
{
    //[Key]
    public int CompanyId { get; set; }
    public Company Company { get; set; }

    //[Key]
    public string UserId { get; set; }
    //todo: add user property later
}
