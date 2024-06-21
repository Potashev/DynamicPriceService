using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace DynamicPriceService.Controllers;
public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
	{
		_logger = logger;
		_httpContextAccessor = httpContextAccessor;
	}

	public IActionResult Index()
	{
		return View();
	}
	public async Task<IActionResult> Login()
	{
		var users = new List<int> { 1, 2 };
		ViewData["Users"] = new SelectList(users);
		return View();
	}

	[HttpPost]
	public IActionResult Login(int selectedValue)
	{
		var context = _httpContextAccessor.HttpContext;
		context.Response.Cookies.Append("User", selectedValue.ToString());
		return RedirectToAction(nameof(Index));
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
