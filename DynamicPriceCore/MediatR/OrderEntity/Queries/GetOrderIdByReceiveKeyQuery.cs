using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetOrderIdByReceiveKeyQuery : IRequest<int>
{
	public string ReceiveKey { get; set; }
	public GetOrderIdByReceiveKeyQuery(string receiveKey) => ReceiveKey = receiveKey;
}
