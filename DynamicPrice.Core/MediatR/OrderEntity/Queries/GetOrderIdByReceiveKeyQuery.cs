using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public record GetOrderIdByReceiveKeyQuery(string ReceiveKey) : IRequest<int>;