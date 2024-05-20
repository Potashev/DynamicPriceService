using DynamicPriceService.MediatR.ViewModel;
using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleDetailsQuery : IRequest<PriceRuleViewModel>;
