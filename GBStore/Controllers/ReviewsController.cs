using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GBStore.Data;
using GBStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace GBStore.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly GbstoreContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewsController(GbstoreContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var gbstoreContext = _context.Reviews.Include(r => r.Customer).Include(r => r.Product);
            return View(await gbstoreContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCustomerReview(int productId, int rating, string comment)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy CustomerId từ login
            if (customerId == null)
            {
                return Unauthorized();
            }

            var review = new Review
            {
                ProductId = productId,
                CustomerId = customerId,
                Rating = rating,
                Comment = comment,
                CreatedDate = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Home", new { id = productId });
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null) return NotFound();

            // chỉ cho phép chủ review chỉnh sửa
            var userId = _userManager.GetUserId(User);
            if (review.CustomerId != userId) return Forbid();

            return View(review);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,ProductId,Rating,Comment")] Review editedReview)
        {
            if (id != editedReview.ReviewId) return NotFound();

            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (review.CustomerId != userId) return Forbid();

            review.Rating = editedReview.Rating;
            review.Comment = editedReview.Comment;
            review.CreatedDate = DateTime.Now; // hoặc giữ ngày tạo cũ nếu muốn

            _context.Update(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Home", new { id = review.ProductId });
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (review.CustomerId != userId) return Forbid();

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (review.CustomerId != userId) return Forbid();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Home", new { id = review.ProductId });
        }
    }
}
