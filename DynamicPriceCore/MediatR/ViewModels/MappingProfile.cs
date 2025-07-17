using AutoMapper;
using DynamicPriceCore.Models;

namespace DynamicPriceCore.MediatR.ViewModels;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<Product, ProductInfoViewModel>()
			.ReverseMap();
		CreateMap<Product, ProductViewModel>()
			.ReverseMap();
		CreateMap<PriceRule, PriceRuleViewModel>()
			.ReverseMap();
		CreateMap<Order, OrderViewModel>()
			.ReverseMap();
		CreateMap<Order, OrderInfoViewModel>()
			.ReverseMap();

		//todo: make mapping for OrderItem and CartItem?
		//CreateMap<OrderProduct, OrderProductViewModel>()
		//	.ReverseMap();
	}
}
