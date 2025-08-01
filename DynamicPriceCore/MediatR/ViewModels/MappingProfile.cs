using AutoMapper;
using DynamicPriceCore.Models;

namespace DynamicPriceCore.MediatR.ViewModels;

//todo: remove from VM folder
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
		CreateMap<Order, OrderInfoViewModel>()
			.ReverseMap();
		CreateMap<OrderItem, OrderItemViewModel>()
			.ReverseMap();
		CreateMap<Cart, CartViewModel>()
			.ReverseMap();
		CreateMap<CartItem, CartItemViewModel>()
			.ReverseMap();
	}
}
