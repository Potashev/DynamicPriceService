using AutoMapper;
using DynamicPriceService.Data;
using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.ProductEntity.Commands;

public class EditProductCommandHandler
	: IRequestHandler<EditProductCommand>
{
	private readonly DynamicPriceServiceContext _context;
	private readonly IMapper _mapper;

	public EditProductCommandHandler(DynamicPriceServiceContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task Handle(EditProductCommand request, CancellationToken cancellationToken)
	{
		var productVm = request.ProductVm;
		var product = _mapper.Map<Product>(productVm);

		//Looks dirty
		var modelFields =
			(from p in _context.Product
			 where p.ProductId == productVm.ProductId
			 select new
			 {
				 p.Company,
				 LastSellTile = p.LastSellTime
			 }).FirstOrDefault();

		product.Company = modelFields.Company;
		product.LastSellTime = modelFields.LastSellTile;

		_context.Update(product);
		_context.SaveChanges();
	}
}
