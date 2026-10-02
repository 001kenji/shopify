using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Management_System.Data;
using Shop_Management_System.Models;
using Shop_Management_System.Services;
using System.Security.Claims;

[Authorize]
public class DeliveriesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAppEmailSender _emailSender;

    public DeliveriesController(ApplicationDbContext context, IAppEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    // GET: /Deliveries  (Admin sees all, users see their own)
    public async Task<IActionResult> Index()
    {
        var isAdmin = User.IsInRole("Admin");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var query = _context.Delivery
            .Include(d => d.Inventory)
            .Include(d => d.User)
            .AsQueryable();

        if (!isAdmin)
            query = query.Where(d => d.UserId == userId);

        var deliveries = await query
            .OrderBy(d => d.Status == DeliveryStatus.Delivered || d.Status == DeliveryStatus.Cancelled)
            .ThenByDescending(d => d.DateRequested)
            .ToListAsync();

        return View(deliveries);
    }

    // GET: /Deliveries/MyDeliveries
    public async Task<IActionResult> MyDeliveries()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var deliveries = await _context.Delivery
            .Include(d => d.Inventory)
            .Where(d => d.UserId == userId)
            .OrderBy(d => d.Status == DeliveryStatus.Delivered || d.Status == DeliveryStatus.Cancelled)
            .ThenByDescending(d => d.DateRequested)
            .ToListAsync();

        return View(deliveries);
    }

    // GET: /Deliveries/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var isAdmin = User.IsInRole("Admin");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var delivery = await _context.Delivery
            .Include(d => d.Inventory)
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id && (isAdmin || d.UserId == userId));

        if (delivery == null) return NotFound();
        return View(delivery);
    }

    // GET: /Deliveries/Create?inventoryId=5
    public async Task<IActionResult> Create(int? inventoryId)
    {
        if (inventoryId == null)
            return BadRequest("Shop item is required.");

        var inventory = await _context.Inventory.FindAsync(inventoryId);
        if (inventory == null) return NotFound();

        if (!inventory.Is_Delivarable)
            return BadRequest("This item is not available for delivery.");

        var delivery = new Delivery
        {
            InventoryID = inventory.ID,
            Quantity = 1,
            DateDelivered = DateTime.Today.AddDays(2),
            DeliveryCost = 0
        };

        ViewData["InventoryName"] = inventory.Name;
        ViewData["Description"] = inventory.Description;
        ViewData["InventoryPrice"] = inventory.SellingPrice;
        ViewData["AvailableQuantity"] = inventory.Quantity;
        ViewData["Unit"] = inventory.Unit;
        ViewData["DeliveryRadius"] = inventory.DeliveryRadius;

        return View(delivery);
    }

    // POST: /Deliveries/Create
    [HttpPost, ActionName("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost(
        [Bind("InventoryID,Quantity,Location,BuildingName,UnitName,DeliveryCost,DateDelivered")]
        Delivery delivery)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var inventory = await _context.Inventory
            .FirstOrDefaultAsync(i => i.ID == delivery.InventoryID);
        if (inventory == null) return NotFound();

        if (!inventory.Is_Delivarable)
            ModelState.AddModelError("", "This item is not available for delivery.");

        if (delivery.Quantity > inventory.Quantity)
            ModelState.AddModelError(nameof(delivery.Quantity),
                $"Only {inventory.Quantity} {inventory.Unit} available.");

        if (delivery.DateDelivered < DateTime.Today)
            ModelState.AddModelError(nameof(delivery.DateDelivered),
                "Delivery date cannot be in the past.");

        // Server-controlled fields — remove from validation
        ModelState.Remove("User");
        ModelState.Remove("UserId");
        ModelState.Remove("Amount");
        ModelState.Remove("Inventory");
        ModelState.Remove("Status");
        ModelState.Remove("DateRequested");
        ModelState.Remove("AdminNotes");

        if (ModelState.IsValid)
        {
            try
            {
                delivery.UserId = userId;
                delivery.Amount = inventory.SellingPrice * delivery.Quantity;
                delivery.Status = DeliveryStatus.Pending;
                delivery.DateRequested = DateTime.UtcNow;

                _context.Add(delivery);
                inventory.Quantity -= delivery.Quantity;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Confirmation), new { id = delivery.Id });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save delivery. Please try again.");
            }
        }

        ViewData["InventoryName"] = inventory.Name;
        ViewData["Description"] = inventory.Description;
        ViewData["InventoryPrice"] = inventory.SellingPrice;
        ViewData["AvailableQuantity"] = inventory.Quantity;
        ViewData["Unit"] = inventory.Unit;
        ViewData["DeliveryRadius"] = inventory.DeliveryRadius;

        return View(delivery);
    }

    // GET: /Deliveries/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var isAdmin = User.IsInRole("Admin");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var delivery = await _context.Delivery
            .Include(d => d.Inventory)
            .FirstOrDefaultAsync(d => d.Id == id && (isAdmin || d.UserId == userId));

        if (delivery == null) return NotFound();

        if (!isAdmin && delivery.Status != DeliveryStatus.Pending)
            return BadRequest("Only pending deliveries can be edited.");

        ViewData["InventoryName"] = delivery.Inventory?.Name;
        ViewData["InventoryPrice"] = delivery.Inventory?.SellingPrice;
        ViewData["Unit"] = delivery.Inventory?.Unit;
        ViewData["AvailableQuantity"] = delivery.Inventory?.Quantity;

        return View(delivery);
    }

    // POST: /Deliveries/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        [Bind("Id,InventoryID,Quantity,Location,BuildingName,UnitName,DeliveryCost,DateDelivered")]
        Delivery delivery)
    {
        if (id != delivery.Id) return NotFound();

        var isAdmin = User.IsInRole("Admin");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var existing = await _context.Delivery
            .Include(d => d.Inventory)
            .FirstOrDefaultAsync(d => d.Id == id && (isAdmin || d.UserId == userId));

        if (existing == null) return NotFound();

        if (!isAdmin && existing.Status != DeliveryStatus.Pending)
            return BadRequest("Only pending deliveries can be edited.");

        var inventory = await _context.Inventory.FindAsync(delivery.InventoryID);
        if (inventory == null) return NotFound();

        if (delivery.Quantity > inventory.Quantity + existing.Quantity)
            ModelState.AddModelError(nameof(delivery.Quantity),
                $"Only {inventory.Quantity + existing.Quantity} {inventory.Unit} available.");

        ModelState.Remove("User");
        ModelState.Remove("UserId");
        ModelState.Remove("Amount");
        ModelState.Remove("Inventory");
        ModelState.Remove("Status");
        ModelState.Remove("DateRequested");
        ModelState.Remove("AdminNotes");

        if (ModelState.IsValid)
        {
            try
            {
                // Restore old stock, decrement new stock
                if (existing.Inventory != null)
                    existing.Inventory.Quantity += existing.Quantity;

                existing.Quantity = delivery.Quantity;
                existing.Location = delivery.Location;
                existing.BuildingName = delivery.BuildingName;
                existing.UnitName = delivery.UnitName;
                existing.DeliveryCost = delivery.DeliveryCost;
                existing.DateDelivered = delivery.DateDelivered;
                existing.Amount = inventory.SellingPrice * delivery.Quantity;

                inventory.Quantity -= delivery.Quantity;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeliveryExists(delivery.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(isAdmin ? nameof(Index) : nameof(MyDeliveries));
        }

        ViewData["InventoryName"] = inventory.Name;
        ViewData["InventoryPrice"] = inventory.SellingPrice;
        ViewData["Unit"] = inventory.Unit;
        ViewData["AvailableQuantity"] = inventory.Quantity;

        return View(delivery);
    }

    // GET: /Deliveries/Confirmation/5
    public async Task<IActionResult> Confirmation(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var isAdmin = User.IsInRole("Admin");

        var delivery = await _context.Delivery
            .Include(d => d.Inventory)
            .FirstOrDefaultAsync(d => d.Id == id && (isAdmin || d.UserId == userId));

        if (delivery == null) return NotFound();
        return View(delivery);
    }

    // GET: /Deliveries/Delete/5  (Admin only)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var delivery = await _context.Delivery
            .Include(d => d.Inventory)
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (delivery == null) return NotFound();
        return View(delivery);
    }

    // POST: /Deliveries/Delete/5
    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var delivery = await _context.Delivery
            .Include(d => d.Inventory)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (delivery != null)
        {
            // Restore stock if the delivery wasn't already completed/cancelled
            if (delivery.Inventory != null &&
                delivery.Status != DeliveryStatus.Delivered &&
                delivery.Status != DeliveryStatus.Cancelled)
            {
                delivery.Inventory.Quantity += delivery.Quantity;
            }

            _context.Delivery.Remove(delivery);
            await _context.SaveChangesAsync();
        }

        TempData["StatusMessage"] = "Delivery deleted.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Deliveries/Confirm/5
    [HttpPost, ActionName("Confirm")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmConfirmed(int id)
    {
        var delivery = await _context.Delivery.FindAsync(id);
        if (delivery == null) return NotFound();

        if (delivery.Status != DeliveryStatus.Pending)
            return BadRequest("Only pending deliveries can be confirmed.");

        delivery.Status = DeliveryStatus.Confirmed;
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = $"Delivery #{delivery.Id:D6} confirmed.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Deliveries/Dispatch/5
    [HttpPost, ActionName("Dispatch")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DispatchConfirmed(int id)
    {
        var delivery = await _context.Delivery.FindAsync(id);
        if (delivery == null) return NotFound();

        if (delivery.Status != DeliveryStatus.Confirmed)
            return BadRequest("Only confirmed deliveries can be dispatched.");

        delivery.Status = DeliveryStatus.OutForDelivery;
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = $"Delivery #{delivery.Id:D6} dispatched.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Deliveries/Complete/5
    [HttpPost, ActionName("Complete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CompleteConfirmed(int id)
    {
        var delivery = await _context.Delivery.FindAsync(id);
        if (delivery == null) return NotFound();

        if (delivery.Status != DeliveryStatus.OutForDelivery)
            return BadRequest("Only deliveries that are out for delivery can be completed.");

        delivery.Status = DeliveryStatus.Delivered; 
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = $"Delivery #{delivery.Id:D6} marked as delivered.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Deliveries/Cancel/5
    [HttpPost, ActionName("Cancel")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelConfirmed(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var isAdmin = User.IsInRole("Admin");

        var delivery = await _context.Delivery
            .Include(d => d.Inventory)
            .FirstOrDefaultAsync(d => d.Id == id && (isAdmin || d.UserId == userId));

        if (delivery == null) return NotFound();

        if (delivery.Status != DeliveryStatus.Pending && delivery.Status != DeliveryStatus.Confirmed)
            return BadRequest("This delivery can no longer be cancelled.");

        delivery.Status = DeliveryStatus.Cancelled;

        if (delivery.Inventory != null)
            delivery.Inventory.Quantity += delivery.Quantity;

        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = $"Delivery #{delivery.Id:D6} cancelled.";
        return RedirectToAction(isAdmin ? nameof(Index) : nameof(MyDeliveries));
    }

    private bool DeliveryExists(int? id)
    {
        return _context.Delivery.Any(e => e.Id == id);
    }
}