using AutoMapper;
using DynamicPriceCore.Models;

namespace DynamicPriceCore.MediatR.ViewModels;

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
