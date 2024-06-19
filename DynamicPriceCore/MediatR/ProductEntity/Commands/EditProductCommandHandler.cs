using AutoMapper;
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
    private readonly IMapper _mapper;

    public EditProductCommandHandler(DynamicPriceCoreContext context, IMapper mapper)
        => (_context, _mapper) = (context, mapper);

    public async Task<int> Handle(EditProductCommand request, CancellationToken cancellationToken)
    {
        var updatedProductVm = request.ProductVm;
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductId == updatedProductVm.ProductId);

        if (product != null)
        {
            _mapper.Map(updatedProductVm, product);

            _context.Update(product);
            _context.SaveChanges();

            //product.Title        = updatedProduct.Title;
            //product.Price        = updatedProduct.Price;
            //product.MinimumPrice = updatedProduct.MinimumPrice;
            //product.Quantity     = updatedProduct.Quantity;
            //product.Description  = updatedProduct.Description;
        }


        return product.ProductId;
    }
}
