using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.CustomerEntity.Queries;

public record GetCustomerInfoQuery : IRequest<CustomerInfoViewModel>;