using Microsoft.AspNetCore.Mvc.RazorPages;
using Northwind.EntityModels; // To use NorthwindContext.

namespace Northwind.Web.Pages;

public class SuppliersModel : PageModel
{
	private readonly NorthwindContext _database;

	public SuppliersModel(NorthwindContext database)
	{
		_database = database;
	}

	public IEnumerable<Supplier>? Suppliers { get; set; }

	public void OnGet()
	{
		ViewData["Title"] = "Northwind B2B - Suppliers";

		Suppliers = _database.Suppliers
			.OrderBy(c => c.Country)
			.ThenBy(c => c.CompanyName);
	}
}
