using DynamicPrice.Core.MediatR.CartEntity.Commands;
using DynamicPrice.Core.MediatR.CartEntity.Queries;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.Controllers;

/// <summary>
/// Контроллер для управления корзиной покупателя.
/// Предоставляет операции получения содержимого корзины, добавления и удаления товаров.
/// Все действия защищены политикой "CustomerPolicy" и требуют аутентификации по JWT.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
public class CartController : ControllerBase
{
	private readonly IMediator _mediator;
	public CartController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Получить детали корзины для указанной компании.
	/// Возвращает пустой результат с кодом 404, если корзина пуста.
	/// </summary>
	/// <param name="companyId">Идентификатор компании (параметр запроса, ключ "company-id").</param>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>Объект <see cref="CartViewModel"/> с деталями корзины или 404, если корзина пуста.</returns>
	[HttpGet]
	public async Task<ActionResult<CartViewModel>> GetCartDetails([FromQuery(Name = "company-id")] string companyId, CancellationToken cancellationToken)
	{
		var cart = await _mediator.Send(new GetCartDetailsQuery(Convert.ToInt32(companyId)), cancellationToken);

		return cart == null
			? NotFound(new { message = "Cart is empty." })
			: Ok(cart);
	}

	/// <summary>
	/// Добавить товар в корзину текущего пользователя.
	/// </summary>
	/// <param name="productId">Идентификатор добавляемого товара в теле запроса.</param>
	/// <returns>Идентификатор компании, к которой относится обновлённая корзина.</returns>
	[HttpPost("items")]
	public async Task<ActionResult<int>> AddProduct([FromBody] int? productId)
	{
		var cart = await _mediator.Send(new AddProductToCartCommand(productId.ToString()));
		return Ok(cart.Company.CompanyId);
	}

	/// <summary>
	/// Удалить товар из корзины текущего пользователя.
	/// </summary>
	/// <param name="productId">Идентификатор удаляемого товара (в маршруте).</param>
	/// <returns>Идентификатор компании, к которой относится обновлённая корзина.</returns>
	[HttpDelete("items/{productId}")]
	public async Task<ActionResult<int>> RemoveProduct(int? productId)
	{
		var cart = await _mediator.Send(new RemoveProductFromCartCommand(productId.ToString()));
		return Ok(cart.Company.CompanyId);
	}
}
