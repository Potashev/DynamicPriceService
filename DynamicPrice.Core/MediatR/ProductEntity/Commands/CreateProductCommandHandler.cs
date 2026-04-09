using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public class CreateProductCommandHandler
	: IRequestHandler<CreateProductCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public CreateProductCommandHandler(
		DynamicPriceCoreContext context,
		IMapper mapper,
		IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<int> Handle(
		CreateProductCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await _userService.GetRequiredCurrentUserAsync();

		var companyId = manager.CompanyId
			?? throw new BusinessException("У пользователя не указан CompanyId.");

		var product = _mapper.Map<Product>(request.ProductVm);

		product.CompanyId = companyId;
		product.LastSellTime = DateTime.UtcNow;

		await _context.Products.AddAsync(product, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		return product.ProductId;
	}
}
