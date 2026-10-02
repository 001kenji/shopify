
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Management_System.Data;
using Shop_Management_System.Models;
using Shop_Management_System.Services;
using System.Security.Claims;

[Authorize]
public class BookingsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAppEmailSender _emailSender;
    public BookingsController(ApplicationDbContext context, IAppEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }


    public async Task<IActionResult> MyBookings()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var bookings = await _context.Booking
            .Include(b => b.Inventory)
            .Where(b => b.UserId == userId)
            .OrderBy(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.Cancelled)
            .OrderByDescending(b => b.DateOrdered)
            .ToListAsync();
        return View(bookings);
    }

    public async Task<IActionResult> Index()    
    {
        if (User.IsInRole("Admin"))
        {
            return View(await _context.Booking
                .Include(b => b.Inventory)
                .Include(b => b.User)
                .OrderBy(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.Cancelled)
                .OrderByDescending(b => b.DateOrdered)
                .ToListAsync()
            );
        }else
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var bookings = await _context.Booking
                .Include(b => b.Inventory)
                .Where(b => b.UserId == userId)
                .OrderBy(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.Cancelled)
                .OrderByDescending(b => b.DateOrdered)
                .ToListAsync();
            return View(bookings);
        }
        
    }

    // GET: BOOKINGS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var isAdmin = User.IsInRole("Admin");

        var booking = await _context.Booking
            .Include(b => b.Inventory)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id && (isAdmin || b.UserId == userId));

        if (booking == null) return NotFound();
        return View(booking);
    }

    // GET: BOOKINGS/Create
    public async Task<IActionResult> Create(int? inventoryId)
    {
        if (inventoryId == null) return BadRequest("Shop item is required.");
        var inventory = await _context.Inventory.FindAsync(inventoryId);
        if (inventory == null) return NotFound();

        var booking = new Booking
        {
            InventoryID = inventory.ID,
            PickingDuration = DateTime.Today.AddDays(1)
        };

        ViewData["InventoryName"] = inventory.Name;
        ViewData["Description"] = inventory.Description;
        ViewData["InventoryPrice"] = inventory.SellingPrice;
        ViewData["AvailableQuantity"] = inventory.Quantity;
        ViewData["Unit"] = inventory.Unit;
        return View(booking); 
    }

    // POST: BOOKINGS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost,ActionName("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost(
    [Bind("InventoryID,Quantity,PickingDuration")] Booking booking)
    {
        // 1) Get the user
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        // 2) Load the inventory item (don't trust the FK alone)
        var inventory = await _context.Inventory
            .FirstOrDefaultAsync(i => i.ID == booking.InventoryID);
        if (inventory == null) return NotFound();

        // 3) Validate stock server-side
        if (booking.Quantity > inventory.Quantity)
        {
            ModelState.AddModelError(nameof(booking.Quantity),
                $"Only {inventory.Quantity} {inventory.Unit} available.");
        }

        // 4) Populate server-controlled fields
        booking.UserId = userId;
        booking.DateOrdered = DateTime.UtcNow;
        booking.Inventory = inventory;   // EF will link the FK anyway, but this helps if you want the entity
        booking.Status = BookingStatus.Pending;
        ModelState.Remove("UserId");
        ModelState.Remove("Inventory");
        ModelState.Remove("DateOrdered");
        ModelState.Remove("User");
        if (ModelState.IsValid)
        {
            try
            {
                booking.User = await _context.Users.FindAsync(userId);
                _context.Add(booking);

                // 5) Optionally reserve/decrement stock here
                inventory.Quantity -= booking.Quantity;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Confirmation), new { id = booking.Id });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save booking. Please try again.");
            }
        }

        // 6) On failure, re-fill the ViewData so the view can render item info
        ViewData["InventoryName"] = inventory.Name;
        ViewData["Description"] = inventory.Description;
        ViewData["InventoryPrice"] = inventory.SellingPrice;
        ViewData["AvailableQuantity"] = inventory.Quantity;
        ViewData["Unit"] = inventory.Unit;

        return View(booking);
    }

    // GET: BOOKINGS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await _context.Booking.FindAsync(id);
        if (booking == null)
        {
            return NotFound();
        }
        return View(booking);
    }

    public async Task<IActionResult> Confirmation(int id)
    {
        var booking = await _context.Booking
            .Include(b => b.Inventory)
            .FirstOrDefaultAsync(b => b.Id == id
                && b.UserId == User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if (booking == null) return NotFound();
        return View(booking);
    }

    // POST: BOOKINGS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost,ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int? id, [Bind("Id,InventoryID,Inventory,UserId,User,Quantity,PickingDuration,DateOrdered")] Booking booking)
    {
        if (id != booking.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(booking);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(booking.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(booking);
    }

    // GET: BOOKINGS/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await _context.Booking
            .FirstOrDefaultAsync(m => m.Id == id);
        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }

    // POST: BOOKINGS/Delete/5
    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var booking = await _context.Booking.FindAsync(id);
        if (booking != null)
        {
            _context.Booking.Remove(booking);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BookingExists(int? id)
    {
        return _context.Booking.Any(e => e.Id == id);
    }


    // POST: /Bookings/Confirm/5
    [HttpPost, ActionName("Confirm")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmConfirmed(int id)
    {
        var booking = await _context.Booking.FindAsync(id);
        if (booking == null) return NotFound();

        if (booking.Status != BookingStatus.Pending)
            return BadRequest("Only pending bookings can be confirmed.");

        booking.Status = BookingStatus.Confirmed;
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = $"Booking #{booking.Id:D6} confirmed.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Bookings/Complete/5
    [HttpPost, ActionName("Complete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CompleteConfirmed(int id)
    {
        var booking = await _context.Booking
            .Include(b => b.Inventory)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (booking == null) return NotFound();

        if (booking.Status != BookingStatus.Confirmed)
            return BadRequest("Only confirmed bookings can be completed.");

        booking.Status = BookingStatus.Completed;
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = $"Booking #{booking.Id:D6} marked as completed.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Bookings/Cancel/5
    [HttpPost, ActionName("Cancel")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelConfirmed(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var isAdmin = User.IsInRole("Admin");

        // Admin can cancel any; user can only cancel their own
        var booking = await _context.Booking
            .Include(b => b.Inventory)
            .FirstOrDefaultAsync(b => b.Id == id && (isAdmin || b.UserId == userId));

        if (booking == null) return NotFound();

        if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
            return BadRequest("This booking can no longer be cancelled.");

        booking.Status = BookingStatus.Cancelled;

        // Restore inventory stock
        if (booking.Inventory != null)
            booking.Inventory.Quantity += booking.Quantity;

        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = $"Booking #{booking.Id:D6} cancelled.";
        return RedirectToAction(isAdmin ? nameof(Index) : nameof(Index));
    }
}

