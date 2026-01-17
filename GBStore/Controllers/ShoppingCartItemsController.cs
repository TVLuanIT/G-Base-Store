using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GBStore.Data;
using GBStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace GBStore.Controllers
{
    public class ShoppingCartItemsController : Controller
    {
        private readonly GbstoreContext _context;

        public ShoppingCartItemsController(GbstoreContext context)
        {
            _context = context;
        }

        // GET: ShoppingCarts
        public async Task<IActionResult> Index()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            int customerId = int.Parse(customerIdClaim.Value);

            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                    .ThenInclude(i => i.Product)
                .Include(c => c.ShoppingCartItems)
                    .ThenInclude(i => i.Product.Brand)
                .Include(c => c.ShoppingCartItems)
                    .ThenInclude(i => i.Product.Category)  // load Category của Product
                .Include(c => c.ShoppingCartItems)
                    .ThenInclude(i => i.Product.Tag)       // load Tag của Product
                .FirstOrDefaultAsync(c =>
                    c.CustomerId == customerId &&
                    c.ShoppingCartStatus == "ACTIVE");

            var items = cart?.ShoppingCartItems.ToList() ?? new List<ShoppingCartItem>();

            return View(items); // gửi danh sách ShoppingCartItem
        }

        // GET: ShoppingCarts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.ShoppingCarts
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(m => m.ShoppingCartId == id);
            if (shoppingCart == null)
            {
                return NotFound();
            }

            return View(shoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int productId, string? returnUrl)
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(customerIdClaim.Value, out int customerId))
            {
                return Unauthorized();
            }

            var item = _context.ShoppingCartItems
                .FirstOrDefault(i =>
                    i.ProductId == productId &&
                    i.ShoppingCart.CustomerId == customerId);

            if (item != null)
            {
                _context.ShoppingCartItems.Remove(item);
                _context.SaveChanges();
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }
    }
}
