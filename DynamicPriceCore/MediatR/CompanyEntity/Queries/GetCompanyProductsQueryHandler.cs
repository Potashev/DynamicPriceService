using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ProductEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.CompanyEntity.Queries;

public class GetCompanyProductsQueryHandler
	: IRequestHandler<GetCompanyProductsQuery, CompanyProductsInfo>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public GetCompanyProductsQueryHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<CompanyProductsInfo> Handle(GetCompanyProductsQuery request, CancellationToken cancellationToken)
	{
		//var products = await _context.Products
		//	.Where(p => p.Company.CompanyId.ToString() == request.CompanyId)
		//	//.Include(p => p.PriceDynamics)	//todo: set lenght?
		//	.ToArrayAsync(cancellationToken);

		//var productsInfoVm = _mapper.Map<ProductInfoViewModel[]>(products);

		////todo: test price dynamics - remove later
		//foreach (var productInfoVm in productsInfoVm)
		//{
		//	SetPriceDynamics(productInfoVm);
		//}

		//var cpi = new CompanyProductsInfo(request.CompanyId, productsInfoVm);
		//return cpi;

		var products = await _context.Products
	.Where(p => p.Company.CompanyId.ToString() == request.CompanyId)
	.Include(p => p.PriceDynamics)  //todo: set lenght?
	.ToArrayAsync(cancellationToken);

		var productsInfoVm = _mapper.Map<ProductInfoViewModel[]>(products);

		////todo: test price dynamics - remove later
		//foreach (var productInfoVm in productsInfoVm)
		//{
		//	SetPriceDynamics(productInfoVm);
		//}

		var cpi = new CompanyProductsInfo(request.CompanyId, productsInfoVm);
		var pd = new PriceDynamic();    // return pd;

		var pdValue = new PriceDynamic();
		pdValue.Id = 1;
		pdValue.Price = 300;

		//cpi.Products[0].PriceDynamics = new PriceDynamic[1] {pdValue};

		return cpi;
	}

	//private void SetPriceDynamics(ProductInfoViewModel productInfoVm)
	//{
	//	var dynamicsLenght = 100;
	//	for (var i = 0; i < dynamicsLenght; i++)
	//	{
	//		var productPrice = productInfoVm.Price;
	//		var size = productPrice * 0.05m;
	//		// определить рамки - низ и верх (допустим в пределах 5% от цены)
	//		var min = productPrice - size;
	//		var max = productPrice + size;
	//		// сгенерить случайное число от 0 до верх-низ
	//		var rnd = new Random().Next((int)(max - min));
	//		// добавить в список низ+рандом
	//		var randomPrice = min + rnd;
	//		productInfoVm.PriceDynamics.Add(randomPrice);
	//	}
	//}
}
