using AutoMapper;
using DynamicPriceService.Data;
using DynamicPriceService.MediatR.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceService.MediatR.ProductEntity.Queries;

public class GetProductDetailsQueryHandler
    : IRequestHandler<GetProductDetailsQuery, ProductViewModel>
{
    private readonly DynamicPriceServiceContext _context;
    private readonly IMapper _mapper;

    public GetProductDetailsQueryHandler(DynamicPriceServiceContext context, IMapper mapper)
        => (_context, _mapper) = (context, mapper);

    public async Task<ProductViewModel> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Product.FirstOrDefaultAsync(product => product.ProductId == request.ProductId, cancellationToken);
        return _mapper.Map<ProductViewModel>(product);
    }
}
