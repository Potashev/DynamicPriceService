using Microsoft.AspNetCore.Identity;

namespace DynamicPriceCore.Models;

public class Customer : IdentityUser
{
    public int CustomerId { get; set; }	//todo: obsolete - remove
    public decimal Balance { get; set; }
}
