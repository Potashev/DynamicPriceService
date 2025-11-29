using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public record GetOrderIdByReceiveKeyQuery(string ReceiveKey) : IRequest<int>;