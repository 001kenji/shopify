
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Management_System.Models;
using Shop_Management_System.Data;
using Microsoft.AspNetCore.Authorization;
using Shop_Management_System.Extensions;

[Authorize]
public class PaymentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public PaymentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: PAYMENTS
    public async Task<IActionResult> Index()    
    {
        var paymentsref = await _context.Payment
            .Include(f => f.User)
            .ToListAsync();
        return View(paymentsref);
    }

    // GET: PAYMENTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var payment = await _context.Payment
            .FirstOrDefaultAsync(m => m.id == id);
        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    // GET: PAYMENTS/Create
    public async Task<IActionResult> Create(int? Id, ServiceType? serviceScope)        
    {
        if (Id == null) return BadRequest("Item to be paid is required");
        var userid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userid == null) return BadRequest("Please login to proceed");
        Booking? bookref = null;
        Delivery? deliveryref = null; 
        if (serviceScope == ServiceType.BOOK)
        {
            bookref = await _context.Booking
            .Include(r => r.Inventory)
            .FirstOrDefaultAsync(b => b.Id == Id && b.UserId == userid);

        }else if(serviceScope == ServiceType.DELIVER)
        {
            deliveryref = await _context.Delivery
            .Include(r => r.Inventory)
            .FirstOrDefaultAsync(b => b.Id == Id && b.UserId == userid);

        }


        var totalamount = serviceScope == ServiceType.BOOK ? bookref.Inventory.SellingPrice * bookref.Quantity : deliveryref.Inventory.SellingPrice * deliveryref.Quantity;
        var requestedQuantity = serviceScope == ServiceType.BOOK ? bookref.Quantity : deliveryref.Quantity;

        var paymentref = new Payment {
            Service = serviceScope ?? null,
            Quantity = requestedQuantity,
            BookingID = serviceScope == ServiceType.BOOK ? bookref.Id : null,
            Amount = totalamount,
            DeliveryId = serviceScope == ServiceType.DELIVER ? deliveryref.Id : null,
            
        };
        

        ViewData["InventoryName"] = serviceScope == ServiceType.BOOK ? bookref.Inventory.Name : deliveryref.Inventory.Name;
        ViewData["InventoryPrice"] = serviceScope == ServiceType.BOOK ? bookref.Inventory.SellingPrice : deliveryref.Inventory.SellingPrice;
        ViewData["RequestedQuantiry"] = requestedQuantity;
        ViewData["Total"] = totalamount;
        ViewData["Units"] = serviceScope == ServiceType.BOOK ? bookref.Inventory.Unit : deliveryref.Inventory.Unit;
        ViewData["AvailableQuantity"] = serviceScope == ServiceType.BOOK
            ? bookref.Inventory.Quantity       // whatever property holds stock on Inventory
            : deliveryref.Inventory.Quantity;
        ViewData["Service"] = serviceScope;
        return View(paymentref);
    }

    // POST: PAYMENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to. 
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost, ActionName("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost([Bind("Amount,Quantity,BookingID,DeliveryId,Service,PaymentMethod")] Payment payment)
    {
        
        var userid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var serviceScope = payment.Service;
        if (serviceScope == null)
        {
            ModelState.AddModelError("Service", "Service scope is required.");
            return View(payment);
        }

        Booking? bookref = null;
        Delivery? deliveryref = null;
        if (userid == null) return BadRequest("Please login to proceed");

        if (serviceScope == ServiceType.BOOK)
        {
            bookref = await _context.Booking
                        .Include(d => d.Inventory)
                        .FirstOrDefaultAsync(b => b.Id == payment.BookingID && b.UserId == userid);
            if (bookref == null) return BadRequest("Booking Item not found");
        }else if (serviceScope == ServiceType.DELIVER)
        { 
            deliveryref = await _context.Delivery
            .Include(d => d.Inventory)
            .FirstOrDefaultAsync(b => b.Id == payment.DeliveryId && b.UserId == userid);
            if (deliveryref == null) return BadRequest("Delivery Item not found");
        }

        var availablequantity = serviceScope == ServiceType.BOOK ? bookref.Inventory.Quantity : deliveryref.Inventory.Quantity;
        
        if(payment.Quantity < 1 || payment.Quantity > availablequantity)
        {
            ModelState.AddModelError(nameof(payment.Quantity),
                $"Quantity must be between 1 and {availablequantity}.");
            return View(payment);
        }

        // process mpesa payment

        ModelState.Remove("User");
        ModelState.Remove("UserId");
        ModelState.Remove("ItemName");
        ModelState.Remove("PaymentDate");
        if (ModelState.IsValid)
        {
            payment.User = await _context.Users.FindAsync(userid);
            payment.UserId = userid;
            payment.ItemName = serviceScope == ServiceType.BOOK ? bookref.Inventory.Name : deliveryref.Inventory.Name;
            payment.Quantity = serviceScope == ServiceType.BOOK ? bookref.Quantity : deliveryref.Quantity;
            payment.PaymentDate = DateTime.Now;

            // mark item as paid
            try
            {
                _context.Add(payment);
                if (serviceScope ==  ServiceType.BOOK)
                {
                    bookref.Status = BookingStatus.PendingConfirmation;
                    bookref.Inventory.Quantity -= payment.Quantity;
                }
                else if (serviceScope == ServiceType.DELIVER)
                {
                    deliveryref.Status = DeliveryStatus.PendingConfimation;
                    deliveryref.Inventory.Quantity -= payment.Quantity;
                } 
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var msg = ex.InnerException?.Message;
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
                ModelState.AddModelError("", "Unable to process payment: " + (ex.InnerException?.Message ?? ex.Message));
            }
            
        }
        ViewData["InventoryName"] = serviceScope == ServiceType.BOOK ? bookref.Inventory.Name : deliveryref.Inventory.Name;
        ViewData["InventoryPrice"] = serviceScope == ServiceType.BOOK ? bookref.Inventory.SellingPrice : deliveryref.Inventory.SellingPrice;
        ViewData["RequestedQuantiry"] = serviceScope == ServiceType.BOOK ? bookref.Quantity : deliveryref.Quantity;
        ViewData["Total"] = serviceScope == ServiceType.BOOK ? bookref.Inventory.SellingPrice * bookref.Quantity : deliveryref.Inventory.SellingPrice * deliveryref.Quantity;
        ViewData["Units"] = serviceScope == ServiceType.BOOK ? bookref.Inventory.Unit : deliveryref.Inventory.Unit;
        ViewData["Service"] = serviceScope;
        return View(payment);
    } 

    // GET: PAYMENTS/Edit/5
    [Authorize("Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        { 
            return NotFound();
        }

        var payment = await _context.Payment.FindAsync(id);
        if (payment == null)
        {
            return NotFound();
        }
        return View(payment);
    }

    // POST: PAYMENTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost,ActionName("Edit")]
    [Authorize("Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int? id, [Bind("id,ItemName,Quantity,UserId,User,PaymentMethod,PaymentDate")] Payment payment)
    {
        if (id != payment.id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(payment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(payment.id))
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
        return View(payment);
    }

    // GET: PAYMENTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var payment = await _context.Payment
            .FirstOrDefaultAsync(m => m.id == id);
        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    // POST: PAYMENTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var payment = await _context.Payment.FindAsync(id);
        if (payment != null)
        {
            _context.Payment.Remove(payment);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PaymentExists(int? id)
    {
        return _context.Payment.Any(e => e.id == id);
    }
}
