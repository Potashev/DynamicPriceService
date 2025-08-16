using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public class EditProductCommandHandler
	: IRequestHandler<EditProductCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly ICurrentUserService _currentUserService;

	public EditProductCommandHandler(DynamicPriceCoreContext context, IMapper mapper, ICurrentUserService currentUserService)
		=> (_context, _mapper, _currentUserService) = (context, mapper, currentUserService);

	public async Task<int> Handle(EditProductCommand request, CancellationToken cancellationToken)
	{
		var manager = await _currentUserService.GetCurrentUserAsync();

		var updatedProductVm = request.ProductVm;

		var product = await _context.Products
			.FirstOrDefaultAsync(p => p.ProductId == updatedProductVm.ProductId && p.CompanyId == manager.CompanyId, cancellationToken);

		if (product != null)
		{
			_mapper.Map(updatedProductVm, product);

			_context.Update(product);
			await _context.SaveChangesAsync(cancellationToken);
		}
		return product.ProductId;
	}
}
