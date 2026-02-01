using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        private async Task<Customer?> GetCurrentCustomer()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return null;

            return await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        // GET: /ShoppingCarts/Payment
        [HttpGet]
        public async Task<IActionResult> Payment()
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return Redirect("/Identity/Account/Login");

            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c =>
                    c.CustomerId == customer.CustomerId &&
                    c.ShoppingCartStatus == "ACTIVE");

            if (cart == null)
                return RedirectToAction("Index", "ShoppingCartItems");

            ViewBag.CustomerFullName = customer.Name;
            ViewBag.CustomerPhone = customer.Phone;
            ViewBag.CustomerAddress = customer.CustomerAddress;
            ViewBag.CustomerNote = "";

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(
            int shoppingCartId,
            string fullName,
            string phone,
            string address,
            string note)
        {
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return Redirect("/Identity/Account/Login");

            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c =>
                    c.ShoppingCartId == shoppingCartId &&
                    c.CustomerId == customer.CustomerId &&
                    c.ShoppingCartStatus == "ACTIVE");

            if (cart == null || !cart.ShoppingCartItems.Any())
            {
                ViewBag.CartError = "Giỏ hàng trống.";
                return View(cart);
            }

            // Cập nhật thông tin Customer
            customer.Name ??= fullName;
            customer.Phone ??= phone;
            customer.CustomerAddress ??= address;
            await _context.SaveChangesAsync();

            // 🔹 Tạo Order
            var order = new Order
            {
                CustomerId = customer.CustomerId,
                CustomerName = fullName,
                Phone = phone,
                ShippingAddress = address,
                Note = note,
                OrderDate = DateTime.Now,
                TotalAmount = cart.ShoppingCartItems.Sum(i => (i.Quantity ?? 0) * i.Product.Price),
                OrderStatus = "PENDING"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 🔹 OrderDetails
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

            await _context.SaveChangesAsync();

            // 🔹 Đóng cart
            cart.ShoppingCartStatus = "CHECKED_OUT";
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderSuccess", "Orders", new { orderId = order.OrderId });
        }

        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            // Lấy Customer thực tế
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return Redirect("/Identity/Account/Login");

            // Kiểm tra quantity hợp lệ
            if (quantity < 1)
                quantity = 1;

            // Lấy giỏ hàng ACTIVE
            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                .FirstOrDefaultAsync(c =>
                    c.CustomerId == customer.CustomerId &&
                    c.ShoppingCartStatus == "ACTIVE");

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    CustomerId = customer.CustomerId,
                    ShoppingCartStatus = "ACTIVE",
                    CreatedDate = DateTime.Now
                };
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Thêm sản phẩm
            var item = cart.ShoppingCartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
            {
                cart.ShoppingCartItems.Add(new ShoppingCartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }
            else
            {
                // Nếu đã có, cộng dồn số lượng
                item.Quantity += quantity;
            }

            await _context.SaveChangesAsync();

            // Redirect về chi tiết sản phẩm hoặc giỏ hàng
            return RedirectToAction("Details", "Home", new { id = productId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity, string returnUrl)
        {
            // 1️⃣ Lấy Customer hiện tại từ Identity
            var customer = await GetCurrentCustomer();
            if (customer == null)
                return Redirect("/Identity/Account/Login");

            // 2️⃣ Lấy ShoppingCartItem của khách hàng, cart ACTIVE
            var item = await _context.ShoppingCartItems
                .Include(i => i.ShoppingCart)
                .FirstOrDefaultAsync(i =>
                    i.ProductId == productId &&
                    i.ShoppingCart.CustomerId == customer.CustomerId &&
                    i.ShoppingCart.ShoppingCartStatus == "ACTIVE");

            // 3️⃣ Cập nhật số lượng
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                await _context.SaveChangesAsync();
            }

            // 4️⃣ Redirect
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Payment");
        }
    }
}
