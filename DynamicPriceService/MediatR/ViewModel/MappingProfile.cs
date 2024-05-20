using AutoMapper;
using DynamicPriceService.Models;

namespace DynamicPriceService.MediatR.ViewModel;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<Product, ProductViewModel>()
			.ReverseMap();
		CreateMap<PriceRule, PriceRuleViewModel>()
			.ReverseMap();
	}

}
