using DynamicPrice.Core.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.CustomerEntity.Queries;

public record GetCustomerInfoQuery : IRequest<CustomerInfoViewModel>;