using DynamicPrice.Core.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.CompanyEntity.Queries;

public record GetCompanyProductsQuery(string CompanyId) : IRequest<CompanyProductsInfo>;