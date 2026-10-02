
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Shop_Management_System.Data;
using Shop_Management_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

[Authorize(Roles ="Admin")]
public class InventoriesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public InventoriesController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    private static List<SelectListItem> GetCategorySelectList()
    {
        return Enum.GetValues<CategoriesChoices>()
            .Select(c => new SelectListItem
            {
                Value = c.ToString(),
                Text = c.GetType()
                         .GetMember(c.ToString())[0]
                         .GetCustomAttribute<DisplayAttribute>()?.Name
                         ?? c.ToString()
            })
            .ToList();
    }

    // GET: INVENTORYS
    public async Task<IActionResult> Index(string? SearchName, CategoriesChoices? category,string? sortOrder )    
    {
        var query = _context.Inventory.AsQueryable();
        if (!string.IsNullOrWhiteSpace(SearchName))
        {
            query = query.Where(inv => inv.Name.Contains(SearchName));
        }
        if (category.HasValue)
        {
            query = query.Where(inv => inv.Categories == category);
        }


        //sort
        query = sortOrder switch
        {
            "qty_asc" => query.OrderBy(i => i.Quantity),
            "qty_desc" => query.OrderByDescending(i => i.Quantity),
            "price_asc" => query.OrderBy(i => i.SellingPrice),
            "price_desc" => query.OrderByDescending(i => i.SellingPrice),
            "name_desc" => query.OrderByDescending(i => i.Name),
            _ => query.OrderBy(i => i.Name), //default sort
        };

        var inventories = await query.ToListAsync();
        ViewData["SearchName"] = SearchName;
        ViewData["SelectedCategory"] = category;
        ViewData["CurrentSort"] = sortOrder;
        return View(inventories);
    }

    // GET: INVENTORYS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var inventory = await _context.Inventory
            .FirstOrDefaultAsync(m => m.ID == id);
        if (inventory == null)
        {
            return NotFound();
        }

        return View(inventory);
    }

    // GET: INVENTORYS/Create
    public IActionResult Create()
    {
        ViewData["Categories"] = GetCategorySelectList();
        return View();
    }

    // POST: INVENTORYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    [Bind("Name,Description,Categories,Unit,BestBefore,BuyingPrice,SellingPrice,Quantity,Unit,Is_Delivarable,DeliveryRadius")]
    Inventory inventory,
    IFormFile? ImageFile)
    {
        // 1) Validate the file FIRST, so any errors land in ModelState before we check it
        string? writtenFilePath = null;
        if (ImageFile != null && ImageFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var extension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                ModelState.AddModelError(nameof(ImageFile), "Only image files are allowed.");

            const long maxBytes = 5 * 1024 * 1024;
            if (ImageFile.Length > maxBytes)
                ModelState.AddModelError(nameof(ImageFile), "File must be 5 MB or smaller.");

            // 2) If anything failed so far, bail out BEFORE writing to disk
            ModelState.Remove("ImagePath");
            if (!ModelState.IsValid)
            {

            
                ViewData["Categories"] = GetCategorySelectList();
                return View(inventory);
            }

            // 3) Now safe to write
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "media");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            writtenFilePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(writtenFilePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(fileStream);
            }

            inventory.ImagePath = "/media/" + uniqueFileName;
        }
        ModelState.Remove("ImagePath");
        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                if (writtenFilePath != null && System.IO.File.Exists(writtenFilePath))
                    System.IO.File.Delete(writtenFilePath);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
        }
        ViewData["Categories"] = GetCategorySelectList();
        return View(inventory);
    }
    // GET: INVENTORYS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var inventory = await _context.Inventory.FindAsync(id);
        if (inventory == null)
        {
            return NotFound();
        }

        ViewData["Categories"] = GetCategorySelectList();
        return View(inventory);
    }

   

    // POST: INVENTORYS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int? id, IFormFile? ImageFile)
    {
        if (id == null)
        {
            return NotFound();
        }
        ModelState.Remove("ImagePath");

        var inventoryToEdit = await _context.Inventory.FirstOrDefaultAsync(inv => inv.ID == id);
        
        if (inventoryToEdit == null) return NotFound();
        string oldFilePath = inventoryToEdit.ImagePath;
        if (await TryUpdateModelAsync<Inventory>(
            inventoryToEdit,
            "",
            inv => inv.Name, inv => inv.Description, inv => inv.Categories, inv => inv.BestBefore, inv => inv.Unit,
            inv => inv.BuyingPrice, inv => inv.SellingPrice, inv => inv.Quantity, inv => inv.Is_Delivarable, inv => inv.DeliveryRadius
            ))
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var extension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(ImageFile), "Only image files are allowed.");
                    ViewData["Categories"] = GetCategorySelectList();
                    return View(inventoryToEdit);
                }

                string uploadsFolder = Path.Combine(_environment.WebRootPath, "media");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }
                inventoryToEdit.ImagePath = "/media/" + uniqueFileName;
                

            }
            try
            {
                await _context.SaveChangesAsync();
                
            }
            catch(DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                ViewData["Categories"] = GetCategorySelectList();
                return View(inventoryToEdit);
            }

            //delete previous file
            if (!string.IsNullOrEmpty(oldFilePath) && ImageFile != null && ImageFile.Length > 0)
            {
                string oldPhysicalPath = Path.Combine(_environment.WebRootPath, oldFilePath.TrimStart('/'));
                if (System.IO.File.Exists(oldPhysicalPath))
                {
                    System.IO.File.Delete(oldPhysicalPath);
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["Categories"] = GetCategorySelectList();
        return View(inventoryToEdit);
    }

    // GET: INVENTORYS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var inventory = await _context.Inventory
            .FirstOrDefaultAsync(m => m.ID == id);
        if (inventory == null)
        {
            return NotFound();
        }

        return View(inventory);
    }

    // POST: INVENTORYS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var inventory = await _context.Inventory.FindAsync(id);
        if (inventory != null)
        {
            _context.Inventory.Remove(inventory);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool InventoryExists(int? id)
    {
        return _context.Inventory.Any(e => e.ID == id);
    }
}
