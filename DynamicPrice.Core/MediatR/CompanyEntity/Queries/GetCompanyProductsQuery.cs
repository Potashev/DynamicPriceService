using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;

namespace DynamicPrice.Core.MediatR.CompanyEntity.Queries;

public record GetCompanyProductsQuery(Guid CompanyId) : IRequest<CompanyProductsInfo>;