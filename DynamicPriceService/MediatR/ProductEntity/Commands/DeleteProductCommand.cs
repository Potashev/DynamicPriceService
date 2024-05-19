﻿using MediatR;

namespace DynamicPriceService.MediatR.ProductEntity.Commands;

public class DeleteProductCommand : IRequest
{
    public int ProductId { get; set; }
    public DeleteProductCommand(int productId) => ProductId = productId;
}
