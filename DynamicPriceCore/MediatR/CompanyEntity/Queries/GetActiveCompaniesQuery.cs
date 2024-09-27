using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.CompanyEntity.Queries;

public record GetActiveCompaniesQuery : IRequest<IEnumerable<Company>>;