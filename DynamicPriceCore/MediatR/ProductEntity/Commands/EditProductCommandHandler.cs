using AutoMapper;
using DynamicPriceCore.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public class EditProductCommandHandler
	: IRequestHandler<EditProductCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public EditProductCommandHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<int> Handle(EditProductCommand request, CancellationToken cancellationToken)
	{
		var updatedProductVm = request.ProductVm;
		var product = await _context.Products
			.FirstOrDefaultAsync(p => p.ProductId == updatedProductVm.ProductId, cancellationToken);

		if (product != null)
		{
			_mapper.Map(updatedProductVm, product);

			_context.Update(product);
			await _context.SaveChangesAsync(cancellationToken);
		}
		return product.ProductId;
	}
}
