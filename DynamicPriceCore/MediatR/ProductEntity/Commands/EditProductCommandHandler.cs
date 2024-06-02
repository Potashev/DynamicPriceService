using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public class EditProductCommandHandler
    : IRequestHandler<EditProductCommand, int>
{
    private readonly DynamicPriceCoreContext _context;

    public EditProductCommandHandler(DynamicPriceCoreContext context)
        => _context = context;

    public async Task<int> Handle(EditProductCommand request, CancellationToken cancellationToken)
    {
        var updatedProduct = request.Product;
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductId == updatedProduct.ProductId);

        if (product != null)
        {
            product.Title        = updatedProduct.Title;
            product.Price        = updatedProduct.Price;
            product.MinimumPrice = updatedProduct.MinimumPrice;
            product.Quantity     = updatedProduct.Quantity;
            product.Description  = updatedProduct.Description;
        }

        _context.Update(product);
        _context.SaveChanges();

        return product.ProductId;
    }
}
