using Microsoft.AspNetCore.Mvc; // To use Controller, IActionResult.
using Northwind.EntityModels; // To use NorthwindContext.
using Northwind.Mvc.Models; // To use ErrorViewModel.
using System.Diagnostics; // To use Activity.

namespace Northwind.Mvc.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly NorthwindContext _database;

	public HomeController(ILogger<HomeController> logger, NorthwindContext database)
	{
		_logger = logger;
		_database = database;
	}

	public IActionResult Index()
	{
		_logger.LogError("This is a serious error (not really!)");
		_logger.LogWarning("This is your first warning!");
		_logger.LogWarning("Second warning!");
		_logger.LogInformation("I am in the Index method of the HomeController.");

		HomeIndexViewModel model = new(VisitorCount: Random.Shared.Next(1, 1001),
			Categories: _database.Categories.ToList(), Products: _database.Products.ToList());

		return View(model); // Pass the model to the view.
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
