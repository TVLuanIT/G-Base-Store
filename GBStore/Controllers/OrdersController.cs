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
using Microsoft.EntityFrameworkCore;

namespace GBStore.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly GbstoreContext _context;

        public OrdersController(GbstoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OrderSuccess(int orderId)
        {
            // 1️ Lấy Customer hiện tại từ Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Redirect("/Identity/Account/Login");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
                return Redirect("/Identity/Account/Login");

            int customerId = customer.CustomerId;

            // 2️ Lấy Order chỉ của Customer hiện tại
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.CustomerId == customerId);

            if (order == null)
                return NotFound(); // hoặc redirect về MyOrders

            // 3️ Trả về View
            return View(order);
        }

        public async Task<IActionResult> MyOrders()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
                return Redirect("/Identity/Account/Login");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Redirect("/Identity/Account/Login");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
                return RedirectToAction("Index", "Home"); // hoặc tạo Customer tự động

            var orders = await _context.Orders
                .Where(o => o.CustomerId == customer.CustomerId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }
}
