using DynamicPriceService.Data;
using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.ProductEntity.Commands;

public class EditProductCommandHandler
	: IRequestHandler<EditProductCommand, int>
{
	private readonly DynamicPriceServiceContext _context;

	public EditProductCommandHandler(DynamicPriceServiceContext context)
		=> _context = context;

	public async Task<int> Handle(EditProductCommand request, CancellationToken cancellationToken)
	{
        var product = request.Product;

        //Looks dirty
        var modelFields =
			(from p in _context.Product
			 where p.ProductId == product.ProductId
			 select new
			 {
				 p.Company,
				 LastSellTile = p.LastSellTime
			 }).FirstOrDefault();

		product.Company = modelFields.Company;
		product.LastSellTime = modelFields.LastSellTile;

		_context.Update(product);
		_context.SaveChanges();

        return product.ProductId;
	}
}
