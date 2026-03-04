using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public class EditProductCommandHandler
	: IRequestHandler<EditProductCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public EditProductCommandHandler(
		DynamicPriceCoreContext context,
		IMapper mapper,
		IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<int> Handle(
		EditProductCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await _userService.GetRequiredCurrentUserAsync();

		var updatedProductVm = request.ProductVm;

		var product = await _context.Products
			.FirstOrDefaultAsync(p => p.ProductId == updatedProductVm.ProductId && p.CompanyId == manager.CompanyId, cancellationToken)
			?? throw new NotFoundException("Product not found.");

		_mapper.Map(updatedProductVm, product);

		_context.Update(product);
		await _context.SaveChangesAsync(cancellationToken);

		return product.ProductId;
	}
}
