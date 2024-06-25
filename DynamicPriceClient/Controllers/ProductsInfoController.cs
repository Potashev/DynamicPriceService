using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceClient.Controllers;
public class ProductsInfoController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}
