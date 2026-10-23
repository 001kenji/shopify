
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Management_System.Models;
using Shop_Management_System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class ReviewsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReviewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: REVIEWS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Review
            .Include(e => e.likes)
            .Include(e=> e.User)
            .AsSplitQuery()
            .ToListAsync());
    }

    // GET: REVIEWS/Details/5
    [Authorize]
    public async Task<IActionResult> Details(int? id)
    {  // only the reviewing user can veiw the details the other will see what is displayed on the table
        if (id == null)
        {
            return NotFound();
        }

        var review = await _context.Review
            .Include(e => e.likes)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (review == null)
        {
            return NotFound();
        }
        var userref = await _context.Users.FindAsync(review.UserId);
        ViewData["UserName"] = userref?.UserName ?? userref?.Email ?? "Anonymous";
        return View(review);
    }

    // GET: REVIEWS/Create
    [Authorize]
    public async Task<IActionResult> Create(int? Id, ServiceReviewed? serviceScope)
    {
        if (Id == null || serviceScope == null) return BadRequest("An item is needed for review.");

        int invId = 0;
        string userId = string.Empty;

        if (serviceScope.Value == ServiceReviewed.BOOK)
        {
            var bookref = await _context.Booking.FindAsync(Id.Value);
            if (bookref == null) return NotFound("Booking not found.");

            invId = bookref.InventoryID;
            userId = bookref.UserId;
        }
        else if (serviceScope.Value == ServiceReviewed.DELIVER)
        {
            var deliveryref = await _context.Delivery.FindAsync(Id.Value);
            if (deliveryref == null) return NotFound("Delivery not found.");

            invId = deliveryref.InventoryID;
            userId = deliveryref.UserId;
        }
        else
        {
            return BadRequest("Invalid service scope.");
        }

        var reviewref = new Review
        {
            Service = serviceScope.Value,
            UserId = userId,
            InventoryID = invId,
        };

        var inventoryref = await _context.Inventory.FindAsync(invId);
        if (inventoryref == null) return NotFound("Inventory not found."); // Safety check

        ViewData["Name"] = inventoryref.Name;
        ViewData["Description"] = inventoryref.Description;
        return View(reviewref);
    }

    // POST: REVIEWS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost, ActionName("Create")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> CreatePost([Bind("InventoryID,Details,Ratings,Service")] Review review)
    {
        var inventoryref = await _context.Inventory.FindAsync(review.InventoryID);
        if (inventoryref == null) return NotFound("Inventory not found.");

        ModelState.Remove("Inventory");
        ModelState.Remove("User");
        ModelState.Remove("UserId");
        ModelState.Remove("PostedOn");

        if (ModelState.IsValid)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var userref = await _context.Users.FindAsync(userId);
            if (userref != null)
            {
                review.User = userref;
                review.UserId = userId;
            }
            else
            {
                return BadRequest("User not found.");
            }

            review.PostedOn = DateTime.Now;

            // REMOVED: review.Inventory = inventoryref; 
            // (EF Core will link it automatically via InventoryID)

            if (review.Service == ServiceReviewed.BOOK)
            {
                var bookref = await _context.Booking.FirstOrDefaultAsync(b => b.InventoryID == review.InventoryID);

                if (bookref != null)
                {
                    bookref.Status = BookingStatus.Reviewed;
                    // EXPLICITLY tell EF Core this is an update, not an insert
                    _context.Entry(bookref).State = EntityState.Modified;
                }
            }
            else if (review.Service == ServiceReviewed.DELIVER)
            {
                var deliveryref = await _context.Delivery.FirstOrDefaultAsync(d => d.InventoryID == review.InventoryID);
                if (deliveryref != null)
                {
                    deliveryref.Status = DeliveryStatus.Reviewed;
                    // EXPLICITLY tell EF Core this is an update
                    _context.Entry(deliveryref).State = EntityState.Modified;
                }
            }

            _context.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["Name"] = inventoryref.Name;
        ViewData["Description"] = inventoryref.Description;
        return View(review);
    }

    // GET: REVIEWS/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        return BadRequest("Reviews are not editable.");
    }

    // POST: REVIEWS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,UserId,InventoryID,Inventory,User,Details,Ratings,PostedOn,likes")] Review review)
    {
        
        return BadRequest("Reviews are not editable.");
    }

    // GET: REVIEWS/Delete/5
    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        

        return BadRequest("You are not allowed to delete a review");
    }

    // POST: REVIEWS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        return BadRequest("You are not allowed to delete a review");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Like(int? Id)
    {
        if (Id == null) return BadRequest("A review is needed for liking");

        // Added .Include(r => r.User) here
        var reviewref = await _context.Review
            .Include(r => r.likes)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == Id);

        if (reviewref == null) return BadRequest("This Review is no longer available for a like");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Now reviewref.User won't be null
        if (currentUserId == reviewref.User?.Id)
            return BadRequest("Reviewers cannot like their own content.");

        // Check if they already liked it (Prevents double-liking)
        if (reviewref.likes.Any(e => e.UserId == currentUserId))
            return RedirectToAction(nameof(Index)); // Or return BadRequest

        var currentUserRef = await _context.Users.FindAsync(currentUserId);
        var newlike = new Likes
        {
            UserId = currentUserId,
            User = currentUserRef,
            ReviewId = reviewref.Id,
            Review = reviewref
        };

        _context.Likes.Add(newlike);
        await _context.SaveChangesAsync();

        // ALWAYS Redirect after a successful POST
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UnLike(int? Id)
    {
        if (Id == null) return BadRequest("A review is needed for unliking");

        var reviewref = await _context.Review
            .Include(r => r.likes)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == Id);

        if (reviewref == null) return BadRequest("This Review is no longer available.");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 1. Reviewers shouldn't unlike their own content (assuming they can't like it anyway)
        if (currentUserId == reviewref.User?.Id)
            return BadRequest("Reviewers cannot unlike their own content.");

        // 2. Find the specific Like record
        var likesref = reviewref.likes.FirstOrDefault(e => e.UserId == currentUserId);

        // 3. If the like doesn't exist, they can't unlike it!
        if (likesref == null)
        {
            // They haven't liked it, just redirect back to the page
            return RedirectToAction(nameof(Index));
        }

        // 4. Remove the like and save
        _context.Likes.Remove(likesref);
        await _context.SaveChangesAsync();

        // ALWAYS Redirect after a successful POST
        return RedirectToAction(nameof(Index));
    }

    private bool ReviewExists(int? id)
    {
        return _context.Review.Any(e => e.Id == id);
    }
}
