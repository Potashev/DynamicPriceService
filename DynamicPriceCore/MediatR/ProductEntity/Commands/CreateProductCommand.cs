using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public class CreateProductCommand : IRequest<int>
{
	public ProductViewModel ProductVm { get; set; }
	public string UserId { get; set; }
	public CreateProductCommand(ProductViewModel productVm, string userId)
		=> (ProductVm, UserId) = (productVm, userId);
}
