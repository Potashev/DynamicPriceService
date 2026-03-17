using AutoMapper;
using DynamicPrice.Core.Models;
using DynamicPrice.Shared.Contracts.Requests;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;

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

		CreateMap<RegisterUserRequest, ApplicationUser>();

		CreateMap<ApplicationUser, ManagerInfoViewModel>()
			.ForMember(d => d.Name, o => o.MapFrom(s => s.UserName));
	}
}
