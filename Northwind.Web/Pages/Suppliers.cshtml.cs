using Microsoft.AspNetCore.Mvc; // To use [BindProperty], IActionResult.
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

    [BindProperty]
    public Supplier? Supplier { get; set; }

    public void OnGet()
    {
        ViewData["Title"] = "Northwind B2B - Suppliers";

        Suppliers = _database.Suppliers
            .OrderBy(c => c.Country)
            .ThenBy(c => c.CompanyName);
    }

    public IActionResult OnPost()
    {
        if (Supplier is not null && ModelState.IsValid)
        {
            _database.Suppliers.Add(Supplier);
            _database.SaveChanges();
            return RedirectToPage("/suppliers");
        }
        else
        {
            return Page(); // Return to original page.
        }
    }
}
