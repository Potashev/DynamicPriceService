using DynamicPriceCore.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.CustomerEntity.Queries;

public record GetCustomerInfoQuery : IRequest<CustomerInfoViewModel>;