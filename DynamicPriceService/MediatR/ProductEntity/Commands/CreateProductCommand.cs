using DynamicPriceService.MediatR.ViewModel;
using MediatR;

namespace DynamicPriceService.MediatR.ProductEntity.Commands;

public class CreateProductCommand : IRequest
{
	public ProductViewModel ProductVm { get; set; }
	public CreateProductCommand(ProductViewModel productVm) => ProductVm = productVm;
}
