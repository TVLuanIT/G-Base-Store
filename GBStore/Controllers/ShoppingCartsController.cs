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
            {
                return RedirectToAction("Index", "ShoppingCartItems");
            }

            int customerId = int.Parse(customerIdClaim.Value);

            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ShoppingCartStatus == "ACTIVE");

            if (cart == null)
            {
                return RedirectToAction("Index", "ShoppingCartItems");
            }
            

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
            {
                return RedirectToAction("Index", "ShoppingCartItems");
            }

            int customerId = int.Parse(customerIdClaim.Value);

            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c =>
                    c.ShoppingCartId == shoppingCartId &&
                    c.CustomerId == customerId &&
                    c.ShoppingCartStatus == "ACTIVE");

            if (cart == null || !cart.ShoppingCartItems.Any())
            {
                ViewBag.CartError = "Giỏ hàng của bạn hiện tại không có sản phẩm nào.";
                return View(new ShoppingCart { ShoppingCartItems = new List<ShoppingCartItem>() });
            }

            //Kiểm tra thông tin bắt buộc
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

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer != null)
            {
                if (string.IsNullOrWhiteSpace(customer.Name))
                {
                    customer.Name = fullName;
                }

                if (string.IsNullOrWhiteSpace(customer.Phone))
                {
                    customer.Phone = phone;
                }

                if (string.IsNullOrWhiteSpace(customer.CustomerAddress))
                {
                    customer.CustomerAddress = address;
                }

                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
            }

            // 1️ Tạo Order
            var order = new Order
            {
                CustomerId = customerId,
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

            // 2️ Tạo OrderDetails
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
            await _context.SaveChangesAsync(); // Lưu tất cả chi tiết đơn hàng

            // 3️ Đóng cart
            cart.ShoppingCartStatus = "CHECKED_OUT";
            _context.ShoppingCartItems.RemoveRange(cart.ShoppingCartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderSuccess", "Orders", new { orderId = order.OrderId });
        }

        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim == null)
                return Redirect("/Identity/Account/Login");

            if (!int.TryParse(customerIdClaim.Value, out int customerId))
            {
                return Redirect("/Identity/Account/Login");
            }

            // 🔹 Kiểm tra quantity hợp lệ
            if (quantity < 1)
            {
                quantity = 1;
            }

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
            if (item == null)
            {
                // Chưa có → thêm mới
                cart.ShoppingCartItems.Add(new ShoppingCartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();

            // Redirect về giỏ hàng
            return RedirectToAction("Details", "Home", new { id = productId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int productId, int quantity, string returnUrl)
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim == null)
                return Redirect("/Identity/Account/Login");

            if (!int.TryParse(customerIdClaim.Value, out int customerId))
                return Redirect("/Identity/Account/Login");

            var item = _context.ShoppingCartItems
                .FirstOrDefault(i => i.ProductId == productId &&
                                     i.ShoppingCart.CustomerId == customerId &&
                                     i.ShoppingCart.ShoppingCartStatus == "ACTIVE");

            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                _context.SaveChanges();
            }

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Payment");
        }
    }
}
