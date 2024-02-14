using Microsoft.AspNetCore.Mvc; // To use Controller, IActionResult.
using Microsoft.EntityFrameworkCore; // To use the Include and ToListAsync extension methods.
using Northwind.EntityModels; // To use NorthwindContext.
using Northwind.Mvc.Models; // To use ErrorViewModel.
using System.Diagnostics; // To use Activity.

namespace Northwind.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly NorthwindContext _database;
    private readonly IHttpClientFactory _clientFactory;

    public HomeController(ILogger<HomeController> logger, NorthwindContext database, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _database = database;
        _clientFactory = clientFactory;
    }

    [ResponseCache(Duration = 10 /* seconds */, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Index()
    {
        _logger.LogError("This is a serious error (not really!)");
        _logger.LogWarning("This is your first warning!");
        _logger.LogWarning("Second warning!");
        _logger.LogInformation("I am in the Index method of the HomeController.");

        HomeIndexViewModel model = new(VisitorCount: Random.Shared.Next(1, 1001),
            Categories: await _database.Categories.ToListAsync(), Products: await _database.Products.ToListAsync());

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

    public async Task<IActionResult> ProductDetail(int? id, string alterstyle = "success")
    {
        ViewData["alterstyle"] = alterstyle;

        if (!id.HasValue)
        {
            return BadRequest("You must pass a product ID in the route, for example, / Home / ProductDetail / 21");
        }

        Product? model = await _database.Products
            .Include(p => p.Category)
            .SingleOrDefaultAsync(p => p.ProductId == id);
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

    public IActionResult ProductsThatCostMoreThan(decimal? price)
    {
        if (!price.HasValue)
        {
            return BadRequest("You must pass a product price in the query string,for example, / Home / ProductsThatCostMoreThan ? price = 50");
        }

        IEnumerable<Product> model = _database.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.UnitPrice > price);

        if (!model.Any())
        {
            return NotFound($"No products cost more than {price:C}.");
        }
        ViewData["MaxPrice"] = price.Value.ToString("C");

        return View(model);
    }

    public async Task<IActionResult> Customers(string country)
    {
        string uri;
        if (string.IsNullOrEmpty(country))
        {
            ViewData["Title"] = "All Customers Worldwide";
            uri = "api/customers";
        }
        else
        {
            ViewData["Title"] = $"Customers in {country}";
            uri = $"api/customers/?country={country}";
        }

        HttpClient client = _clientFactory.CreateClient(name: "Northwind.WebApi");
        HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: uri);
        HttpResponseMessage response = await client.SendAsync(request);
        IEnumerable<Customer>? model = await response.Content.ReadFromJsonAsync<IEnumerable<Customer>>();

        return View(model);
    }
}
