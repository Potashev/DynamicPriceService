using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public class EditProductCommand : IRequest<int>
{
	public ProductViewModel ProductVm { get; set; }
	public EditProductCommand(ProductViewModel productVm) => ProductVm = productVm;
}
