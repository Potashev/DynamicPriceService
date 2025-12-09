using DynamicPrice.Core.MediatR.ProductEntity.Commands;
using DynamicPrice.Core.MediatR.ProductEntity.Queries;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.Controllers;

/// <summary>
/// API для управления товарами компании.
/// Доступен только для пользователей с политикой "ManagerPolicy".
/// </summary>
[Route("api/company/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
public class ProductsController : ControllerBase
{
	private readonly IMediator _mediator;
	public ProductsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Получить список товаров компании.
	/// </summary>
	/// <param name="cancellationToken">Токен для отмены операции.</param>
	/// <returns>Список представлений товаров <see cref="ProductViewModel"/> с кодом 200.</returns>
	[HttpGet]
	public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProducts(CancellationToken cancellationToken)
	{
		var productsVm = await _mediator.Send(new GetProductsQuery(), cancellationToken);
		return Ok(productsVm);
	}

	/// <summary>
	/// Получить детали конкретного товара по идентификатору.
	/// </summary>
	/// <param name="id">Идентификатор товара.</param>
	/// <returns>Детали товара <see cref="ProductViewModel"/> с кодом 200.</returns>
	[HttpGet("{id}")]
	public async Task<ActionResult<ProductViewModel>> GetProduct(int id)
	{
		var productVm = await _mediator.Send(new GetProductDetailsQuery(id));
		return Ok(productVm);
	}

	/// <summary>
	/// Обновить существующий товар.
	/// </summary>
	/// <param name="id">Идентификатор товара в маршруте.</param>
	/// <param name="productVm">Модель товара с обновлёнными полями.</param>
	/// <returns>Идентификатор обновлённого товара (200) или 400, если id не совпадает.</returns>
	[HttpPut("{id}")]
	public async Task<IActionResult> Edit(int id, ProductViewModel productVm)
	{
		if (id != productVm.ProductId)
		{
			return BadRequest();
		}
		var productId = await _mediator.Send(new EditProductCommand(productVm));
		return Ok(productId);
	}

	/// <summary>
	/// Создать новый товар.
	/// </summary>
	/// <param name="productVm">Модель создаваемого товара.</param>
	/// <returns>Идентификатор созданного товара.</returns>
	[HttpPost]
	public async Task<ActionResult<int>> Create(ProductViewModel productVm)
	{
		var productId = await _mediator.Send(new CreateProductCommand(productVm));
		return Ok(productId);
	}

	/// <summary>
	/// Удалить товар по идентификатору.
	/// </summary>
	/// <param name="id">Идентификатор удаляемого товара.</param>
	/// <returns>200 OK при успешном удалении.</returns>
	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		await _mediator.Send(new DeleteProductCommand(id));
		return Ok();
	}
}