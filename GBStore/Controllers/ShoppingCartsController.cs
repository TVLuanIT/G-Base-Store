using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GBStore.Data;
using GBStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace GBStore.Controllers
{
    [Authorize]
    public class ShoppingCartsController : Controller
    {
        private readonly GbstoreContext _context;

        public ShoppingCartsController(GbstoreContext context)
        {
            _context = context;
        }

        // GET: /ShoppingCarts/Payment
        [HttpGet]
        public async Task<IActionResult> Payment()
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim == null)
                return RedirectToAction("Index");

            int customerId = int.Parse(customerIdClaim.Value);

            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ShoppingCartStatus == "ACTIVE");

            if (cart == null)
                return RedirectToAction("Index");

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer != null)
            {
                ViewBag.CustomerFullName = customer.Name;
                ViewBag.CustomerPhone = customer.Phone;
                ViewBag.CustomerAddress = customer.CustomerAddress;
                ViewBag.CustomerNote = "";
            }

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(int shoppingCartId, string fullName, string phone, string address, string note)
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim == null)
                return RedirectToAction("Index");

            int customerId = int.Parse(customerIdClaim.Value);

            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c =>
                    c.ShoppingCartId == shoppingCartId &&
                    c.CustomerId == customerId &&
                    c.ShoppingCartStatus == "ACTIVE");

            if (cart == null || !cart.ShoppingCartItems.Any())
                return RedirectToAction("Index");

            // ✅ Kiểm tra thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(address))
            {
                // Trả về lại view Payment với thông báo lỗi
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin trước khi thanh toán.");
                ViewBag.CustomerFullName = fullName;
                ViewBag.CustomerPhone = phone;
                ViewBag.CustomerAddress = address;
                ViewBag.CustomerNote = note;
                return View(cart);
            }

            // 1️⃣ Tạo Order
            var order = new Order
            {
                CustomerId = customerId,
                CustomerName = fullName,
                Phone = phone,
                ShippingAddress = address,
                Note = note,
                OrderDate = DateTime.Now,
                TotalAmount = cart.ShoppingCartItems.Sum(i => (i.Quantity ?? 0) * i.Product.Price) + 22,
                OrderStatus = "PENDING"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 2️⃣ Tạo OrderDetails
            foreach (var item in cart.ShoppingCartItems)
            {
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity ?? 0,
                    Price = item.Product.Price
                });
            }

            // 3️⃣ Đóng cart
            cart.ShoppingCartStatus = "CHECKED_OUT";
            _context.ShoppingCartItems.RemoveRange(cart.ShoppingCartItems);

            await _context.SaveChangesAsync();

            return RedirectToAction("OrderSuccess", new { orderId = order.OrderId });
        }

        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim == null)
                return Redirect("/Identity/Account/Login");

            int customerId = int.Parse(customerIdClaim.Value);

            // Lấy giỏ hàng ACTIVE
            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ShoppingCartStatus == "ACTIVE");

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    CustomerId = customerId,
                    ShoppingCartStatus = "ACTIVE",
                    CreatedDate = DateTime.Now
                };
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Thêm sản phẩm
            var item = cart.ShoppingCartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
                item.Quantity += quantity;
            else
                cart.ShoppingCartItems.Add(new ShoppingCartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });

            await _context.SaveChangesAsync();

            // Redirect về giỏ hàng
            return RedirectToAction("Details", "Home", new { id = productId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int ProductId)
        {
            var item = _context.ShoppingCartItems
                       .FirstOrDefault(i => i.ProductId == ProductId);
            if (item != null)
            {
                _context.ShoppingCartItems.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int itemId, int quantity)
        {
            var item = _context.ShoppingCartItems.Find(itemId);
            if (item != null)
            {
                item.Quantity = quantity;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
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
                .Include(c=>c.ShoppingCartItems)
                    .ThenInclude(i=>i.Product.Brand)
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

        // GET: ShoppingCarts/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            return View();
        }

        // POST: ShoppingCarts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShoppingCartId,CustomerId,CreatedDate,ShoppingCartStatus")] ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoppingCart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", shoppingCart.CustomerId);
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.ShoppingCarts.FindAsync(id);
            if (shoppingCart == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", shoppingCart.CustomerId);
            return View(shoppingCart);
        }

        // POST: ShoppingCarts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShoppingCartId,CustomerId,CreatedDate,ShoppingCartStatus")] ShoppingCart shoppingCart)
        {
            if (id != shoppingCart.ShoppingCartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingCart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingCartExists(shoppingCart.ShoppingCartId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", shoppingCart.CustomerId);
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: ShoppingCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingCart = await _context.ShoppingCarts.FindAsync(id);
            if (shoppingCart != null)
            {
                _context.ShoppingCarts.Remove(shoppingCart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingCartExists(int id)
        {
            return _context.ShoppingCarts.Any(e => e.ShoppingCartId == id);
        }
    }
}
