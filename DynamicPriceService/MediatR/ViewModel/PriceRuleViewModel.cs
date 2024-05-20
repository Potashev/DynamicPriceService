using DynamicPriceService.Models;

namespace DynamicPriceService.MediatR.ViewModel;

public class PriceRuleViewModel
{
    public int PriceRuleId { get; set; }
    public int Increase { get; set; }
    public int Reduction { get; set; }
    public TimeSpan? NoSellTime { get; set; }
}
