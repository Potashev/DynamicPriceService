using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.CustomerEntity.Queries;

public record GetCustomerInfoQuery : IRequest<CustomerInfoViewModel>;