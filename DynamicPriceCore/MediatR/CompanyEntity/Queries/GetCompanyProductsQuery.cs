using DynamicPriceCore.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.CompanyEntity.Queries;

public record GetCompanyProductsQuery(string CompanyId) : IRequest<CompanyProductsInfo>;