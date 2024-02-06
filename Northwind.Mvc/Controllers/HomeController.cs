using Microsoft.AspNetCore.Mvc; // To use Controller, IActionResult.
using Microsoft.EntityFrameworkCore; // To use Include method.
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

	[ResponseCache(Duration = 10 /* seconds */, Location = ResponseCacheLocation.Any)]
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

	[Route("privacy")]
	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}

	public IActionResult ProductDetail(int? id, string alterstyle = "success")
	{
		ViewData["alterstyle"] = alterstyle;

		if (!id.HasValue)
		{
			return BadRequest("You must pass a product ID in the route, for example, / Home / ProductDetail / 21");
		}

		Product? model = _database.Products
			.Include(p => p.Category)
			.SingleOrDefault(p => p.ProductId == id);
		if (model is null)
		{
			return NotFound($"ProductId {id} not found.");
		}

		return View(model); // Pass model to view and then return result.
	}

	// This action method will handle GET and other requests except POST.
	public IActionResult ModelBinding()
	{
		return View(); // The page with a form to submit.
	}

	[HttpPost] // This action method will handle POST requests.
	public IActionResult ModelBinding(Thing thing)
	{
		HomeModelBindingViewModel model = new(
			Thing: thing,
			HasErrors: !ModelState.IsValid,
			ValidationErrors: ModelState.Values
			.SelectMany(state => state.Errors)
			.Select(error => error.ErrorMessage));

		return View(model); // Show the model bound thing.
	}
}
