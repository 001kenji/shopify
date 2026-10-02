
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Management_System.Models;
using Shop_Management_System.Data;
using Microsoft.AspNetCore.Authorization;


public class ShopController : Controller
{
    private readonly ApplicationDbContext _context;

    public ShopController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: INVENTORYS
    public async Task<IActionResult> Index(string? SearchName,CategoriesChoices? category)    
    {
        var query = _context.Inventory
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(SearchName))
        {
            query = query.Where(inv => inv.Name.Contains(SearchName));
        }
        if (category.HasValue)
        {
            query = query.Where(inv => inv.Categories == category);
        }
        var intentories = await query
            .OrderBy(i => i.Name)
            .ToListAsync();

        ViewData["SearchName"] = SearchName;
        ViewData["SelectedCategory"] = category;
        return View(intentories);
    }

    // GET: INVENTORYS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var inventory = await _context.Inventory
            .Include(i => i.Bookings)
            .Include(i => i.Deliveries)
            .FirstOrDefaultAsync(m => m.ID == id);
        if (inventory == null)
        {
            return NotFound();
        }

        return View(inventory);
    }

   
}
