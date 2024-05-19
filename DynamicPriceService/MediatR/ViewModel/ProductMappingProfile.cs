using AutoMapper;
using DynamicPriceService.Models;

namespace DynamicPriceService.MediatR.ViewModel;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile() =>
        CreateMap<Product, ProductViewModel>()
            .ReverseMap();
}
