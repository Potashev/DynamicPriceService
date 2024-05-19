using DynamicPriceService.MediatR.ViewModel;
using MediatR;

namespace DynamicPriceService.MediatR.ProductEntity.Commands;

public class EditProductCommand : IRequest
{
    public ProductViewModel ProductVm { get; set; }
    public EditProductCommand(ProductViewModel productVm) => ProductVm = productVm;
}
