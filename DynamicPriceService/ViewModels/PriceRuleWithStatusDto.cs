namespace DynamicPriceService.ViewModels;

public class PriceRuleWithStatus
{
    public PriceRuleViewModel PriceRuleVm { get; set; }
    public bool IsActive { get; set; }
    public PriceRuleWithStatus(PriceRuleViewModel priceRuleVm, bool isActive)
    {
        PriceRuleVm = priceRuleVm;
        IsActive = isActive;
    }
}
