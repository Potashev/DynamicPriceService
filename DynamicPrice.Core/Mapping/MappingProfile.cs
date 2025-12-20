using AutoMapper;
using DynamicPrice.Core.Models;
using DynamicPrice.Shared.Contracts.ViewModels;

namespace DynamicPrice.Core.Mapping;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<Company, CompanyViewModel>()
			.ReverseMap();
		CreateMap<Product, ProductInfoViewModel>()
			.ReverseMap();
		CreateMap<Product, ProductViewModel>()
			.ReverseMap();
		CreateMap<PriceRule, PriceRuleViewModel>()
			.ReverseMap();
		CreateMap<PriceDynamic, PriceDynamicViewModel>()
			.ReverseMap();
		CreateMap<Order, OrderViewModel>()
			.ReverseMap();
		CreateMap<OrderItem, OrderItemViewModel>()
			.ReverseMap();
		CreateMap<Cart, CartViewModel>()
			.ReverseMap();
		CreateMap<CartItem, CartItemViewModel>()
			.ReverseMap();
	}
}
