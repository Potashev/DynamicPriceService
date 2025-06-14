using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceClient.Controllers;

//todo: obsolete?
public class ProductsInfoController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}
