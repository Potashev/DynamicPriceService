using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;

namespace DynamicPrice.Core.MediatR.CustomerEntity.Queries;

public record GetCustomerInfoQuery : IRequest<CustomerInfoViewModel>;